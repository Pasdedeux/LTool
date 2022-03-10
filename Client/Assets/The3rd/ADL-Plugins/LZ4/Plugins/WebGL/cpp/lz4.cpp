
/**************************************
*  Tuning parameters
**************************************/
/*
 * HEAPMODE :
 * Select how default compression functions will allocate memory for their hash table,
 * in memory stack (0:default, fastest), or in memory heap (1:requires malloc()).
 */
#define HEAPMODE 0

/*
 * ACCELERATION_DEFAULT :
 * Select "acceleration" for LZ3__compress_fast() when parameter value <= 0
 */
#define ACCELERATION_DEFAULT 1


/**************************************
*  CPU Feature Detection
**************************************/
/*
 * LZ3__FORCE_SW_BITCOUNT
 * Define this parameter if your target system or compiler does not support hardware bit count
 */
#if defined(_MSC_VER) && defined(_WIN32_WCE)   /* Visual Studio for Windows CE does not support Hardware bit count */
#  define LZ3__FORCE_SW_BITCOUNT
#endif


/**************************************
*  Includes
**************************************/
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


/**************************************
*  Basic Types
**************************************/
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


/**************************************
*  Common Constants
**************************************/
#define MINMATCH 4

#define COPYLENGTH 8
#define LASTLITERALS 5
#define MFLIMIT (COPYLENGTH+MINMATCH)
static const int LZ3__minLength = (MFLIMIT+1);

#define KB *(1 <<10)
#define MB *(1 <<20)
#define GB *(1U<<30)

#define MAXD_LOG 16
#define MAX_DISTANCE ((1 << MAXD_LOG) - 1)

#define ML_BITS  4
#define ML_MASK  ((1U<<ML_BITS)-1)
#define RUN_BITS (8-ML_BITS)
#define RUN_MASK ((1U<<RUN_BITS)-1)


/**************************************
*  Common Utils
**************************************/
#define LZ3__STATIC_ASSERT(c)    { enum { LZ3__static_assert = 1/(int)(!!(c)) }; }   /* use only *after* variable declarations */


/**************************************
*  Common functions
**************************************/
static unsigned LZ3__NbCommonBytes (register size_t val)
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


#ifndef LZ3__COMMONDEFS_ONLY
/**************************************
*  Local Constants
**************************************/
#define LZ3__HASHLOG   (LZ3__MEMORY_USAGE-2)
#define HASHTABLESIZE (1 << LZ3__MEMORY_USAGE)
#define HASH_SIZE_U32 (1 << LZ3__HASHLOG)       /* required as macro for static allocation */

static const int LZ3__64Klimit = ((64 KB) + (MFLIMIT-1));
static const U32 LZ3__skipTrigger = 6;  /* Increase this value ==> compression run slower on incompressible data */


/**************************************
*  Local Structures and types
**************************************/
typedef struct {
    U32 hashTable[HASH_SIZE_U32];
    U32 currentOffset;
    U32 initCheck;
    const BYTE* dictionary;
    BYTE* bufferStart;   /* obsolete, used for slideInputBuffer */
    U32 dictSize;
} LZ3__stream_t_internal;

typedef enum { notLimited = 0, limitedOutput = 1 } limitedOutput_directive;
typedef enum { byPtr, byU32, byU16 } tableType_t;

typedef enum { noDict = 0, withPrefix64k, usingExtDict } dict_directive;
typedef enum { noDictIssue = 0, dictSmall } dictIssue_directive;

typedef enum { endOnOutputSize = 0, endOnInputSize = 1 } endCondition_directive;
typedef enum { full = 0, partial = 1 } earlyEnd_directive;


#define LZ3__COMPRESSBOUND(isize)  ((unsigned)(isize) > (unsigned)LZ3__MAX_INPUT_SIZE ? 0 : (isize) + ((isize)/255) + 16)
int LZ3__compressBound(int inputSize);
/**************************************
*  Local Utils
**************************************/
int LZ3__versionNumber (void) { return LZ3__VERSION_NUMBER; }
int LZ3__compressBound(int isize)  { return LZ3__COMPRESSBOUND(isize); }
int LZ3__sizeofState() { return LZ3__STREAMSIZE; }

//


/********************************
*  Compression functions
********************************/

static U32 LZ3__hashSequence(U32 sequence, tableType_t const tableType)
{
    if (tableType == byU16)
        return (((sequence) * 2654435761U) >> ((MINMATCH*8)-(LZ3__HASHLOG+1)));
    else
        return (((sequence) * 2654435761U) >> ((MINMATCH*8)-LZ3__HASHLOG));
}

static const U64 prime5bytes = 889523592379ULL;
static U32 LZ3__hashSequence64(size_t sequence, tableType_t const tableType)
{
    const U32 hashLog = (tableType == byU16) ? LZ3__HASHLOG+1 : LZ3__HASHLOG;
    const U32 hashMask = (1<<hashLog) - 1;
    return ((sequence * prime5bytes) >> (40 - hashLog)) & hashMask;
}

static U32 LZ3__hashSequenceT(size_t sequence, tableType_t const tableType)
{
    if (LZ3__64bits())
        return LZ3__hashSequence64(sequence, tableType);
    return LZ3__hashSequence((U32)sequence, tableType);
}

static U32 LZ3__hashPosition(const void* p, tableType_t tableType) { return LZ3__hashSequenceT(LZ3__read_ARCH(p), tableType); }

static void LZ3__putPositionOnHash(const BYTE* p, U32 h, void* tableBase, tableType_t const tableType, const BYTE* srcBase)
{
    switch (tableType)
    {
    case byPtr: { const BYTE** hashTable = (const BYTE**)tableBase; hashTable[h] = p; return; }
    case byU32: { U32* hashTable = (U32*) tableBase; hashTable[h] = (U32)(p-srcBase); return; }
    case byU16: { U16* hashTable = (U16*) tableBase; hashTable[h] = (U16)(p-srcBase); return; }
    }
}

static void LZ3__putPosition(const BYTE* p, void* tableBase, tableType_t tableType, const BYTE* srcBase)
{
    U32 h = LZ3__hashPosition(p, tableType);
    LZ3__putPositionOnHash(p, h, tableBase, tableType, srcBase);
}

static const BYTE* LZ3__getPositionOnHash(U32 h, void* tableBase, tableType_t tableType, const BYTE* srcBase)
{
    if (tableType == byPtr) { const BYTE** hashTable = (const BYTE**) tableBase; return hashTable[h]; }
    if (tableType == byU32) { U32* hashTable = (U32*) tableBase; return hashTable[h] + srcBase; }
    { U16* hashTable = (U16*) tableBase; return hashTable[h] + srcBase; }   /* default, to ensure a return */
}

static const BYTE* LZ3__getPosition(const BYTE* p, void* tableBase, tableType_t tableType, const BYTE* srcBase)
{
    U32 h = LZ3__hashPosition(p, tableType);
    return LZ3__getPositionOnHash(h, tableBase, tableType, srcBase);
}

FORCE_INLINE int LZ3__compress_generic(
                 void* const ctx,
                 const char* const source,
                 char* const dest,
                 const int inputSize,
                 const int maxOutputSize,
                 const limitedOutput_directive outputLimited,
                 const tableType_t tableType,
                 const dict_directive dict,
                 const dictIssue_directive dictIssue,
                 const U32 acceleration)
{
    LZ3__stream_t_internal* const dictPtr = (LZ3__stream_t_internal*)ctx;

    const BYTE* ip = (const BYTE*) source;
    const BYTE* base;
    const BYTE* lowLimit;
    const BYTE* const lowRefLimit = ip - dictPtr->dictSize;
    const BYTE* const dictionary = dictPtr->dictionary;
    const BYTE* const dictEnd = dictionary + dictPtr->dictSize;
    const size_t dictDelta = dictEnd - (const BYTE*)source;
    const BYTE* anchor = (const BYTE*) source;
    const BYTE* const iend = ip + inputSize;
    const BYTE* const mflimit = iend - MFLIMIT;
    const BYTE* const matchlimit = iend - LASTLITERALS;

    BYTE* op = (BYTE*) dest;
    BYTE* const olimit = op + maxOutputSize;

    U32 forwardH;
    size_t refDelta=0;

    /* Init conditions */
    if ((U32)inputSize > (U32)LZ3__MAX_INPUT_SIZE) return 0;   /* Unsupported input size, too large (or negative) */
    switch(dict)
    {
    case noDict:
    default:
        base = (const BYTE*)source;
        lowLimit = (const BYTE*)source;
        break;
    case withPrefix64k:
        base = (const BYTE*)source - dictPtr->currentOffset;
        lowLimit = (const BYTE*)source - dictPtr->dictSize;
        break;
    case usingExtDict:
        base = (const BYTE*)source - dictPtr->currentOffset;
        lowLimit = (const BYTE*)source;
        break;
    }
    if ((tableType == byU16) && (inputSize>=LZ3__64Klimit)) return 0;   /* Size too large (not within 64K limit) */
    if (inputSize<LZ3__minLength) goto _last_literals;                  /* Input too small, no compression (all literals) */

    /* First Byte */
    LZ3__putPosition(ip, ctx, tableType, base);
    ip++; forwardH = LZ3__hashPosition(ip, tableType);

    /* Main Loop */
    for ( ; ; )
    {
        const BYTE* match;
        BYTE* token;
        {
            const BYTE* forwardIp = ip;
            unsigned step = 1;
            unsigned searchMatchNb = acceleration << LZ3__skipTrigger;

            /* Find a match */
            do {
                U32 h = forwardH;
                ip = forwardIp;
                forwardIp += step;
                step = (searchMatchNb++ >> LZ3__skipTrigger);

                if (unlikely(forwardIp > mflimit)) goto _last_literals;

                match = LZ3__getPositionOnHash(h, ctx, tableType, base);
                if (dict==usingExtDict)
                {
                    if (match<(const BYTE*)source)
                    {
                        refDelta = dictDelta;
                        lowLimit = dictionary;
                    }
                    else
                    {
                        refDelta = 0;
                        lowLimit = (const BYTE*)source;
                    }
                }
                forwardH = LZ3__hashPosition(forwardIp, tableType);
                LZ3__putPositionOnHash(ip, h, ctx, tableType, base);

            } while ( ((dictIssue==dictSmall) ? (match < lowRefLimit) : 0)
                || ((tableType==byU16) ? 0 : (match + MAX_DISTANCE < ip))
                || (LZ3__read32(match+refDelta) != LZ3__read32(ip)) );
        }

        /* Catch up */
        while ((ip>anchor) && (match+refDelta > lowLimit) && (unlikely(ip[-1]==match[refDelta-1]))) { ip--; match--; }

        {
            /* Encode Literal length */
            unsigned litLength = (unsigned)(ip - anchor);
            token = op++;
            if ((outputLimited) && (unlikely(op + litLength + (2 + 1 + LASTLITERALS) + (litLength/255) > olimit)))
                return 0;   /* Check output limit */
            if (litLength>=RUN_MASK)
            {
                int len = (int)litLength-RUN_MASK;
                *token=(RUN_MASK<<ML_BITS);
                for(; len >= 255 ; len-=255) *op++ = 255;
                *op++ = (BYTE)len;
            }
            else *token = (BYTE)(litLength<<ML_BITS);

            /* Copy Literals */
            LZ3__wildCopy(op, anchor, op+litLength);
            op+=litLength;
        }

_next_match:
        /* Encode Offset */
        LZ3__writeLE16(op, (U16)(ip-match)); op+=2;

        /* Encode MatchLength */
        {
            unsigned matchLength;

            if ((dict==usingExtDict) && (lowLimit==dictionary))
            {
                const BYTE* limit;
                match += refDelta;
                limit = ip + (dictEnd-match);
                if (limit > matchlimit) limit = matchlimit;
                matchLength = LZ3__count(ip+MINMATCH, match+MINMATCH, limit);
                ip += MINMATCH + matchLength;
                if (ip==limit)
                {
                    unsigned more = LZ3__count(ip, (const BYTE*)source, matchlimit);
                    matchLength += more;
                    ip += more;
                }
            }
            else
            {
                matchLength = LZ3__count(ip+MINMATCH, match+MINMATCH, matchlimit);
                ip += MINMATCH + matchLength;
            }

            if ((outputLimited) && (unlikely(op + (1 + LASTLITERALS) + (matchLength>>8) > olimit)))
                return 0;    /* Check output limit */
            if (matchLength>=ML_MASK)
            {
                *token += ML_MASK;
                matchLength -= ML_MASK;
                for (; matchLength >= 510 ; matchLength-=510) { *op++ = 255; *op++ = 255; }
                if (matchLength >= 255) { matchLength-=255; *op++ = 255; }
                *op++ = (BYTE)matchLength;
            }
            else *token += (BYTE)(matchLength);
        }

        anchor = ip;

        /* Test end of chunk */
        if (ip > mflimit) break;

        /* Fill table */
        LZ3__putPosition(ip-2, ctx, tableType, base);

        /* Test next position */
        match = LZ3__getPosition(ip, ctx, tableType, base);
        if (dict==usingExtDict)
        {
            if (match<(const BYTE*)source)
            {
                refDelta = dictDelta;
                lowLimit = dictionary;
            }
            else
            {
                refDelta = 0;
                lowLimit = (const BYTE*)source;
            }
        }
        LZ3__putPosition(ip, ctx, tableType, base);
        if ( ((dictIssue==dictSmall) ? (match>=lowRefLimit) : 1)
            && (match+MAX_DISTANCE>=ip)
            && (LZ3__read32(match+refDelta)==LZ3__read32(ip)) )
        { token=op++; *token=0; goto _next_match; }

        /* Prepare next loop */
        forwardH = LZ3__hashPosition(++ip, tableType);
    }

_last_literals:
    /* Encode Last Literals */
    {
        const size_t lastRun = (size_t)(iend - anchor);
        if ((outputLimited) && ((op - (BYTE*)dest) + lastRun + 1 + ((lastRun+255-RUN_MASK)/255) > (U32)maxOutputSize))
            return 0;   /* Check output limit */
        if (lastRun >= RUN_MASK)
        {
            size_t accumulator = lastRun - RUN_MASK;
            *op++ = RUN_MASK << ML_BITS;
            for(; accumulator >= 255 ; accumulator-=255) *op++ = 255;
            *op++ = (BYTE) accumulator;
        }
        else
        {
            *op++ = (BYTE)(lastRun<<ML_BITS);
        }
        memcpy(op, anchor, lastRun);
        op += lastRun;
    }

    /* End */
    return (int) (((char*)op)-dest);
}


int LZ3__compress_fast_extState(void* state, const char* source, char* dest, int inputSize, int maxOutputSize, int acceleration)
{
    LZ3__resetStream((LZ3__stream_t*)state);
    if (acceleration < 1) acceleration = ACCELERATION_DEFAULT;

    if (maxOutputSize >= LZ3__compressBound(inputSize))
    {
        if (inputSize < LZ3__64Klimit)
            return LZ3__compress_generic(state, source, dest, inputSize, 0, notLimited, byU16,                        noDict, noDictIssue, acceleration);
        else
            return LZ3__compress_generic(state, source, dest, inputSize, 0, notLimited, LZ3__64bits() ? byU32 : byPtr, noDict, noDictIssue, acceleration);
    }
    else
    {
        if (inputSize < LZ3__64Klimit)
            return LZ3__compress_generic(state, source, dest, inputSize, maxOutputSize, limitedOutput, byU16,                        noDict, noDictIssue, acceleration);
        else
            return LZ3__compress_generic(state, source, dest, inputSize, maxOutputSize, limitedOutput, LZ3__64bits() ? byU32 : byPtr, noDict, noDictIssue, acceleration);
    }
}


int LZ3__compress_fast(const char* source, char* dest, int inputSize, int maxOutputSize, int acceleration)
{
#if (HEAPMODE)
    void* ctxPtr = ALLOCATOR(1, sizeof(LZ3__stream_t));   /* malloc-calloc always properly aligned */
#else
    LZ3__stream_t ctx;
    void* ctxPtr = &ctx;
#endif

    int result = LZ3__compress_fast_extState(ctxPtr, source, dest, inputSize, maxOutputSize, acceleration);

#if (HEAPMODE)
    FREEMEM(ctxPtr);
#endif
    return result;
}


int LZ3__compress_default(const char* source, char* dest, int inputSize, int maxOutputSize)
{
    return LZ3__compress_fast(source, dest, inputSize, maxOutputSize, 1);
}


/* hidden debug function */
/* strangely enough, gcc generates faster code when this function is uncommented, even if unused */
int LZ3__compress_fast_force(const char* source, char* dest, int inputSize, int maxOutputSize, int acceleration)
{
    LZ3__stream_t ctx;

    LZ3__resetStream(&ctx);

    if (inputSize < LZ3__64Klimit)
        return LZ3__compress_generic(&ctx, source, dest, inputSize, maxOutputSize, limitedOutput, byU16,                        noDict, noDictIssue, acceleration);
    else
        return LZ3__compress_generic(&ctx, source, dest, inputSize, maxOutputSize, limitedOutput, LZ3__64bits() ? byU32 : byPtr, noDict, noDictIssue, acceleration);
}


/********************************
*  destSize variant
********************************/

static int LZ3__compress_destSize_generic(
                       void* const ctx,
                 const char* const src,
                       char* const dst,
                       int*  const srcSizePtr,
                 const int targetDstSize,
                 const tableType_t tableType)
{
    const BYTE* ip = (const BYTE*) src;
    const BYTE* base = (const BYTE*) src;
    const BYTE* lowLimit = (const BYTE*) src;
    const BYTE* anchor = ip;
    const BYTE* const iend = ip + *srcSizePtr;
    const BYTE* const mflimit = iend - MFLIMIT;
    const BYTE* const matchlimit = iend - LASTLITERALS;

    BYTE* op = (BYTE*) dst;
    BYTE* const oend = op + targetDstSize;
    BYTE* const oMaxLit = op + targetDstSize - 2 /* offset */ - 8 /* because 8+MINMATCH==MFLIMIT */ - 1 /* token */;
    BYTE* const oMaxMatch = op + targetDstSize - (LASTLITERALS + 1 /* token */);
    BYTE* const oMaxSeq = oMaxLit - 1 /* token */;

    U32 forwardH;


    /* Init conditions */
    if (targetDstSize < 1) return 0;                                     /* Impossible to store anything */
    if ((U32)*srcSizePtr > (U32)LZ3__MAX_INPUT_SIZE) return 0;            /* Unsupported input size, too large (or negative) */
    if ((tableType == byU16) && (*srcSizePtr>=LZ3__64Klimit)) return 0;   /* Size too large (not within 64K limit) */
    if (*srcSizePtr<LZ3__minLength) goto _last_literals;                  /* Input too small, no compression (all literals) */

    /* First Byte */
    *srcSizePtr = 0;
    LZ3__putPosition(ip, ctx, tableType, base);
    ip++; forwardH = LZ3__hashPosition(ip, tableType);

    /* Main Loop */
    for ( ; ; )
    {
        const BYTE* match;
        BYTE* token;
        {
            const BYTE* forwardIp = ip;
            unsigned step = 1;
            unsigned searchMatchNb = 1 << LZ3__skipTrigger;

            /* Find a match */
            do {
                U32 h = forwardH;
                ip = forwardIp;
                forwardIp += step;
                step = (searchMatchNb++ >> LZ3__skipTrigger);

                if (unlikely(forwardIp > mflimit))
                    goto _last_literals;

                match = LZ3__getPositionOnHash(h, ctx, tableType, base);
                forwardH = LZ3__hashPosition(forwardIp, tableType);
                LZ3__putPositionOnHash(ip, h, ctx, tableType, base);

            } while ( ((tableType==byU16) ? 0 : (match + MAX_DISTANCE < ip))
                || (LZ3__read32(match) != LZ3__read32(ip)) );
        }

        /* Catch up */
        while ((ip>anchor) && (match > lowLimit) && (unlikely(ip[-1]==match[-1]))) { ip--; match--; }

        {
            /* Encode Literal length */
            unsigned litLength = (unsigned)(ip - anchor);
            token = op++;
            if (op + ((litLength+240)/255) + litLength > oMaxLit)
            {
                /* Not enough space for a last match */
                op--;
                goto _last_literals;
            }
            if (litLength>=RUN_MASK)
            {
                unsigned len = litLength - RUN_MASK;
                *token=(RUN_MASK<<ML_BITS);
                for(; len >= 255 ; len-=255) *op++ = 255;
                *op++ = (BYTE)len;
            }
            else *token = (BYTE)(litLength<<ML_BITS);

            /* Copy Literals */
            LZ3__wildCopy(op, anchor, op+litLength);
            op += litLength;
        }

_next_match:
        /* Encode Offset */
        LZ3__writeLE16(op, (U16)(ip-match)); op+=2;

        /* Encode MatchLength */
        {
            size_t matchLength;

            matchLength = LZ3__count(ip+MINMATCH, match+MINMATCH, matchlimit);

            if (op + ((matchLength+240)/255) > oMaxMatch)
            {
                /* Match description too long : reduce it */
                matchLength = (15-1) + (oMaxMatch-op) * 255;
            }
            //printf("offset %5i, matchLength%5i \n", (int)(ip-match), matchLength + MINMATCH);
            ip += MINMATCH + matchLength;

            if (matchLength>=ML_MASK)
            {
                *token += ML_MASK;
                matchLength -= ML_MASK;
                while (matchLength >= 255) { matchLength-=255; *op++ = 255; }
                *op++ = (BYTE)matchLength;
            }
            else *token += (BYTE)(matchLength);
        }

        anchor = ip;

        /* Test end of block */
        if (ip > mflimit) break;
        if (op > oMaxSeq) break;

        /* Fill table */
        LZ3__putPosition(ip-2, ctx, tableType, base);

        /* Test next position */
        match = LZ3__getPosition(ip, ctx, tableType, base);
        LZ3__putPosition(ip, ctx, tableType, base);
        if ( (match+MAX_DISTANCE>=ip)
            && (LZ3__read32(match)==LZ3__read32(ip)) )
        { token=op++; *token=0; goto _next_match; }

        /* Prepare next loop */
        forwardH = LZ3__hashPosition(++ip, tableType);
    }

_last_literals:
    /* Encode Last Literals */
    {
        size_t lastRunSize = (size_t)(iend - anchor);
        if (op + 1 /* token */ + ((lastRunSize+240)/255) /* litLength */ + lastRunSize /* literals */ > oend)
        {
            /* adapt lastRunSize to fill 'dst' */
            lastRunSize  = (oend-op) - 1;
            lastRunSize -= (lastRunSize+240)/255;
        }
        ip = anchor + lastRunSize;

        if (lastRunSize >= RUN_MASK)
        {
            size_t accumulator = lastRunSize - RUN_MASK;
            *op++ = RUN_MASK << ML_BITS;
            for(; accumulator >= 255 ; accumulator-=255) *op++ = 255;
            *op++ = (BYTE) accumulator;
        }
        else
        {
            *op++ = (BYTE)(lastRunSize<<ML_BITS);
        }
        memcpy(op, anchor, lastRunSize);
        op += lastRunSize;
    }

    /* End */
    *srcSizePtr = (int) (((const char*)ip)-src);
    return (int) (((char*)op)-dst);
}


static int LZ3__compress_destSize_extState (void* state, const char* src, char* dst, int* srcSizePtr, int targetDstSize)
{
    LZ3__resetStream((LZ3__stream_t*)state);

    if (targetDstSize >= LZ3__compressBound(*srcSizePtr))   /* compression success is guaranteed */
    {
        return LZ3__compress_fast_extState(state, src, dst, *srcSizePtr, targetDstSize, 1);
    }
    else
    {
        if (*srcSizePtr < LZ3__64Klimit)
            return LZ3__compress_destSize_generic(state, src, dst, srcSizePtr, targetDstSize, byU16);
        else
            return LZ3__compress_destSize_generic(state, src, dst, srcSizePtr, targetDstSize, LZ3__64bits() ? byU32 : byPtr);
    }
}


int LZ3__compress_destSize(const char* src, char* dst, int* srcSizePtr, int targetDstSize)
{
#if (HEAPMODE)
    void* ctx = ALLOCATOR(1, sizeof(LZ3__stream_t));   /* malloc-calloc always properly aligned */
#else
    LZ3__stream_t ctxBody;
    void* ctx = &ctxBody;
#endif

    int result = LZ3__compress_destSize_extState(ctx, src, dst, srcSizePtr, targetDstSize);

#if (HEAPMODE)
    FREEMEM(ctx);
#endif
    return result;
}



/********************************
*  Streaming functions
********************************/

LZ3__stream_t* LZ3__createStream(void)
{
    LZ3__stream_t* lz3s = (LZ3__stream_t*)ALLOCATOR(8, LZ3__STREAMSIZE_U64);
    LZ3__STATIC_ASSERT(LZ3__STREAMSIZE >= sizeof(LZ3__stream_t_internal));    /* A compilation error here means LZ3__STREAMSIZE is not large enough */
    LZ3__resetStream(lz3s);
    return lz3s;
}

void LZ3__resetStream (LZ3__stream_t* LZ3__stream)
{
    MEM_INIT(LZ3__stream, 0, sizeof(LZ3__stream_t));
}

int LZ3__freeStream (LZ3__stream_t* LZ3__stream)
{
    FREEMEM(LZ3__stream);
    return (0);
}


#define HASH_UNIT sizeof(size_t)
int LZ3__loadDict (LZ3__stream_t* LZ3__dict, const char* dictionary, int dictSize)
{
    LZ3__stream_t_internal* dict = (LZ3__stream_t_internal*) LZ3__dict;
    const BYTE* p = (const BYTE*)dictionary;
    const BYTE* const dictEnd = p + dictSize;
    const BYTE* base;

    if ((dict->initCheck) || (dict->currentOffset > 1 GB))  /* Uninitialized structure, or reuse overflow */
        LZ3__resetStream(LZ3__dict);

    if (dictSize < (int)HASH_UNIT)
    {
        dict->dictionary = NULL;
        dict->dictSize = 0;
        return 0;
    }

    if ((dictEnd - p) > 64 KB) p = dictEnd - 64 KB;
    dict->currentOffset += 64 KB;
    base = p - dict->currentOffset;
    dict->dictionary = p;
    dict->dictSize = (U32)(dictEnd - p);
    dict->currentOffset += dict->dictSize;

    while (p <= dictEnd-HASH_UNIT)
    {
        LZ3__putPosition(p, dict->hashTable, byU32, base);
        p+=3;
    }

    return dict->dictSize;
}


static void LZ3__renormDictT(LZ3__stream_t_internal* LZ3__dict, const BYTE* src)
{
    if ((LZ3__dict->currentOffset > 0x80000000) ||
        ((size_t)LZ3__dict->currentOffset > (size_t)src))   /* address space overflow */
    {
        /* rescale hash table */
        U32 delta = LZ3__dict->currentOffset - 64 KB;
        const BYTE* dictEnd = LZ3__dict->dictionary + LZ3__dict->dictSize;
        int i;
        for (i=0; i<HASH_SIZE_U32; i++)
        {
            if (LZ3__dict->hashTable[i] < delta) LZ3__dict->hashTable[i]=0;
            else LZ3__dict->hashTable[i] -= delta;
        }
        LZ3__dict->currentOffset = 64 KB;
        if (LZ3__dict->dictSize > 64 KB) LZ3__dict->dictSize = 64 KB;
        LZ3__dict->dictionary = dictEnd - LZ3__dict->dictSize;
    }
}


int LZ3__compress_fast_continue (LZ3__stream_t* LZ3__stream, const char* source, char* dest, int inputSize, int maxOutputSize, int acceleration)
{
    LZ3__stream_t_internal* streamPtr = (LZ3__stream_t_internal*)LZ3__stream;
    const BYTE* const dictEnd = streamPtr->dictionary + streamPtr->dictSize;

    const BYTE* smallest = (const BYTE*) source;
    if (streamPtr->initCheck) return 0;   /* Uninitialized structure detected */
    if ((streamPtr->dictSize>0) && (smallest>dictEnd)) smallest = dictEnd;
    LZ3__renormDictT(streamPtr, smallest);
    if (acceleration < 1) acceleration = ACCELERATION_DEFAULT;

    /* Check overlapping input/dictionary space */
    {
        const BYTE* sourceEnd = (const BYTE*) source + inputSize;
        if ((sourceEnd > streamPtr->dictionary) && (sourceEnd < dictEnd))
        {
            streamPtr->dictSize = (U32)(dictEnd - sourceEnd);
            if (streamPtr->dictSize > 64 KB) streamPtr->dictSize = 64 KB;
            if (streamPtr->dictSize < 4) streamPtr->dictSize = 0;
            streamPtr->dictionary = dictEnd - streamPtr->dictSize;
        }
    }

    /* prefix mode : source data follows dictionary */
    if (dictEnd == (const BYTE*)source)
    {
        int result;
        if ((streamPtr->dictSize < 64 KB) && (streamPtr->dictSize < streamPtr->currentOffset))
            result = LZ3__compress_generic(LZ3__stream, source, dest, inputSize, maxOutputSize, limitedOutput, byU32, withPrefix64k, dictSmall, acceleration);
        else
            result = LZ3__compress_generic(LZ3__stream, source, dest, inputSize, maxOutputSize, limitedOutput, byU32, withPrefix64k, noDictIssue, acceleration);
        streamPtr->dictSize += (U32)inputSize;
        streamPtr->currentOffset += (U32)inputSize;
        return result;
    }

    /* external dictionary mode */
    {
        int result;
        if ((streamPtr->dictSize < 64 KB) && (streamPtr->dictSize < streamPtr->currentOffset))
            result = LZ3__compress_generic(LZ3__stream, source, dest, inputSize, maxOutputSize, limitedOutput, byU32, usingExtDict, dictSmall, acceleration);
        else
            result = LZ3__compress_generic(LZ3__stream, source, dest, inputSize, maxOutputSize, limitedOutput, byU32, usingExtDict, noDictIssue, acceleration);
        streamPtr->dictionary = (const BYTE*)source;
        streamPtr->dictSize = (U32)inputSize;
        streamPtr->currentOffset += (U32)inputSize;
        return result;
    }
}


/* Hidden debug function, to force external dictionary mode */
int LZ3__compress_forceExtDict (LZ3__stream_t* LZ3__dict, const char* source, char* dest, int inputSize)
{
    LZ3__stream_t_internal* streamPtr = (LZ3__stream_t_internal*)LZ3__dict;
    int result;
    const BYTE* const dictEnd = streamPtr->dictionary + streamPtr->dictSize;

    const BYTE* smallest = dictEnd;
    if (smallest > (const BYTE*) source) smallest = (const BYTE*) source;
    LZ3__renormDictT((LZ3__stream_t_internal*)LZ3__dict, smallest);

    result = LZ3__compress_generic(LZ3__dict, source, dest, inputSize, 0, notLimited, byU32, usingExtDict, noDictIssue, 1);

    streamPtr->dictionary = (const BYTE*)source;
    streamPtr->dictSize = (U32)inputSize;
    streamPtr->currentOffset += (U32)inputSize;

    return result;
}


int LZ3__saveDict (LZ3__stream_t* LZ3__dict, char* safeBuffer, int dictSize)
{
    LZ3__stream_t_internal* dict = (LZ3__stream_t_internal*) LZ3__dict;
    const BYTE* previousDictEnd = dict->dictionary + dict->dictSize;

    if ((U32)dictSize > 64 KB) dictSize = 64 KB;   /* useless to define a dictionary > 64 KB */
    if ((U32)dictSize > dict->dictSize) dictSize = dict->dictSize;

    memmove(safeBuffer, previousDictEnd - dictSize, dictSize);

    dict->dictionary = (const BYTE*)safeBuffer;
    dict->dictSize = (U32)dictSize;

    return dictSize;
}



/*******************************
*  Decompression functions
*******************************/
/*
 * This generic decompression function cover all use cases.
 * It shall be instantiated several times, using different sets of directives
 * Note that it is essential this generic function is really inlined,
 * in order to remove useless branches during compilation optimization.
 */
FORCE_INLINE int LZ3__decompress_generic(
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


int LZ3__decompress_safe(const char* source, char* dest, int compressedSize, int maxDecompressedSize)
{
    return LZ3__decompress_generic(source, dest, compressedSize, maxDecompressedSize, endOnInputSize, full, 0, noDict, (BYTE*)dest, NULL, 0);
}

int LZ3__decompress_safe_partial(const char* source, char* dest, int compressedSize, int targetOutputSize, int maxDecompressedSize)
{
    return LZ3__decompress_generic(source, dest, compressedSize, maxDecompressedSize, endOnInputSize, partial, targetOutputSize, noDict, (BYTE*)dest, NULL, 0);
}

int LZ3__decompress_fast(const char* source, char* dest, int originalSize)
{
    return LZ3__decompress_generic(source, dest, 0, originalSize, endOnOutputSize, full, 0, withPrefix64k, (BYTE*)(dest - 64 KB), NULL, 64 KB);
}


/* streaming decompression functions */

typedef struct
{
    const BYTE* externalDict;
    size_t extDictSize;
    const BYTE* prefixEnd;
    size_t prefixSize;
} LZ3__streamDecode_t_internal;

/*
 * If you prefer dynamic allocation methods,
 * LZ3__createStreamDecode()
 * provides a pointer (void*) towards an initialized LZ3__streamDecode_t structure.
 */
LZ3__streamDecode_t* LZ3__createStreamDecode(void)
{
    LZ3__streamDecode_t* lz3s = (LZ3__streamDecode_t*) ALLOCATOR(1, sizeof(LZ3__streamDecode_t));
    return lz3s;
}

int LZ3__freeStreamDecode (LZ3__streamDecode_t* LZ3__stream)
{
    FREEMEM(LZ3__stream);
    return 0;
}

/*
 * LZ3__setStreamDecode
 * Use this function to instruct where to find the dictionary
 * This function is not necessary if previous data is still available where it was decoded.
 * Loading a size of 0 is allowed (same effect as no dictionary).
 * Return : 1 if OK, 0 if error
 */
int LZ3__setStreamDecode (LZ3__streamDecode_t* LZ3__streamDecode, const char* dictionary, int dictSize)
{
    LZ3__streamDecode_t_internal* lz3sd = (LZ3__streamDecode_t_internal*) LZ3__streamDecode;
    lz3sd->prefixSize = (size_t) dictSize;
    lz3sd->prefixEnd = (const BYTE*) dictionary + dictSize;
    lz3sd->externalDict = NULL;
    lz3sd->extDictSize  = 0;
    return 1;
}

/*
*_continue() :
    These decoding functions allow decompression of multiple blocks in "streaming" mode.
    Previously decoded blocks must still be available at the memory position where they were decoded.
    If it's not possible, save the relevant part of decoded data into a safe buffer,
    and indicate where it stands using LZ3__setStreamDecode()
*/
int LZ3__decompress_safe_continue (LZ3__streamDecode_t* LZ3__streamDecode, const char* source, char* dest, int compressedSize, int maxOutputSize)
{
    LZ3__streamDecode_t_internal* lz3sd = (LZ3__streamDecode_t_internal*) LZ3__streamDecode;
    int result;

    if (lz3sd->prefixEnd == (BYTE*)dest)
    {
        result = LZ3__decompress_generic(source, dest, compressedSize, maxOutputSize,
                                        endOnInputSize, full, 0,
                                        usingExtDict, lz3sd->prefixEnd - lz3sd->prefixSize, lz3sd->externalDict, lz3sd->extDictSize);
        if (result <= 0) return result;
        lz3sd->prefixSize += result;
        lz3sd->prefixEnd  += result;
    }
    else
    {
        lz3sd->extDictSize = lz3sd->prefixSize;
        lz3sd->externalDict = lz3sd->prefixEnd - lz3sd->extDictSize;
        result = LZ3__decompress_generic(source, dest, compressedSize, maxOutputSize,
                                        endOnInputSize, full, 0,
                                        usingExtDict, (BYTE*)dest, lz3sd->externalDict, lz3sd->extDictSize);
        if (result <= 0) return result;
        lz3sd->prefixSize = result;
        lz3sd->prefixEnd  = (BYTE*)dest + result;
    }

    return result;
}

int LZ3__decompress_fast_continue (LZ3__streamDecode_t* LZ3__streamDecode, const char* source, char* dest, int originalSize)
{
    LZ3__streamDecode_t_internal* lz3sd = (LZ3__streamDecode_t_internal*) LZ3__streamDecode;
    int result;

    if (lz3sd->prefixEnd == (BYTE*)dest)
    {
        result = LZ3__decompress_generic(source, dest, 0, originalSize,
                                        endOnOutputSize, full, 0,
                                        usingExtDict, lz3sd->prefixEnd - lz3sd->prefixSize, lz3sd->externalDict, lz3sd->extDictSize);
        if (result <= 0) return result;
        lz3sd->prefixSize += originalSize;
        lz3sd->prefixEnd  += originalSize;
    }
    else
    {
        lz3sd->extDictSize = lz3sd->prefixSize;
        lz3sd->externalDict = (BYTE*)dest - lz3sd->extDictSize;
        result = LZ3__decompress_generic(source, dest, 0, originalSize,
                                        endOnOutputSize, full, 0,
                                        usingExtDict, (BYTE*)dest, lz3sd->externalDict, lz3sd->extDictSize);
        if (result <= 0) return result;
        lz3sd->prefixSize = originalSize;
        lz3sd->prefixEnd  = (BYTE*)dest + originalSize;
    }

    return result;
}


/*
Advanced decoding functions :
*_usingDict() :
    These decoding functions work the same as "_continue" ones,
    the dictionary must be explicitly provided within parameters
*/

FORCE_INLINE int LZ3__decompress_usingDict_generic(const char* source, char* dest, int compressedSize, int maxOutputSize, int safe, const char* dictStart, int dictSize)
{
    if (dictSize==0)
        return LZ3__decompress_generic(source, dest, compressedSize, maxOutputSize, safe, full, 0, noDict, (BYTE*)dest, NULL, 0);
    if (dictStart+dictSize == dest)
    {
        if (dictSize >= (int)(64 KB - 1))
            return LZ3__decompress_generic(source, dest, compressedSize, maxOutputSize, safe, full, 0, withPrefix64k, (BYTE*)dest-64 KB, NULL, 0);
        return LZ3__decompress_generic(source, dest, compressedSize, maxOutputSize, safe, full, 0, noDict, (BYTE*)dest-dictSize, NULL, 0);
    }
    return LZ3__decompress_generic(source, dest, compressedSize, maxOutputSize, safe, full, 0, usingExtDict, (BYTE*)dest, (const BYTE*)dictStart, dictSize);
}

int LZ3__decompress_safe_usingDict(const char* source, char* dest, int compressedSize, int maxOutputSize, const char* dictStart, int dictSize)
{
    return LZ3__decompress_usingDict_generic(source, dest, compressedSize, maxOutputSize, 1, dictStart, dictSize);
}

int LZ3__decompress_fast_usingDict(const char* source, char* dest, int originalSize, const char* dictStart, int dictSize)
{
    return LZ3__decompress_usingDict_generic(source, dest, 0, originalSize, 0, dictStart, dictSize);
}

/* debug function */
int LZ3__decompress_safe_forceExtDict(const char* source, char* dest, int compressedSize, int maxOutputSize, const char* dictStart, int dictSize)
{
    return LZ3__decompress_generic(source, dest, compressedSize, maxOutputSize, endOnInputSize, full, 0, usingExtDict, (BYTE*)dest, (const BYTE*)dictStart, dictSize);
}


/***************************************************
*  Obsolete Functions
***************************************************/
/* obsolete compression functions */
int LZ3__compress_limitedOutput(const char* source, char* dest, int inputSize, int maxOutputSize) { return LZ3__compress_default(source, dest, inputSize, maxOutputSize); }
int LZ3__compress(const char* source, char* dest, int inputSize) { return LZ3__compress_default(source, dest, inputSize, LZ3__compressBound(inputSize)); }
int LZ3__compress_limitedOutput_withState (void* state, const char* src, char* dst, int srcSize, int dstSize) { return LZ3__compress_fast_extState(state, src, dst, srcSize, dstSize, 1); }
int LZ3__compress_withState (void* state, const char* src, char* dst, int srcSize) { return LZ3__compress_fast_extState(state, src, dst, srcSize, LZ3__compressBound(srcSize), 1); }
int LZ3__compress_limitedOutput_continue (LZ3__stream_t* LZ3__stream, const char* src, char* dst, int srcSize, int maxDstSize) { return LZ3__compress_fast_continue(LZ3__stream, src, dst, srcSize, maxDstSize, 1); }
int LZ3__compress_continue (LZ3__stream_t* LZ3__stream, const char* source, char* dest, int inputSize) { return LZ3__compress_fast_continue(LZ3__stream, source, dest, inputSize, LZ3__compressBound(inputSize), 1); }

/*
These function names are deprecated and should no longer be used.
They are only provided here for compatibility with older user programs.
- LZ3__uncompress is totally equivalent to LZ3__decompress_fast
- LZ3__uncompress_unknownOutputSize is totally equivalent to LZ3__decompress_safe
*/
int LZ3__uncompress (const char* source, char* dest, int outputSize) { return LZ3__decompress_fast(source, dest, outputSize); }
int LZ3__uncompress_unknownOutputSize (const char* source, char* dest, int isize, int maxOutputSize) { return LZ3__decompress_safe(source, dest, isize, maxOutputSize); }


/* Obsolete Streaming functions */

int LZ3__sizeofStreamState() { return LZ3__STREAMSIZE; }

static void LZ3__init(LZ3__stream_t_internal* lz3ds, BYTE* base)
{
    MEM_INIT(lz3ds, 0, LZ3__STREAMSIZE);
    lz3ds->bufferStart = base;
}

int LZ3__resetStreamState(void* state, char* inputBuffer)
{
    if ((((size_t)state) & 3) != 0) return 1;   /* Error : pointer is not aligned on 4-bytes boundary */
    LZ3__init((LZ3__stream_t_internal*)state, (BYTE*)inputBuffer);
    return 0;
}

void* LZ3__create (char* inputBuffer)
{
    void* lz3ds = ALLOCATOR(8, LZ3__STREAMSIZE_U64);
    LZ3__init ((LZ3__stream_t_internal*)lz3ds, (BYTE*)inputBuffer);
    return lz3ds;
}

char* LZ3__slideInputBuffer (void* LZ3__Data)
{
    LZ3__stream_t_internal* ctx = (LZ3__stream_t_internal*)LZ3__Data;
    int dictSize = LZ3__saveDict((LZ3__stream_t*)LZ3__Data, (char*)ctx->bufferStart, 64 KB);
    return (char*)(ctx->bufferStart + dictSize);
}

/* Obsolete streaming decompression functions */

int LZ3__decompress_safe_withPrefix64k(const char* source, char* dest, int compressedSize, int maxOutputSize)
{
    return LZ3__decompress_generic(source, dest, compressedSize, maxOutputSize, endOnInputSize, full, 0, withPrefix64k, (BYTE*)dest - 64 KB, NULL, 64 KB);
}

int LZ3__decompress_fast_withPrefix64k(const char* source, char* dest, int originalSize)
{
    return LZ3__decompress_generic(source, dest, 0, originalSize, endOnOutputSize, full, 0, withPrefix64k, (BYTE*)dest - 64 KB, NULL, 64 KB);
}

#endif   /* LZ3__COMMONDEFS_ONLY */

