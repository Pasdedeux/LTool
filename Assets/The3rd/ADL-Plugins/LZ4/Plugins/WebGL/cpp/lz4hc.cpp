

/**************************************
*  Tuning Parameter
**************************************/
static const int LZ3HC_compressionLevel_default = 9;


/**************************************
*  Includes
**************************************/
#include "lz4hc.h"


/**************************************
*  Local Compiler Options
**************************************/
#if defined(__GNUC__)
#  pragma GCC diagnostic ignored "-Wunused-function"
#endif

#if defined (__clang__)
#  pragma clang diagnostic ignored "-Wunused-function"
#endif


/**************************************
*  Common LZ3 definition
**************************************/
#define LZ3__COMMONDEFS_ONLY
#include "lz4.h"


/**************************************
*  Compiler Options
**************************************/
#ifdef _MSC_VER    /* Visual Studio */
#  define FORCE_INLINE static __forceinline
#  include <intrin.h>
#  pragma warning(disable : 4127)        /* disable: C4127: conditional expression is constant */
#  pragma warning(disable : 4293)        /* disable: C4293: too large shift (32-bits) */
#else
#  if defined(__STDC_VERSION__) && (__STDC_VERSION__ >= 199901L)   /* C99 */
#    if defined(__GNUC__) || defined(__clang__)
#      define FORCE_INLINE static inline __attribute__((always_inline))
#    else
#      define FORCE_INLINE static inline
#    endif
#  else
#    define FORCE_INLINE static
#  endif   /* __STDC_VERSION__ */
#endif  /* _MSC_VER */

/* LZ3__GCC_VERSION is defined into lz3.h */
#if (LZ3__GCC_VERSION >= 302) || (__INTEL_COMPILER >= 800) || defined(__clang__)
#  define expect(expr,value)    (__builtin_expect ((expr),(value)) )
#else
#  define expect(expr,value)    (expr)
#endif

#define likely(expr)     expect((expr) != 0, 1)
#define unlikely(expr)   expect((expr) != 0, 0)

/**************************************
*  Memory routines
**************************************/
#include <stdlib.h>   /* malloc, calloc, free */
#define ALLOCATOR(n,s) calloc(n,s)
#define FREEMEM        free
#include <string.h>   /* memset, memcpy */
#define MEM_INIT       memset


#if defined (__STDC_VERSION__) && (__STDC_VERSION__ >= 199901L)   /* C99 */
# include <stdint.h>
  typedef  uint8_t BYTE;
  typedef uint16_t U16;
  typedef uint32_t U32;
  typedef  int32_t S32;
  typedef uint64_t U64;
#else
  typedef unsigned char       BYTE;
  typedef unsigned short      U16;
  typedef unsigned int        U32;
  typedef   signed int        S32;
  typedef unsigned long long  U64;
#endif


/**************************************
*  Reading and writing into memory
**************************************/
#define STEPSIZE sizeof(size_t)

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

static U64 LZ3__read64(const void* memPtr)
{
    U64 val64;
    memcpy(&val64, memPtr, 8);
    return val64;
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

#define LZ3__STATIC_ASSERT(c)    { enum { LZ3__static_assert = 1/(int)(!!(c)) }; }   /* use only *after* variable declarations */

#define LZ3__COMPRESSBOUND(isize)  ((unsigned)(isize) > (unsigned)LZ3__MAX_INPUT_SIZE ? 0 : (isize) + ((isize)/255) + 16)
int LZ3__compressBound(int inputSize);
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

static U32 LZ3HC_hashPtr(const void* ptr) { return HASH_FUNCTION(LZ3__read32(ptr)); }



/**************************************
*  HC Compression
**************************************/
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
FORCE_INLINE void LZ3HC_Insert (LZ3HC_Data_Structure* hc4, const BYTE* ip)
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


FORCE_INLINE int LZ3HC_InsertAndFindBestMatch (LZ3HC_Data_Structure* hc4,   /* Index table will be updated */
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


FORCE_INLINE int LZ3HC_InsertAndGetWiderMatch (
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


typedef enum { noLimit = 0, limitedOutput = 1 } limitedOutput_directive;

#define LZ3HC_DEBUG 0
#if LZ3HC_DEBUG
static unsigned debug = 0;
#endif

FORCE_INLINE int LZ3HC_encodeSequence (
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

#if LZ3HC_DEBUG
    if (debug) printf("literal : %u  --  match : %u  --  offset : %u\n", (U32)(*ip - *anchor), (U32)matchLength, (U32)(*ip-match));
#endif

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


static int LZ3HC_compress_generic (
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


int LZ3__sizeofStateHC(void) { return sizeof(LZ3HC_Data_Structure); }

int LZ3__compress_HC_extStateHC (void* state, const char* src, char* dst, int srcSize, int maxDstSize, int compressionLevel)
{
    if (((size_t)(state)&(sizeof(void*)-1)) != 0) return 0;   /* Error : state is not aligned for pointers (32 or 64 bits) */
    LZ3HC_init ((LZ3HC_Data_Structure*)state, (const BYTE*)src);
    if (maxDstSize < LZ3__compressBound(srcSize))
        return LZ3HC_compress_generic (state, src, dst, srcSize, maxDstSize, compressionLevel, limitedOutput);
    else
        return LZ3HC_compress_generic (state, src, dst, srcSize, maxDstSize, compressionLevel, noLimit);
}

int LZ3__compress_HC(const char* src, char* dst, int srcSize, int maxDstSize, int compressionLevel)
{
    LZ3HC_Data_Structure state;
    return LZ3__compress_HC_extStateHC(&state, src, dst, srcSize, maxDstSize, compressionLevel);
}



/**************************************
*  Streaming Functions
**************************************/
/* allocation */
LZ3__streamHC_t* LZ3__createStreamHC(void) { return (LZ3__streamHC_t*)malloc(sizeof(LZ3__streamHC_t)); }
int             LZ3__freeStreamHC (LZ3__streamHC_t* LZ3__streamHCPtr) { free(LZ3__streamHCPtr); return 0; }


/* initialization */
void LZ3__resetStreamHC (LZ3__streamHC_t* LZ3__streamHCPtr, int compressionLevel)
{
    LZ3__STATIC_ASSERT(sizeof(LZ3HC_Data_Structure) <= sizeof(LZ3__streamHC_t));   /* if compilation fails here, LZ3__STREAMHCSIZE must be increased */
    ((LZ3HC_Data_Structure*)LZ3__streamHCPtr)->base = NULL;
    ((LZ3HC_Data_Structure*)LZ3__streamHCPtr)->compressionLevel = (unsigned)compressionLevel;
}

int LZ3__loadDictHC (LZ3__streamHC_t* LZ3__streamHCPtr, const char* dictionary, int dictSize)
{
    LZ3HC_Data_Structure* ctxPtr = (LZ3HC_Data_Structure*) LZ3__streamHCPtr;
    if (dictSize > 64 KB)
    {
        dictionary += dictSize - 64 KB;
        dictSize = 64 KB;
    }
    LZ3HC_init (ctxPtr, (const BYTE*)dictionary);
    if (dictSize >= 4) LZ3HC_Insert (ctxPtr, (const BYTE*)dictionary +(dictSize-3));
    ctxPtr->end = (const BYTE*)dictionary + dictSize;
    return dictSize;
}


/* compression */

static void LZ3HC_setExternalDict(LZ3HC_Data_Structure* ctxPtr, const BYTE* newBlock)
{
    if (ctxPtr->end >= ctxPtr->base + 4)
        LZ3HC_Insert (ctxPtr, ctxPtr->end-3);   /* Referencing remaining dictionary content */
    /* Only one memory segment for extDict, so any previous extDict is lost at this stage */
    ctxPtr->lowLimit  = ctxPtr->dictLimit;
    ctxPtr->dictLimit = (U32)(ctxPtr->end - ctxPtr->base);
    ctxPtr->dictBase  = ctxPtr->base;
    ctxPtr->base = newBlock - ctxPtr->dictLimit;
    ctxPtr->end  = newBlock;
    ctxPtr->nextToUpdate = ctxPtr->dictLimit;   /* match referencing will resume from there */
}

static int LZ3__compressHC_continue_generic (LZ3HC_Data_Structure* ctxPtr,
                                            const char* source, char* dest,
                                            int inputSize, int maxOutputSize, limitedOutput_directive limit)
{
    /* auto-init if forgotten */
    if (ctxPtr->base == NULL)
        LZ3HC_init (ctxPtr, (const BYTE*) source);

    /* Check overflow */
    if ((size_t)(ctxPtr->end - ctxPtr->base) > 2 GB)
    {
        size_t dictSize = (size_t)(ctxPtr->end - ctxPtr->base) - ctxPtr->dictLimit;
        if (dictSize > 64 KB) dictSize = 64 KB;

        LZ3__loadDictHC((LZ3__streamHC_t*)ctxPtr, (const char*)(ctxPtr->end) - dictSize, (int)dictSize);
    }

    /* Check if blocks follow each other */
    if ((const BYTE*)source != ctxPtr->end)
        LZ3HC_setExternalDict(ctxPtr, (const BYTE*)source);

    /* Check overlapping input/dictionary space */
    {
        const BYTE* sourceEnd = (const BYTE*) source + inputSize;
        const BYTE* dictBegin = ctxPtr->dictBase + ctxPtr->lowLimit;
        const BYTE* dictEnd   = ctxPtr->dictBase + ctxPtr->dictLimit;
        if ((sourceEnd > dictBegin) && ((const BYTE*)source < dictEnd))
        {
            if (sourceEnd > dictEnd) sourceEnd = dictEnd;
            ctxPtr->lowLimit = (U32)(sourceEnd - ctxPtr->dictBase);
            if (ctxPtr->dictLimit - ctxPtr->lowLimit < 4) ctxPtr->lowLimit = ctxPtr->dictLimit;
        }
    }

    return LZ3HC_compress_generic (ctxPtr, source, dest, inputSize, maxOutputSize, ctxPtr->compressionLevel, limit);
}

int LZ3__compress_HC_continue (LZ3__streamHC_t* LZ3__streamHCPtr, const char* source, char* dest, int inputSize, int maxOutputSize)
{
    if (maxOutputSize < LZ3__compressBound(inputSize))
        return LZ3__compressHC_continue_generic ((LZ3HC_Data_Structure*)LZ3__streamHCPtr, source, dest, inputSize, maxOutputSize, limitedOutput);
    else
        return LZ3__compressHC_continue_generic ((LZ3HC_Data_Structure*)LZ3__streamHCPtr, source, dest, inputSize, maxOutputSize, noLimit);
}


/* dictionary saving */

int LZ3__saveDictHC (LZ3__streamHC_t* LZ3__streamHCPtr, char* safeBuffer, int dictSize)
{
    LZ3HC_Data_Structure* streamPtr = (LZ3HC_Data_Structure*)LZ3__streamHCPtr;
    int prefixSize = (int)(streamPtr->end - (streamPtr->base + streamPtr->dictLimit));
    if (dictSize > 64 KB) dictSize = 64 KB;
    if (dictSize < 4) dictSize = 0;
    if (dictSize > prefixSize) dictSize = prefixSize;
    memmove(safeBuffer, streamPtr->end - dictSize, dictSize);
    {
        U32 endIndex = (U32)(streamPtr->end - streamPtr->base);
        streamPtr->end = (const BYTE*)safeBuffer + dictSize;
        streamPtr->base = streamPtr->end - endIndex;
        streamPtr->dictLimit = endIndex - dictSize;
        streamPtr->lowLimit = endIndex - dictSize;
        if (streamPtr->nextToUpdate < streamPtr->dictLimit) streamPtr->nextToUpdate = streamPtr->dictLimit;
    }
    return dictSize;
}


/***********************************
*  Deprecated Functions
***********************************/
/* Deprecated compression functions */
/* These functions are planned to start generate warnings by r131 approximately */
int LZ3__compressHC(const char* src, char* dst, int srcSize) { return LZ3__compress_HC (src, dst, srcSize, LZ3__compressBound(srcSize), 0); }
int LZ3__compressHC_limitedOutput(const char* src, char* dst, int srcSize, int maxDstSize) { return LZ3__compress_HC(src, dst, srcSize, maxDstSize, 0); }
int LZ3__compressHC2(const char* src, char* dst, int srcSize, int cLevel) { return LZ3__compress_HC (src, dst, srcSize, LZ3__compressBound(srcSize), cLevel); }
int LZ3__compressHC2_limitedOutput(const char* src, char* dst, int srcSize, int maxDstSize, int cLevel) { return LZ3__compress_HC(src, dst, srcSize, maxDstSize, cLevel); }
int LZ3__compressHC_withStateHC (void* state, const char* src, char* dst, int srcSize) { return LZ3__compress_HC_extStateHC (state, src, dst, srcSize, LZ3__compressBound(srcSize), 0); }
int LZ3__compressHC_limitedOutput_withStateHC (void* state, const char* src, char* dst, int srcSize, int maxDstSize) { return LZ3__compress_HC_extStateHC (state, src, dst, srcSize, maxDstSize, 0); }
int LZ3__compressHC2_withStateHC (void* state, const char* src, char* dst, int srcSize, int cLevel) { return LZ3__compress_HC_extStateHC(state, src, dst, srcSize, LZ3__compressBound(srcSize), cLevel); }
int LZ3__compressHC2_limitedOutput_withStateHC (void* state, const char* src, char* dst, int srcSize, int maxDstSize, int cLevel) { return LZ3__compress_HC_extStateHC(state, src, dst, srcSize, maxDstSize, cLevel); }
int LZ3__compressHC_continue (LZ3__streamHC_t* ctx, const char* src, char* dst, int srcSize) { return LZ3__compress_HC_continue (ctx, src, dst, srcSize, LZ3__compressBound(srcSize)); }
int LZ3__compressHC_limitedOutput_continue (LZ3__streamHC_t* ctx, const char* src, char* dst, int srcSize, int maxDstSize) { return LZ3__compress_HC_continue (ctx, src, dst, srcSize, maxDstSize); }


/* Deprecated streaming functions */
/* These functions currently generate deprecation warnings */
int LZ3__sizeofStreamStateHC(void) { return LZ3__STREAMHCSIZE; }

int LZ3__resetStreamStateHC(void* state, char* inputBuffer)
{
    if ((((size_t)state) & (sizeof(void*)-1)) != 0) return 1;   /* Error : pointer is not aligned for pointer (32 or 64 bits) */
    LZ3HC_init((LZ3HC_Data_Structure*)state, (const BYTE*)inputBuffer);
    ((LZ3HC_Data_Structure*)state)->inputBuffer = (BYTE*)inputBuffer;
    return 0;
}

void* LZ3__createHC (char* inputBuffer)
{
    void* hc4 = ALLOCATOR(1, sizeof(LZ3HC_Data_Structure));
    if (hc4 == NULL) return NULL;   /* not enough memory */
    LZ3HC_init ((LZ3HC_Data_Structure*)hc4, (const BYTE*)inputBuffer);
    ((LZ3HC_Data_Structure*)hc4)->inputBuffer = (BYTE*)inputBuffer;
    return hc4;
}

int LZ3__freeHC (void* LZ3HC_Data)
{
    FREEMEM(LZ3HC_Data);
    return (0);
}

int LZ3__compressHC2_continue (void* LZ3HC_Data, const char* source, char* dest, int inputSize, int compressionLevel)
{
    return LZ3HC_compress_generic (LZ3HC_Data, source, dest, inputSize, 0, compressionLevel, noLimit);
}

int LZ3__compressHC2_limitedOutput_continue (void* LZ3HC_Data, const char* source, char* dest, int inputSize, int maxOutputSize, int compressionLevel)
{
    return LZ3HC_compress_generic (LZ3HC_Data, source, dest, inputSize, maxOutputSize, compressionLevel, limitedOutput);
}

char* LZ3__slideInputBufferHC(void* LZ3HC_Data)
{
    LZ3HC_Data_Structure* hc4 = (LZ3HC_Data_Structure*)LZ3HC_Data;
    int dictSize = LZ3__saveDictHC((LZ3__streamHC_t*)LZ3HC_Data, (char*)(hc4->inputBuffer), 64 KB);
    return (char*)(hc4->inputBuffer + dictSize);
}
