


/*****************************
*  Includes
*****************************/
#include <stdio.h>     /* fprintf, fopen, fread, stdin, stdout, fflush, getchar */
#include <stdlib.h>    /* malloc, free */
#include <string.h>    /* strcmp, strlen */
#include <time.h>      /* clock */
#include <sys/types.h> /* stat64 */
#include <sys/stat.h>  /* stat64 */
#include "lz4w.h"
//#include "lz3.h"       /* still required for legacy format */
#include "lz4hc.h"     /* still required for legacy format */

#include "lz4frame.h"





#define LZ3W__BLOCKSIZEID_DEFAULT 7





/**************************************
*  Local Parameters
**************************************/

static int g_blockSizeId = LZ3W__BLOCKSIZEID_DEFAULT;

static int g_streamChecksum = 1;
static int g_blockIndependence = 1;



extern "C"{
 void LZ4releaseBuffer(unsigned char *src)
{
	free(src);
}


 char* LZ4Create_Buffer(int size)
{

	return (char *)malloc(size);
}



 void LZ4AddTo_Buffer(char* dst, int offset, char* buf, int len)
{

	memcpy(dst + offset, buf, len);
}

}

/*
#define LZ3__MAX_INPUT_SIZE        0x7E000000   // 2 113 929 216 bytes 
#define LZ3__COMPRESSBOUND(isize)  ((unsigned)(isize) > (unsigned)LZ3__MAX_INPUT_SIZE ? 0 : (isize) + ((isize)/255) + 16)
static int LZ3__compressBound(int isize)  { return LZ3__COMPRESSBOUND(isize); }
*/

/*
extern "C"{
unsigned char* LZ3CompressBuffer(unsigned const char* src, int inLength, int *sizz, int level) {

	 (*sizz) = 0;
	LZ3F_preferences_t prefs;

    prefs.autoFlush = 1;
    prefs.compressionLevel = level;
    prefs.frameInfo.blockMode = (LZ3F_blockMode_t)g_blockIndependence;
    prefs.frameInfo.blockSizeID = (LZ3F_blockSizeID_t)g_blockSizeId;
    prefs.frameInfo.contentChecksumFlag = (LZ3F_contentChecksum_t)g_streamChecksum;
	
	int max = (int)LZ3F_compressFrameBound(inLength, &prefs);

	unsigned char *dest = (unsigned char *)malloc(((int)max) * sizeof(char));

	//(*sizz) = LZ3__compress_default(src ,  dest, inLength, max);
	
	(*sizz) = LZ3F_compressFrame(dest, max, src, inLength, &prefs);

	return dest;
}
}
*/



 
 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 //
 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 
   typedef unsigned char       BYTE;
  typedef unsigned short      U16;
  typedef unsigned int        U32;
  typedef   signed int        S32;
  typedef unsigned long long  U64;
  
 /* LZ3__GCC_VERSION is defined into lz3.h */
#if (LZ3__GCC_VERSION >= 302) || (__INTEL_COMPILER >= 800) || defined(__clang__)
#  define expect(expr,value)    (__builtin_expect ((expr),(value)) )
#else
#  define expect(expr,value)    (expr)
#endif
 
  #define likely(expr)     expect((expr) != 0, 1)
#define unlikely(expr)   expect((expr) != 0, 0)

  #define MINMATCH 4

#define COPYLENGTH 8
#define LASTLITERALS 5
#define MFLIMIT (COPYLENGTH+MINMATCH)
//static const int LZ3__minLength = (MFLIMIT+1);

#define KB *(1 <<10)
#define MB *(1 <<20)
#define GB *(1U<<30)

#define MAXD_LOG 16
#define MAX_DISTANCE ((1 << MAXD_LOG) - 1)

#define ML_BITS  4
#define ML_MASK  ((1U<<ML_BITS)-1)
#define RUN_BITS (8-ML_BITS)
#define RUN_MASK ((1U<<RUN_BITS)-1)

typedef enum { notLimited = 0, limitedOutput = 1 } limitedOutput_directive;
typedef enum { byPtr, byU32, byU16 } tableType_t;

typedef enum { noDict = 0, withPrefix64k, usingExtDict } dict_directive;
typedef enum { noDictIssue = 0, dictSmall } dictIssue_directive;

typedef enum { endOnOutputSize = 0, endOnInputSize = 1 } endCondition_directive;
typedef enum { full = 0, partial = 1 } earlyEnd_directive;



static unsigned LZ3__64bits(void) { return sizeof(void*)==8; }

static unsigned LZ3__isLittleEndian(void)
{
    const union { U32 i; BYTE c[4]; } one = { 1 };   /* don't use static : performance detrimental  */
    return one.c[0];
}


static U16 LZ3__read16(const void* memPtr)
{
    U16 val16;
    memcpy(&val16, memPtr, 2);
    return val16;
}

static U16 LZ3__readLE16(const void* memPtr)
{
    if (LZ3__isLittleEndian())
    {
        return LZ3__read16(memPtr);
    }
    else
    {
        const BYTE* p = (const BYTE*)memPtr;
        return (U16)((U16)p[0] + (p[1]<<8));
    }
}

static U64 LZ3__read64(const void* memPtr)
{
    U64 val64;
    memcpy(&val64, memPtr, 8);
    return val64;
}

static void LZ3__writeLE16(void* memPtr, U16 value)
{
    if (LZ3__isLittleEndian())
    {
        memcpy(memPtr, &value, 2);
    }
    else
    {
        BYTE* p = (BYTE*)memPtr;
        p[0] = (BYTE) value;
        p[1] = (BYTE)(value>>8);
    }
}

static U32 LZ3__read32(const void* memPtr)
{
    U32 val32;
    memcpy(&val32, memPtr, 4);
    return val32;
}

static size_t LZ3__read_ARCH(const void* p)
{
    if (LZ3__64bits())
        return (size_t)LZ3__read64(p);
    else
        return (size_t)LZ3__read32(p);
}



static void LZ3__copy4(void* dstPtr, const void* srcPtr) { memcpy(dstPtr, srcPtr, 4); }

static void LZ3__copy8(void* dstPtr, const void* srcPtr) { memcpy(dstPtr, srcPtr, 8); }

/* customized version of memcpy, which may overwrite up to 7 bytes beyond dstEnd */
static void LZ3__wildCopy(void* dstPtr, const void* srcPtr, void* dstEnd)
{
    BYTE* d = (BYTE*)dstPtr;
    const BYTE* s = (const BYTE*)srcPtr;
    BYTE* e = (BYTE*)dstEnd;
    do { LZ3__copy8(d,s); d+=8; s+=8; } while (d<e);
}
 
 
 ////////////////////////////------------------------------------/////////////////////////////////-----------------------------//////////////////////////////
 
 
 static inline __attribute__((always_inline)) int LZ3__decompress_generic(
                 const char* const source,
                 char* const dest,
                 int inputSize,
                 int outputSize,         /* If endOnInput==endOnInputSize, this value is the max size of Output Buffer. */

                 int endOnInput,         /* endOnOutputSize, endOnInputSize */
                 int partialDecoding,    /* full, partial */
                 int targetOutputSize,   /* only used if partialDecoding==partial */
                 int dict,               /* noDict, withPrefix64k, usingExtDict */
                 const BYTE* const lowPrefix,  /* == dest if dict == noDict */
                 const BYTE* const dictStart,  /* only if dict==usingExtDict */
                 const size_t dictSize         /* note : = 0 if noDict */
                 )
{
    /* Local Variables */
    const BYTE* ip = (const BYTE*) source;
    const BYTE* const iend = ip + inputSize;

    BYTE* op = (BYTE*) dest;
    BYTE* const oend = op + outputSize;
    BYTE* cpy;
    BYTE* oexit = op + targetOutputSize;
    const BYTE* const lowLimit = lowPrefix - dictSize;

    const BYTE* const dictEnd = (const BYTE*)dictStart + dictSize;
    const size_t dec32table[] = {4, 1, 2, 1, 4, 4, 4, 4};
    const size_t dec64table[] = {0, 0, 0, (size_t)-1, 0, 1, 2, 3};

    const int safeDecode = (endOnInput==endOnInputSize);
    const int checkOffset = ((safeDecode) && (dictSize < (int)(64 KB)));


    /* Special cases */
    if ((partialDecoding) && (oexit> oend-MFLIMIT)) oexit = oend-MFLIMIT;                         /* targetOutputSize too high => decode everything */
    if ((endOnInput) && (unlikely(outputSize==0))) return ((inputSize==1) && (*ip==0)) ? 0 : -1;  /* Empty output buffer */
    if ((!endOnInput) && (unlikely(outputSize==0))) return (*ip==0?1:-1);


    /* Main Loop */
    while (1)
    {
        unsigned token;
        size_t length;
        const BYTE* match;

        /* get literal length */
        token = *ip++;
        if ((length=(token>>ML_BITS)) == RUN_MASK)
        {
            unsigned s;
            do
            {
                s = *ip++;
                length += s;
            }
            while (likely((endOnInput)?ip<iend-RUN_MASK:1) && (s==255));
            if ((safeDecode) && unlikely((size_t)(op+length)<(size_t)(op))) goto _output_error;   /* overflow detection */
            if ((safeDecode) && unlikely((size_t)(ip+length)<(size_t)(ip))) goto _output_error;   /* overflow detection */
        }

        /* copy literals */
        cpy = op+length;
        if (((endOnInput) && ((cpy>(partialDecoding?oexit:oend-MFLIMIT)) || (ip+length>iend-(2+1+LASTLITERALS))) )
            || ((!endOnInput) && (cpy>oend-COPYLENGTH)))
        {
            if (partialDecoding)
            {
                if (cpy > oend) goto _output_error;                           /* Error : write attempt beyond end of output buffer */
                if ((endOnInput) && (ip+length > iend)) goto _output_error;   /* Error : read attempt beyond end of input buffer */
            }
            else
            {
                if ((!endOnInput) && (cpy != oend)) goto _output_error;       /* Error : block decoding must stop exactly there */
                if ((endOnInput) && ((ip+length != iend) || (cpy > oend))) goto _output_error;   /* Error : input must be consumed */
            }
            memcpy(op, ip, length);
            ip += length;
            op += length;
            break;     /* Necessarily EOF, due to parsing restrictions */
        }
        LZ3__wildCopy(op, ip, cpy);
        ip += length; op = cpy;

        /* get offset */
        match = cpy - LZ3__readLE16(ip); ip+=2;
        if ((checkOffset) && (unlikely(match < lowLimit))) goto _output_error;   /* Error : offset outside destination buffer */

        /* get matchlength */
        length = token & ML_MASK;
        if (length == ML_MASK)
        {
            unsigned s;
            do
            {
                if ((endOnInput) && (ip > iend-LASTLITERALS)) goto _output_error;
                s = *ip++;
                length += s;
            } while (s==255);
            if ((safeDecode) && unlikely((size_t)(op+length)<(size_t)op)) goto _output_error;   /* overflow detection */
        }
        length += MINMATCH;

        /* check external dictionary */
        if ((dict==usingExtDict) && (match < lowPrefix))
        {
            if (unlikely(op+length > oend-LASTLITERALS)) goto _output_error;   /* doesn't respect parsing restriction */

            if (length <= (size_t)(lowPrefix-match))
            {
                /* match can be copied as a single segment from external dictionary */
                match = dictEnd - (lowPrefix-match);
                memmove(op, match, length); op += length;
            }
            else
            {
                /* match encompass external dictionary and current segment */
                size_t copySize = (size_t)(lowPrefix-match);
                memcpy(op, dictEnd - copySize, copySize);
                op += copySize;
                copySize = length - copySize;
                if (copySize > (size_t)(op-lowPrefix))   /* overlap within current segment */
                {
                    BYTE* const endOfMatch = op + copySize;
                    const BYTE* copyFrom = lowPrefix;
                    while (op < endOfMatch) *op++ = *copyFrom++;
                }
                else
                {
                    memcpy(op, lowPrefix, copySize);
                    op += copySize;
                }
            }
            continue;
        }

        /* copy repeated sequence */
        cpy = op + length;
        if (unlikely((op-match)<8))
        {
            const size_t dec64 = dec64table[op-match];
            op[0] = match[0];
            op[1] = match[1];
            op[2] = match[2];
            op[3] = match[3];
            match += dec32table[op-match];
            LZ3__copy4(op+4, match);
            op += 8; match -= dec64;
        } else { LZ3__copy8(op, match); op+=8; match+=8; }

        if (unlikely(cpy>oend-12))
        {
            if (cpy > oend-LASTLITERALS) goto _output_error;    /* Error : last LASTLITERALS bytes must be literals */
            if (op < oend-8)
            {
                LZ3__wildCopy(op, match, oend-8);
                match += (oend-8) - op;
                op = oend-8;
            }
            while (op<cpy) *op++ = *match++;
        }
        else
            LZ3__wildCopy(op, match, cpy);
        op=cpy;   /* correction */
    }

    /* end of decoding */
    if (endOnInput)
       return (int) (((char*)op)-dest);     /* Nb of output bytes decoded */
    else
       return (int) (((const char*)ip)-source);   /* Nb of input bytes read */

    /* Overflow error detected */
_output_error:
    return (int) (-(((const char*)ip)-source))-1;
}


 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 //
 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/**************************************
*  Local Constants
**************************************/
#define DICTIONARY_LOGSIZE 16
#define MAXD (1<<DICTIONARY_LOGSIZE)
#define MAXD_MASK (MAXD - 1)

#define HASH_LOG (DICTIONARY_LOGSIZE-1)
#define HASHTABLESIZE (1 << HASH_LOG)
#define HASH_MASK (HASHTABLESIZE - 1)

#define OPTIMAL_ML (int)((ML_MASK-1)+MINMATCH)

static const int g_maxCompressionLevel = 16;

#define MEM_INIT       memset

#define STEPSIZE sizeof(size_t)
 
 
/**************************************
*  Common functions
**************************************/
static unsigned LZ3__NbCommonBytes ( size_t val)
{
    if (LZ3__isLittleEndian())
    {
        if (LZ3__64bits())
        {
#       if defined(_MSC_VER) && defined(_WIN64) && !defined(LZ3__FORCE_SW_BITCOUNT)
            unsigned long r = 0;
            _BitScanForward64( &r, (U64)val );
            return (int)(r>>3);
#       elif (defined(__clang__) || (LZ3__GCC_VERSION >= 304)) && !defined(LZ3__FORCE_SW_BITCOUNT)
            return (__builtin_ctzll((U64)val) >> 3);
#       else
            static const int DeBruijnBytePos[64] = { 0, 0, 0, 0, 0, 1, 1, 2, 0, 3, 1, 3, 1, 4, 2, 7, 0, 2, 3, 6, 1, 5, 3, 5, 1, 3, 4, 4, 2, 5, 6, 7, 7, 0, 1, 2, 3, 3, 4, 6, 2, 6, 5, 5, 3, 4, 5, 6, 7, 1, 2, 4, 6, 4, 4, 5, 7, 2, 6, 5, 7, 6, 7, 7 };
            return DeBruijnBytePos[((U64)((val & -(long long)val) * 0x0218A392CDABBD3FULL)) >> 58];
#       endif
        }
        else /* 32 bits */
        {
#       if defined(_MSC_VER) && !defined(LZ3__FORCE_SW_BITCOUNT)
            unsigned long r;
            _BitScanForward( &r, (U32)val );
            return (int)(r>>3);
#       elif (defined(__clang__) || (LZ3__GCC_VERSION >= 304)) && !defined(LZ3__FORCE_SW_BITCOUNT)
            return (__builtin_ctz((U32)val) >> 3);
#       else
            static const int DeBruijnBytePos[32] = { 0, 0, 3, 0, 3, 1, 3, 0, 3, 2, 2, 1, 3, 2, 0, 1, 3, 3, 1, 2, 2, 2, 2, 0, 3, 1, 2, 0, 1, 0, 1, 1 };
            return DeBruijnBytePos[((U32)((val & -(S32)val) * 0x077CB531U)) >> 27];
#       endif
        }
    }
    else   /* Big Endian CPU */
    {
        if (LZ3__64bits())
        {
#       if defined(_MSC_VER) && defined(_WIN64) && !defined(LZ3__FORCE_SW_BITCOUNT)
            unsigned long r = 0;
            _BitScanReverse64( &r, val );
            return (unsigned)(r>>3);
#       elif (defined(__clang__) || (LZ3__GCC_VERSION >= 304)) && !defined(LZ3__FORCE_SW_BITCOUNT)
            return (__builtin_clzll((U64)val) >> 3);
#       else
            unsigned r;
            if (!(val>>32)) { r=4; } else { r=0; val>>=32; }
            if (!(val>>16)) { r+=2; val>>=8; } else { val>>=24; }
            r += (!val);
            return r;
#       endif
        }
        else /* 32 bits */
        {
#       if defined(_MSC_VER) && !defined(LZ3__FORCE_SW_BITCOUNT)
            unsigned long r = 0;
            _BitScanReverse( &r, (unsigned long)val );
            return (unsigned)(r>>3);
#       elif (defined(__clang__) || (LZ3__GCC_VERSION >= 304)) && !defined(LZ3__FORCE_SW_BITCOUNT)
            return (__builtin_clz((U32)val) >> 3);
#       else
            unsigned r;
            if (!(val>>16)) { r=2; val>>=8; } else { r=0; val>>=24; }
            r += (!val);
            return r;
#       endif
        }
    }
}



 static const int LZ3HC_compressionLevel_default = 9;
 


/**************************************
*  Local Types
**************************************/
typedef struct
{
    U32   hashTable[HASHTABLESIZE];
    U16   chainTable[MAXD];
    const BYTE* end;        /* next block here to continue on current prefix */
    const BYTE* base;       /* All index relative to this position */
    const BYTE* dictBase;   /* alternate base for extDict */
    BYTE* inputBuffer;      /* deprecated */
    U32   dictLimit;        /* below that point, need extDict */
    U32   lowLimit;         /* below that point, no more dict */
    U32   nextToUpdate;     /* index from which to continue dictionary update */
    U32   compressionLevel;
} LZ3HC_Data_Structure;


/**************************************
*  Local Macros
**************************************/
#define HASH_FUNCTION(i)       (((i) * 2654435761U) >> ((MINMATCH*8)-HASH_LOG))
//#define DELTANEXTU16(p)        chainTable[(p) & MAXD_MASK]   /* flexible, MAXD dependent */
#define DELTANEXTU16(p)        chainTable[(U16)(p)]   /* faster */




static unsigned LZ3__count(const BYTE* pIn, const BYTE* pMatch, const BYTE* pInLimit)
{
    const BYTE* const pStart = pIn;

    while (likely(pIn<pInLimit-(STEPSIZE-1)))
    {
        size_t diff = LZ3__read_ARCH(pMatch) ^ LZ3__read_ARCH(pIn);
        if (!diff) { pIn+=STEPSIZE; pMatch+=STEPSIZE; continue; }
        pIn += LZ3__NbCommonBytes(diff);
        return (unsigned)(pIn - pStart);
    }

    if (LZ3__64bits()) if ((pIn<(pInLimit-3)) && (LZ3__read32(pMatch) == LZ3__read32(pIn))) { pIn+=4; pMatch+=4; }
    if ((pIn<(pInLimit-1)) && (LZ3__read16(pMatch) == LZ3__read16(pIn))) { pIn+=2; pMatch+=2; }
    if ((pIn<pInLimit) && (*pMatch == *pIn)) pIn++;
    return (unsigned)(pIn - pStart);
}






static U32 LZ3HC_hashPtr(const void* ptr) { return HASH_FUNCTION(LZ3__read32(ptr)); }


static void LZ3HC_init (LZ3HC_Data_Structure* hc4, const BYTE* start)
{
    MEM_INIT((void*)hc4->hashTable, 0, sizeof(hc4->hashTable));
    MEM_INIT(hc4->chainTable, 0xFF, sizeof(hc4->chainTable));
    hc4->nextToUpdate = 64 KB;
    hc4->base = start - 64 KB;
    hc4->end = start;
    hc4->dictBase = start - 64 KB;
    hc4->dictLimit = 64 KB;
    hc4->lowLimit = 64 KB;
}



/* Update chains up to ip (excluded) */
 static inline __attribute__((always_inline)) void LZ3HC_Insert (LZ3HC_Data_Structure* hc4, const BYTE* ip)
{
    U16* chainTable = hc4->chainTable;
    U32* HashTable  = hc4->hashTable;
    const BYTE* const base = hc4->base;
    const U32 target = (U32)(ip - base);
    U32 idx = hc4->nextToUpdate;

    while(idx < target)
    {
        U32 h = LZ3HC_hashPtr(base+idx);
        size_t delta = idx - HashTable[h];
        if (delta>MAX_DISTANCE) delta = MAX_DISTANCE;
        DELTANEXTU16(idx) = (U16)delta;
        HashTable[h] = idx;
        idx++;
    }

    hc4->nextToUpdate = target;
}



 static inline __attribute__((always_inline))   int LZ3HC_InsertAndFindBestMatch (LZ3HC_Data_Structure* hc4,   /* Index table will be updated */
                                               const BYTE* ip, const BYTE* const iLimit,
                                               const BYTE** matchpos,
                                               const int maxNbAttempts)
{
    U16* const chainTable = hc4->chainTable;
    U32* const HashTable = hc4->hashTable;
    const BYTE* const base = hc4->base;
    const BYTE* const dictBase = hc4->dictBase;
    const U32 dictLimit = hc4->dictLimit;
    const U32 lowLimit = (hc4->lowLimit + 64 KB > (U32)(ip-base)) ? hc4->lowLimit : (U32)(ip - base) - (64 KB - 1);
    U32 matchIndex;
    const BYTE* match;
    int nbAttempts=maxNbAttempts;
    size_t ml=0;

    /* HC4 match finder */
    LZ3HC_Insert(hc4, ip);
    matchIndex = HashTable[LZ3HC_hashPtr(ip)];

    while ((matchIndex>=lowLimit) && (nbAttempts))
    {
        nbAttempts--;
        if (matchIndex >= dictLimit)
        {
            match = base + matchIndex;
            if (*(match+ml) == *(ip+ml)
                && (LZ3__read32(match) == LZ3__read32(ip)))
            {
                size_t mlt = LZ3__count(ip+MINMATCH, match+MINMATCH, iLimit) + MINMATCH;
                if (mlt > ml) { ml = mlt; *matchpos = match; }
            }
        }
        else
        {
            match = dictBase + matchIndex;
            if (LZ3__read32(match) == LZ3__read32(ip))
            {
                size_t mlt;
                const BYTE* vLimit = ip + (dictLimit - matchIndex);
                if (vLimit > iLimit) vLimit = iLimit;
                mlt = LZ3__count(ip+MINMATCH, match+MINMATCH, vLimit) + MINMATCH;
                if ((ip+mlt == vLimit) && (vLimit < iLimit))
                    mlt += LZ3__count(ip+mlt, base+dictLimit, iLimit);
                if (mlt > ml) { ml = mlt; *matchpos = base + matchIndex; }   /* virtual matchpos */
            }
        }
        matchIndex -= DELTANEXTU16(matchIndex);
    }

    return (int)ml;
}




 static inline __attribute__((always_inline))  int LZ3HC_InsertAndGetWiderMatch (
    LZ3HC_Data_Structure* hc4,
    const BYTE* const ip,
    const BYTE* const iLowLimit,
    const BYTE* const iHighLimit,
    int longest,
    const BYTE** matchpos,
    const BYTE** startpos,
    const int maxNbAttempts)
{
    U16* const chainTable = hc4->chainTable;
    U32* const HashTable = hc4->hashTable;
    const BYTE* const base = hc4->base;
    const U32 dictLimit = hc4->dictLimit;
    const BYTE* const lowPrefixPtr = base + dictLimit;
    const U32 lowLimit = (hc4->lowLimit + 64 KB > (U32)(ip-base)) ? hc4->lowLimit : (U32)(ip - base) - (64 KB - 1);
    const BYTE* const dictBase = hc4->dictBase;
    U32   matchIndex;
    int nbAttempts = maxNbAttempts;
    int delta = (int)(ip-iLowLimit);


    /* First Match */
    LZ3HC_Insert(hc4, ip);
    matchIndex = HashTable[LZ3HC_hashPtr(ip)];

    while ((matchIndex>=lowLimit) && (nbAttempts))
    {
        nbAttempts--;
        if (matchIndex >= dictLimit)
        {
            const BYTE* matchPtr = base + matchIndex;
            if (*(iLowLimit + longest) == *(matchPtr - delta + longest))
                if (LZ3__read32(matchPtr) == LZ3__read32(ip))
                {
                    int mlt = MINMATCH + LZ3__count(ip+MINMATCH, matchPtr+MINMATCH, iHighLimit);
                    int back = 0;

                    while ((ip+back>iLowLimit)
                           && (matchPtr+back > lowPrefixPtr)
                           && (ip[back-1] == matchPtr[back-1]))
                            back--;

                    mlt -= back;

                    if (mlt > longest)
                    {
                        longest = (int)mlt;
                        *matchpos = matchPtr+back;
                        *startpos = ip+back;
                    }
                }
        }
        else
        {
            const BYTE* matchPtr = dictBase + matchIndex;
            if (LZ3__read32(matchPtr) == LZ3__read32(ip))
            {
                size_t mlt;
                int back=0;
                const BYTE* vLimit = ip + (dictLimit - matchIndex);
                if (vLimit > iHighLimit) vLimit = iHighLimit;
                mlt = LZ3__count(ip+MINMATCH, matchPtr+MINMATCH, vLimit) + MINMATCH;
                if ((ip+mlt == vLimit) && (vLimit < iHighLimit))
                    mlt += LZ3__count(ip+mlt, base+dictLimit, iHighLimit);
                while ((ip+back > iLowLimit) && (matchIndex+back > lowLimit) && (ip[back-1] == matchPtr[back-1])) back--;
                mlt -= back;
                if ((int)mlt > longest) { longest = (int)mlt; *matchpos = base + matchIndex + back; *startpos = ip+back; }
            }
        }
        matchIndex -= DELTANEXTU16(matchIndex);
    }

    return longest;
}






 static inline __attribute__((always_inline))  int LZ3HC_encodeSequence (
    const BYTE** ip,
    BYTE** op,
    const BYTE** anchor,
    int matchLength,
    const BYTE* const match,
    limitedOutput_directive limitedOutputBuffer,
    BYTE* oend)
{
    int length;
    BYTE* token;

    /* Encode Literal length */
    length = (int)(*ip - *anchor);
    token = (*op)++;
    if ((limitedOutputBuffer) && ((*op + (length>>8) + length + (2 + 1 + LASTLITERALS)) > oend)) return 1;   /* Check output limit */
    if (length>=(int)RUN_MASK) { int len; *token=(RUN_MASK<<ML_BITS); len = length-RUN_MASK; for(; len > 254 ; len-=255) *(*op)++ = 255;  *(*op)++ = (BYTE)len; }
    else *token = (BYTE)(length<<ML_BITS);

    /* Copy Literals */
    LZ3__wildCopy(*op, *anchor, (*op) + length);
    *op += length;

    /* Encode Offset */
    LZ3__writeLE16(*op, (U16)(*ip-match)); *op += 2;

    /* Encode MatchLength */
    length = (int)(matchLength-MINMATCH);
    if ((limitedOutputBuffer) && (*op + (length>>8) + (1 + LASTLITERALS) > oend)) return 1;   /* Check output limit */
    if (length>=(int)ML_MASK) { *token+=ML_MASK; length-=ML_MASK; for(; length > 509 ; length-=510) { *(*op)++ = 255; *(*op)++ = 255; } if (length > 254) { length-=255; *(*op)++ = 255; } *(*op)++ = (BYTE)length; }
    else *token += (BYTE)(length);

    /* Prepare next loop */
    *ip += matchLength;
    *anchor = *ip;

    return 0;
}






static int LZ3HC_compress_generic2 (
    void* ctxvoid,
    const char* source,
    char* dest,
    int inputSize,
    int maxOutputSize,
    int compressionLevel,
    limitedOutput_directive limit
    )
{
    LZ3HC_Data_Structure* ctx = (LZ3HC_Data_Structure*) ctxvoid;
    const BYTE* ip = (const BYTE*) source;
    const BYTE* anchor = ip;
    const BYTE* const iend = ip + inputSize;
    const BYTE* const mflimit = iend - MFLIMIT;
    const BYTE* const matchlimit = (iend - LASTLITERALS);

    BYTE* op = (BYTE*) dest;
    BYTE* const oend = op + maxOutputSize;

    unsigned maxNbAttempts;
    int   ml, ml2, ml3, ml0;
    const BYTE* ref=NULL;
    const BYTE* start2=NULL;
    const BYTE* ref2=NULL;
    const BYTE* start3=NULL;
    const BYTE* ref3=NULL;
    const BYTE* start0;
    const BYTE* ref0;


    /* init */
    if (compressionLevel > g_maxCompressionLevel) compressionLevel = g_maxCompressionLevel;
    if (compressionLevel < 1) compressionLevel = LZ3HC_compressionLevel_default;
    maxNbAttempts = 1 << (compressionLevel-1);
    ctx->end += inputSize;

    ip++;

    /* Main Loop */
    while (ip < mflimit)
    {
        ml = LZ3HC_InsertAndFindBestMatch (ctx, ip, matchlimit, (&ref), maxNbAttempts);
        if (!ml) { ip++; continue; }

        /* saved, in case we would skip too much */
        start0 = ip;
        ref0 = ref;
        ml0 = ml;

_Search2:
        if (ip+ml < mflimit)
            ml2 = LZ3HC_InsertAndGetWiderMatch(ctx, ip + ml - 2, ip + 1, matchlimit, ml, &ref2, &start2, maxNbAttempts);
        else ml2 = ml;

        if (ml2 == ml)  /* No better match */
        {
            if (LZ3HC_encodeSequence(&ip, &op, &anchor, ml, ref, limit, oend)) return 0;
            continue;
        }

        if (start0 < ip)
        {
            if (start2 < ip + ml0)   /* empirical */
            {
                ip = start0;
                ref = ref0;
                ml = ml0;
            }
        }

        /* Here, start0==ip */
        if ((start2 - ip) < 3)   /* First Match too small : removed */
        {
            ml = ml2;
            ip = start2;
            ref =ref2;
            goto _Search2;
        }

_Search3:
        /*
        * Currently we have :
        * ml2 > ml1, and
        * ip1+3 <= ip2 (usually < ip1+ml1)
        */
        if ((start2 - ip) < OPTIMAL_ML)
        {
            int correction;
            int new_ml = ml;
            if (new_ml > OPTIMAL_ML) new_ml = OPTIMAL_ML;
            if (ip+new_ml > start2 + ml2 - MINMATCH) new_ml = (int)(start2 - ip) + ml2 - MINMATCH;
            correction = new_ml - (int)(start2 - ip);
            if (correction > 0)
            {
                start2 += correction;
                ref2 += correction;
                ml2 -= correction;
            }
        }
        /* Now, we have start2 = ip+new_ml, with new_ml = min(ml, OPTIMAL_ML=18) */

        if (start2 + ml2 < mflimit)
            ml3 = LZ3HC_InsertAndGetWiderMatch(ctx, start2 + ml2 - 3, start2, matchlimit, ml2, &ref3, &start3, maxNbAttempts);
        else ml3 = ml2;

        if (ml3 == ml2) /* No better match : 2 sequences to encode */
        {
            /* ip & ref are known; Now for ml */
            if (start2 < ip+ml)  ml = (int)(start2 - ip);
            /* Now, encode 2 sequences */
            if (LZ3HC_encodeSequence(&ip, &op, &anchor, ml, ref, limit, oend)) return 0;
            ip = start2;
            if (LZ3HC_encodeSequence(&ip, &op, &anchor, ml2, ref2, limit, oend)) return 0;
            continue;
        }

        if (start3 < ip+ml+3) /* Not enough space for match 2 : remove it */
        {
            if (start3 >= (ip+ml)) /* can write Seq1 immediately ==> Seq2 is removed, so Seq3 becomes Seq1 */
            {
                if (start2 < ip+ml)
                {
                    int correction = (int)(ip+ml - start2);
                    start2 += correction;
                    ref2 += correction;
                    ml2 -= correction;
                    if (ml2 < MINMATCH)
                    {
                        start2 = start3;
                        ref2 = ref3;
                        ml2 = ml3;
                    }
                }

                if (LZ3HC_encodeSequence(&ip, &op, &anchor, ml, ref, limit, oend)) return 0;
                ip  = start3;
                ref = ref3;
                ml  = ml3;

                start0 = start2;
                ref0 = ref2;
                ml0 = ml2;
                goto _Search2;
            }

            start2 = start3;
            ref2 = ref3;
            ml2 = ml3;
            goto _Search3;
        }

        /*
        * OK, now we have 3 ascending matches; let's write at least the first one
        * ip & ref are known; Now for ml
        */
        if (start2 < ip+ml)
        {
            if ((start2 - ip) < (int)ML_MASK)
            {
                int correction;
                if (ml > OPTIMAL_ML) ml = OPTIMAL_ML;
                if (ip + ml > start2 + ml2 - MINMATCH) ml = (int)(start2 - ip) + ml2 - MINMATCH;
                correction = ml - (int)(start2 - ip);
                if (correction > 0)
                {
                    start2 += correction;
                    ref2 += correction;
                    ml2 -= correction;
                }
            }
            else
            {
                ml = (int)(start2 - ip);
            }
        }
        if (LZ3HC_encodeSequence(&ip, &op, &anchor, ml, ref, limit, oend)) return 0;

        ip = start2;
        ref = ref2;
        ml = ml2;

        start2 = start3;
        ref2 = ref3;
        ml2 = ml3;

        goto _Search3;
    }

    /* Encode Last Literals */
    {
        int lastRun = (int)(iend - anchor);
        if ((limit) && (((char*)op - dest) + lastRun + 1 + ((lastRun+255-RUN_MASK)/255) > (U32)maxOutputSize)) return 0;  /* Check output limit */
        if (lastRun>=(int)RUN_MASK) { *op++=(RUN_MASK<<ML_BITS); lastRun-=RUN_MASK; for(; lastRun > 254 ; lastRun-=255) *op++ = 255; *op++ = (BYTE) lastRun; }
        else *op++ = (BYTE)(lastRun<<ML_BITS);
        memcpy(op, anchor, iend - anchor);
        op += iend-anchor;
    }

    /* End */
    return (int) (((char*)op)-dest);
}


 
 
 extern "C"{
	 int LZ4DecompressBuffer(const char* source, char* dest, int uncompressedSize) {

		 //int h = 0;
		// if (useHeader) h = 4;

		 //return LZ3__decompress_fast(source , dest, uncompressedSize);//source + h
		 return LZ3__decompress_generic(source, dest, 0, uncompressedSize, endOnOutputSize, full, 0, withPrefix64k, (BYTE*)(dest - 64 KB), NULL, 64 KB);
	 }
 }
 



#define LZ3__MAX_INPUT_SIZE        0x7E000000   /* 2 113 929 216 bytes */
#define LZ3__COMPRESSBOUND(isize)  ((unsigned)(isize) > (unsigned)LZ3__MAX_INPUT_SIZE ? 0 : (isize) + ((isize)/255) + 16)
int LZ3__compressBound(int inputSize);
//typedef enum { notLimited = 0, limitedOutput = 1 } limitedOutput_directive;

 int LZ4__compressBound(int isize)  { return LZ3__COMPRESSBOUND(isize); }
 
 
 extern "C"{
	 char* LZ4CompressBuffer(const char* src, int srcSize,  int *sizz, int compressionLevel) {

		 (*sizz) = 0;

		//int max = LZ3__compressBound(inLength);

		LZ3F_preferences_t prefs;
		prefs.autoFlush = 1;
		prefs.compressionLevel = compressionLevel;
		prefs.frameInfo.blockMode = (LZ3F_blockMode_t)g_blockIndependence;
		prefs.frameInfo.blockSizeID = (LZ3F_blockSizeID_t)g_blockSizeId;
		prefs.frameInfo.contentChecksumFlag = (LZ3F_contentChecksum_t)g_streamChecksum;
		
		int maxDstSize = (int)LZ3F_compressFrameBound(srcSize, &prefs);

		
		char *dst = ( char *)malloc(((int)maxDstSize) * sizeof(char));
		
		LZ3HC_Data_Structure state;

		    if (((size_t)(&state)&(sizeof(void*)-1)) != 0) return NULL;   // Error : state is not aligned for pointers (32 or 64 bits)
			
			LZ3HC_init ((LZ3HC_Data_Structure*)&state, (const BYTE*)src);
			
			//if (maxDstSize < LZ3__compressBound(srcSize))//(srcSize) + ((srcSize)/255) + 16;
			if (maxDstSize < (srcSize) + ((srcSize)/255) + 16)
				(*sizz) = LZ3HC_compress_generic2 (&state, src, dst, srcSize, maxDstSize, compressionLevel, limitedOutput);
			else
				(*sizz) = LZ3HC_compress_generic2 (&state, src, dst, srcSize, maxDstSize, compressionLevel, notLimited);
		
		//int LZ3__compress_HC (const char* src, char* dst, int srcSize, int maxDstSize, int compressionLevel);
		//(*sizz) = LZ3__compress_HC(src ,  dest, inLength, max, level);

		return dst;
	}
}


/*
extern "C"{
unsigned char* LZ4CompressBuffer(const char* src, int inLength, int *sizz, int level) {

	 (*sizz) = 0;
	LZ3F_preferences_t prefs;

    prefs.autoFlush = 1;
    prefs.compressionLevel = level;
    prefs.frameInfo.blockMode = (LZ3F_blockMode_t)g_blockIndependence;
    prefs.frameInfo.blockSizeID = (LZ3F_blockSizeID_t)g_blockSizeId;
    prefs.frameInfo.contentChecksumFlag = (LZ3F_contentChecksum_t)g_streamChecksum;
	
	int max = (int)LZ3F_compressFrameBound(inLength, &prefs);

	unsigned char *dest = (unsigned char *)malloc(((int)max) * sizeof(char));

	//(*sizz) = LZ3__compress_default(src ,  dest, inLength, max);
	
	(*sizz) = LZ3F_compressFrame(dest, max, src, inLength, &prefs);

	return dest;
}
}
*/





 