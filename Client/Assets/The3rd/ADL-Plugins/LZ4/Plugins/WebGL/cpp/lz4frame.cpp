
/**************************************
*  Compiler Options
**************************************/
#ifdef _MSC_VER    /* Visual Studio */
#  pragma warning(disable : 4127)        /* disable: C4127: conditional expression is constant */
#endif


/**************************************
*  Memory routines
**************************************/
#include <stdlib.h>   /* malloc, calloc, free */
#define ALLOCATOR(s)   calloc(1,s)
#define FREEMEM        free
#include <string.h>   /* memset, memcpy, memmove */
#define MEM_INIT       memset


/**************************************
*  Includes
**************************************/
#include "lz4frame_static.h"
#include "lz4.h"
#include "lz4hc.h"
#include "xxhash.h"


/**************************************
*  Basic Types
**************************************/
#if defined(__STDC_VERSION__) && (__STDC_VERSION__ >= 199901L)   /* C99 */
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
*  Constants
**************************************/
#define KB *(1<<10)
#define MB *(1<<20)
#define GB *(1<<30)

#define _1BIT  0x01
#define _2BITS 0x03
#define _3BITS 0x07
#define _4BITS 0x0F
#define _8BITS 0xFF

#define LZ3F_MAGIC_SKIPPABLE_START 0x184D2A50U
#define LZ3F_MAGICNUMBER 0x184D2204U
#define LZ3F_BLOCKUNCOMPRESSED_FLAG 0x80000000U
#define LZ3F_BLOCKSIZEID_DEFAULT LZ3F_max64KB

static const size_t minFHSize = 7;
static const size_t maxFHSize = 15;
static const size_t BHSize = 4;
static const int    minHClevel = 3;


/**************************************
*  Structures and local types
**************************************/
typedef struct LZ3F_cctx_s
{
    LZ3F_preferences_t prefs;
    U32    version;
    U32    cStage;
    size_t maxBlockSize;
    size_t maxBufferSize;
    BYTE*  tmpBuff;
    BYTE*  tmpIn;
    size_t tmpInSize;
    U64    totalInSize;
    XX_H32_state_t xxh;
    void*  lz3CtxPtr;
    U32    lz3CtxLevel;     /* 0: unallocated;  1: LZ3__stream_t;  3: LZ3__streamHC_t */
} LZ3F_cctx_t;

typedef struct LZ3F_dctx_s
{
    LZ3F_frameInfo_t frameInfo;
    U32    version;
    U32    dStage;
    U64    frameRemainingSize;
    size_t maxBlockSize;
    size_t maxBufferSize;
    const BYTE* srcExpect;
    BYTE*  tmpIn;
    size_t tmpInSize;
    size_t tmpInTarget;
    BYTE*  tmpOutBuffer;
    const BYTE*  dict;
    size_t dictSize;
    BYTE*  tmpOut;
    size_t tmpOutSize;
    size_t tmpOutStart;
    XX_H32_state_t xxh;
    BYTE   header[16];
} LZ3F_dctx_t;


/**************************************
*  Error management
**************************************/
#define LZ3F_GENERATE_STRING(STRING) #STRING,
static const char* LZ3F_errorStrings[] = { LZ3F_LIST_ERRORS(LZ3F_GENERATE_STRING) };



unsigned LZ3F_isError(LZ3F_errorCode_t code)
{
    return (code > (LZ3F_errorCode_t)(-LZ3F_ERROR_maxCode));
}

const char* LZ3F_getErrorName(LZ3F_errorCode_t code)
{
    static const char* codeError = "Unspecified error code";
    if (LZ3F_isError(code)) return LZ3F_errorStrings[-(int)(code)];
    return codeError;
}


/**************************************
*  Private functions
**************************************/
static size_t LZ3F_getBlockSize(unsigned blockSizeID)
{
    static const size_t blockSizes[4] = { 64 KB, 256 KB, 1 MB, 4 MB };

    if (blockSizeID == 0) blockSizeID = LZ3F_BLOCKSIZEID_DEFAULT;
    blockSizeID -= 4;
    if (blockSizeID > 3) return (size_t)-LZ3F_ERROR_maxBlockSize_invalid;
    return blockSizes[blockSizeID];
}


/* unoptimized version; solves endianess & alignment issues */
static U32 LZ3F_readLE32 (const BYTE* srcPtr)
{
    U32 value32 = srcPtr[0];
    value32 += (srcPtr[1]<<8);
    value32 += (srcPtr[2]<<16);
    value32 += ((U32)srcPtr[3])<<24;
    return value32;
}

static void LZ3F_writeLE32 (BYTE* dstPtr, U32 value32)
{
    dstPtr[0] = (BYTE)value32;
    dstPtr[1] = (BYTE)(value32 >> 8);
    dstPtr[2] = (BYTE)(value32 >> 16);
    dstPtr[3] = (BYTE)(value32 >> 24);
}

static U64 LZ3F_readLE64 (const BYTE* srcPtr)
{
    U64 value64 = srcPtr[0];
    value64 += ((U64)srcPtr[1]<<8);
    value64 += ((U64)srcPtr[2]<<16);
    value64 += ((U64)srcPtr[3]<<24);
    value64 += ((U64)srcPtr[4]<<32);
    value64 += ((U64)srcPtr[5]<<40);
    value64 += ((U64)srcPtr[6]<<48);
    value64 += ((U64)srcPtr[7]<<56);
    return value64;
}

static void LZ3F_writeLE64 (BYTE* dstPtr, U64 value64)
{
    dstPtr[0] = (BYTE)value64;
    dstPtr[1] = (BYTE)(value64 >> 8);
    dstPtr[2] = (BYTE)(value64 >> 16);
    dstPtr[3] = (BYTE)(value64 >> 24);
    dstPtr[4] = (BYTE)(value64 >> 32);
    dstPtr[5] = (BYTE)(value64 >> 40);
    dstPtr[6] = (BYTE)(value64 >> 48);
    dstPtr[7] = (BYTE)(value64 >> 56);
}


static BYTE LZ3F_headerChecksum (const void* header, size_t length)
{
    U32 xxh = XX_H32(header, length, 0);
    return (BYTE)(xxh >> 8);
}


/**************************************
*  Simple compression functions
**************************************/
static LZ3F_blockSizeID_t LZ3F_optimalBSID(const LZ3F_blockSizeID_t requestedBSID, const size_t srcSize)
{
    LZ3F_blockSizeID_t proposedBSID = LZ3F_max64KB;
    size_t maxBlockSize = 64 KB;
    while (requestedBSID > proposedBSID)
    {
        if (srcSize <= maxBlockSize)
            return proposedBSID;
        proposedBSID = (LZ3F_blockSizeID_t)((int)proposedBSID + 1);
        maxBlockSize <<= 2;
    }
    return requestedBSID;
}


size_t LZ3F_compressFrameBound(size_t srcSize, const LZ3F_preferences_t* preferencesPtr)
{
    LZ3F_preferences_t prefs;
    size_t headerSize;
    size_t streamSize;

    if (preferencesPtr!=NULL) prefs = *preferencesPtr;
    else memset(&prefs, 0, sizeof(prefs));

    prefs.frameInfo.blockSizeID = LZ3F_optimalBSID(prefs.frameInfo.blockSizeID, srcSize);
    prefs.autoFlush = 1;

    headerSize = maxFHSize;      /* header size, including magic number and frame content size*/
    streamSize = LZ3F_compressBound(srcSize, &prefs);

    return headerSize + streamSize;
}


/* LZ3F_compressFrame()
* Compress an entire srcBuffer into a valid LZ3 frame, as defined by specification v1.5.0, in a single step.
* The most important rule is that dstBuffer MUST be large enough (dstMaxSize) to ensure compression completion even in worst case.
* You can get the minimum value of dstMaxSize by using LZ3F_compressFrameBound()
* If this condition is not respected, LZ3F_compressFrame() will fail (result is an errorCode)
* The LZ3F_preferences_t structure is optional : you can provide NULL as argument. All preferences will then be set to default.
* The result of the function is the number of bytes written into dstBuffer.
* The function outputs an error code if it fails (can be tested using LZ3F_isError())
*/
size_t LZ3F_compressFrame(void* dstBuffer, size_t dstMaxSize, const void* srcBuffer, size_t srcSize, const LZ3F_preferences_t* preferencesPtr)
{
    LZ3F_cctx_t cctxI;
    LZ3__stream_t lz3ctx;
    LZ3F_preferences_t prefs;
    LZ3F_compressOptions_t options;
    LZ3F_errorCode_t errorCode;
    BYTE* const dstStart = (BYTE*) dstBuffer;
    BYTE* dstPtr = dstStart;
    BYTE* const dstEnd = dstStart + dstMaxSize;

    memset(&cctxI, 0, sizeof(cctxI));   /* works because no allocation */
    memset(&options, 0, sizeof(options));

    cctxI.version = LZ3F_VERSION;
    cctxI.maxBufferSize = 5 MB;   /* mess with real buffer size to prevent allocation; works because autoflush==1 & stableSrc==1 */

    if (preferencesPtr!=NULL)
        prefs = *preferencesPtr;
    else
        memset(&prefs, 0, sizeof(prefs));
    if (prefs.frameInfo.contentSize != 0)
        prefs.frameInfo.contentSize = (U64)srcSize;   /* auto-correct content size if selected (!=0) */

    if (prefs.compressionLevel < (int)minHClevel)
    {
        cctxI.lz3CtxPtr = &lz3ctx;
        cctxI.lz3CtxLevel = 1;
    }

    prefs.frameInfo.blockSizeID = LZ3F_optimalBSID(prefs.frameInfo.blockSizeID, srcSize);
    prefs.autoFlush = 1;
    if (srcSize <= LZ3F_getBlockSize(prefs.frameInfo.blockSizeID))
        prefs.frameInfo.blockMode = LZ3F_blockIndependent;   /* no need for linked blocks */

    options.stableSrc = 1;

    if (dstMaxSize < LZ3F_compressFrameBound(srcSize, &prefs))
        return (size_t)-LZ3F_ERROR_dstMaxSize_tooSmall;

    errorCode = LZ3F_compressBegin(&cctxI, dstBuffer, dstMaxSize, &prefs);  /* write header */
    if (LZ3F_isError(errorCode)) return errorCode;
    dstPtr += errorCode;   /* header size */

    errorCode = LZ3F_compressUpdate(&cctxI, dstPtr, dstEnd-dstPtr, srcBuffer, srcSize, &options);
    if (LZ3F_isError(errorCode)) return errorCode;
    dstPtr += errorCode;

    errorCode = LZ3F_compressEnd(&cctxI, dstPtr, dstEnd-dstPtr, &options);   /* flush last block, and generate suffix */
    if (LZ3F_isError(errorCode)) return errorCode;
    dstPtr += errorCode;

    if (prefs.compressionLevel >= (int)minHClevel)   /* no allocation necessary with lz3 fast */
        FREEMEM(cctxI.lz3CtxPtr);

    return (dstPtr - dstStart);
}


/***********************************
*  Advanced compression functions
***********************************/

/* LZ3F_createCompressionContext() :
* The first thing to do is to create a compressionContext object, which will be used in all compression operations.
* This is achieved using LZ3F_createCompressionContext(), which takes as argument a version and an LZ3F_preferences_t structure.
* The version provided MUST be LZ3F_VERSION. It is intended to track potential version differences between different binaries.
* The function will provide a pointer to an allocated LZ3F_compressionContext_t object.
* If the result LZ3F_errorCode_t is not OK_NoError, there was an error during context creation.
* Object can release its memory using LZ3F_freeCompressionContext();
*/
LZ3F_errorCode_t LZ3F_createCompressionContext(LZ3F_compressionContext_t* LZ3F_compressionContextPtr, unsigned version)
{
    LZ3F_cctx_t* cctxPtr;

    cctxPtr = (LZ3F_cctx_t*)ALLOCATOR(sizeof(LZ3F_cctx_t));
    if (cctxPtr==NULL) return (LZ3F_errorCode_t)(-LZ3F_ERROR_allocation_failed);

    cctxPtr->version = version;
    cctxPtr->cStage = 0;   /* Next stage : write header */

    *LZ3F_compressionContextPtr = (LZ3F_compressionContext_t)cctxPtr;

    return LZ3F_OK_NoError;
}


LZ3F_errorCode_t LZ3F_freeCompressionContext(LZ3F_compressionContext_t LZ3F_compressionContext)
{
    LZ3F_cctx_t* cctxPtr = (LZ3F_cctx_t*)LZ3F_compressionContext;

    if (cctxPtr != NULL)   /* null pointers can be safely provided to this function, like free() */
    {
       FREEMEM(cctxPtr->lz3CtxPtr);
       FREEMEM(cctxPtr->tmpBuff);
       FREEMEM(LZ3F_compressionContext);
    }

    return LZ3F_OK_NoError;
}


/* LZ3F_compressBegin() :
* will write the frame header into dstBuffer.
* dstBuffer must be large enough to accommodate a header (dstMaxSize). Maximum header size is LZ3F_MAXHEADERFRAME_SIZE bytes.
* The result of the function is the number of bytes written into dstBuffer for the header
* or an error code (can be tested using LZ3F_isError())
*/
size_t LZ3F_compressBegin(LZ3F_compressionContext_t compressionContext, void* dstBuffer, size_t dstMaxSize, const LZ3F_preferences_t* preferencesPtr)
{
    LZ3F_preferences_t prefNull;
    LZ3F_cctx_t* cctxPtr = (LZ3F_cctx_t*)compressionContext;
    BYTE* const dstStart = (BYTE*)dstBuffer;
    BYTE* dstPtr = dstStart;
    BYTE* headerStart;
    size_t requiredBuffSize;

    if (dstMaxSize < maxFHSize) return (size_t)-LZ3F_ERROR_dstMaxSize_tooSmall;
    if (cctxPtr->cStage != 0) return (size_t)-LZ3F_ERROR_GENERIC;
    memset(&prefNull, 0, sizeof(prefNull));
    if (preferencesPtr == NULL) preferencesPtr = &prefNull;
    cctxPtr->prefs = *preferencesPtr;

    /* ctx Management */
    {
        U32 tableID = (cctxPtr->prefs.compressionLevel < minHClevel) ? 1 : 2;  /* 0:nothing ; 1:LZ3 table ; 2:HC tables */
        if (cctxPtr->lz3CtxLevel < tableID)
        {
            FREEMEM(cctxPtr->lz3CtxPtr);
            if (cctxPtr->prefs.compressionLevel < minHClevel)
                cctxPtr->lz3CtxPtr = (void*)LZ3__createStream();
            else
                cctxPtr->lz3CtxPtr = (void*)LZ3__createStreamHC();
            cctxPtr->lz3CtxLevel = tableID;
        }
    }

    /* Buffer Management */
    if (cctxPtr->prefs.frameInfo.blockSizeID == 0) cctxPtr->prefs.frameInfo.blockSizeID = LZ3F_BLOCKSIZEID_DEFAULT;
    cctxPtr->maxBlockSize = LZ3F_getBlockSize(cctxPtr->prefs.frameInfo.blockSizeID);

    requiredBuffSize = cctxPtr->maxBlockSize + ((cctxPtr->prefs.frameInfo.blockMode == LZ3F_blockLinked) * 128 KB);
    if (preferencesPtr->autoFlush)
        requiredBuffSize = (cctxPtr->prefs.frameInfo.blockMode == LZ3F_blockLinked) * 64 KB;   /* just needs dict */

    if (cctxPtr->maxBufferSize < requiredBuffSize)
    {
        cctxPtr->maxBufferSize = requiredBuffSize;
        FREEMEM(cctxPtr->tmpBuff);
        cctxPtr->tmpBuff = (BYTE*)ALLOCATOR(requiredBuffSize);
        if (cctxPtr->tmpBuff == NULL) return (size_t)-LZ3F_ERROR_allocation_failed;
    }
    cctxPtr->tmpIn = cctxPtr->tmpBuff;
    cctxPtr->tmpInSize = 0;
    XX_H32_reset(&(cctxPtr->xxh), 0);
    if (cctxPtr->prefs.compressionLevel < minHClevel)
        LZ3__resetStream((LZ3__stream_t*)(cctxPtr->lz3CtxPtr));
    else
        LZ3__resetStreamHC((LZ3__streamHC_t*)(cctxPtr->lz3CtxPtr), cctxPtr->prefs.compressionLevel);

    /* Magic Number */
    LZ3F_writeLE32(dstPtr, LZ3F_MAGICNUMBER);
    dstPtr += 4;
    headerStart = dstPtr;

    /* FLG Byte */
    *dstPtr++ = (BYTE)(((1 & _2BITS) << 6)    /* Version('01') */
        + ((cctxPtr->prefs.frameInfo.blockMode & _1BIT ) << 5)    /* Block mode */
        + ((cctxPtr->prefs.frameInfo.contentChecksumFlag & _1BIT ) << 2)   /* Frame checksum */
        + ((cctxPtr->prefs.frameInfo.contentSize > 0) << 3));   /* Frame content size */
    /* BD Byte */
    *dstPtr++ = (BYTE)((cctxPtr->prefs.frameInfo.blockSizeID & _3BITS) << 4);
    /* Optional Frame content size field */
    if (cctxPtr->prefs.frameInfo.contentSize)
    {
        LZ3F_writeLE64(dstPtr, cctxPtr->prefs.frameInfo.contentSize);
        dstPtr += 8;
        cctxPtr->totalInSize = 0;
    }
    /* CRC Byte */
    *dstPtr = LZ3F_headerChecksum(headerStart, dstPtr - headerStart);
    dstPtr++;

    cctxPtr->cStage = 1;   /* header written, now request input data block */

    return (dstPtr - dstStart);
}


/* LZ3F_compressBound() : gives the size of Dst buffer given a srcSize to handle worst case situations.
*                        The LZ3F_frameInfo_t structure is optional :
*                        you can provide NULL as argument, preferences will then be set to cover worst case situations.
* */
size_t LZ3F_compressBound(size_t srcSize, const LZ3F_preferences_t* preferencesPtr)
{
    LZ3F_preferences_t prefsNull;
    memset(&prefsNull, 0, sizeof(prefsNull));
    prefsNull.frameInfo.contentChecksumFlag = LZ3F_contentChecksumEnabled;   /* worst case */
    {
        const LZ3F_preferences_t* prefsPtr = (preferencesPtr==NULL) ? &prefsNull : preferencesPtr;
        LZ3F_blockSizeID_t bid = prefsPtr->frameInfo.blockSizeID;
        size_t blockSize = LZ3F_getBlockSize(bid);
        unsigned nbBlocks = (unsigned)(srcSize / blockSize) + 1;
        size_t lastBlockSize = prefsPtr->autoFlush ? srcSize % blockSize : blockSize;
        size_t blockInfo = 4;   /* default, without block CRC option */
        size_t frameEnd = 4 + (prefsPtr->frameInfo.contentChecksumFlag*4);

        return (blockInfo * nbBlocks) + (blockSize * (nbBlocks-1)) + lastBlockSize + frameEnd;;
    }
}


typedef int (*compressFunc_t)(void* ctx, const char* src, char* dst, int srcSize, int dstSize, int level);

static size_t LZ3F_compressBlock(void* dst, const void* src, size_t srcSize, compressFunc_t compress, void* lz3ctx, int level)
{
    /* compress one block */
    BYTE* cSizePtr = (BYTE*)dst;
    U32 cSize;
    cSize = (U32)compress(lz3ctx, (const char*)src, (char*)(cSizePtr+4), (int)(srcSize), (int)(srcSize-1), level);
    LZ3F_writeLE32(cSizePtr, cSize);
    if (cSize == 0)   /* compression failed */
    {
        cSize = (U32)srcSize;
        LZ3F_writeLE32(cSizePtr, cSize + LZ3F_BLOCKUNCOMPRESSED_FLAG);
        memcpy(cSizePtr+4, src, srcSize);
    }
    return cSize + 4;
}


static int LZ3F_localLZ3__compress_limitedOutput_withState(void* ctx, const char* src, char* dst, int srcSize, int dstSize, int level)
{
    (void) level;
    return LZ3__compress_limitedOutput_withState(ctx, src, dst, srcSize, dstSize);
}

static int LZ3F_localLZ3__compress_limitedOutput_continue(void* ctx, const char* src, char* dst, int srcSize, int dstSize, int level)
{
    (void) level;
    return LZ3__compress_limitedOutput_continue((LZ3__stream_t*)ctx, src, dst, srcSize, dstSize);
}

static int LZ3F_localLZ3__compressHC_limitedOutput_continue(void* ctx, const char* src, char* dst, int srcSize, int dstSize, int level)
{
    (void) level;
    return LZ3__compress_HC_continue((LZ3__streamHC_t*)ctx, src, dst, srcSize, dstSize);
}

static compressFunc_t LZ3F_selectCompression(LZ3F_blockMode_t blockMode, int level)
{
    if (level < minHClevel)
    {
        if (blockMode == LZ3F_blockIndependent) return LZ3F_localLZ3__compress_limitedOutput_withState;
        return LZ3F_localLZ3__compress_limitedOutput_continue;
    }
    if (blockMode == LZ3F_blockIndependent) return LZ3__compress_HC_extStateHC;
    return LZ3F_localLZ3__compressHC_limitedOutput_continue;
}

static int LZ3F_localSaveDict(LZ3F_cctx_t* cctxPtr)
{
    if (cctxPtr->prefs.compressionLevel < minHClevel)
        return LZ3__saveDict ((LZ3__stream_t*)(cctxPtr->lz3CtxPtr), (char*)(cctxPtr->tmpBuff), 64 KB);
    return LZ3__saveDictHC ((LZ3__streamHC_t*)(cctxPtr->lz3CtxPtr), (char*)(cctxPtr->tmpBuff), 64 KB);
}

typedef enum { notDone, fromTmpBuffer, fromSrcBuffer } LZ3F_lastBlockStatus;

/* LZ3F_compressUpdate()
* LZ3F_compressUpdate() can be called repetitively to compress as much data as necessary.
* The most important rule is that dstBuffer MUST be large enough (dstMaxSize) to ensure compression completion even in worst case.
* If this condition is not respected, LZ3F_compress() will fail (result is an errorCode)
* You can get the minimum value of dstMaxSize by using LZ3F_compressBound()
* The LZ3F_compressOptions_t structure is optional : you can provide NULL as argument.
* The result of the function is the number of bytes written into dstBuffer : it can be zero, meaning input data was just buffered.
* The function outputs an error code if it fails (can be tested using LZ3F_isError())
*/
size_t LZ3F_compressUpdate(LZ3F_compressionContext_t compressionContext, void* dstBuffer, size_t dstMaxSize, const void* srcBuffer, size_t srcSize, const LZ3F_compressOptions_t* compressOptionsPtr)
{
    LZ3F_compressOptions_t cOptionsNull;
    LZ3F_cctx_t* cctxPtr = (LZ3F_cctx_t*)compressionContext;
    size_t blockSize = cctxPtr->maxBlockSize;
    const BYTE* srcPtr = (const BYTE*)srcBuffer;
    const BYTE* const srcEnd = srcPtr + srcSize;
    BYTE* const dstStart = (BYTE*)dstBuffer;
    BYTE* dstPtr = dstStart;
    LZ3F_lastBlockStatus lastBlockCompressed = notDone;
    compressFunc_t compress;


    if (cctxPtr->cStage != 1) return (size_t)-LZ3F_ERROR_GENERIC;
    if (dstMaxSize < LZ3F_compressBound(srcSize, &(cctxPtr->prefs))) return (size_t)-LZ3F_ERROR_dstMaxSize_tooSmall;
    memset(&cOptionsNull, 0, sizeof(cOptionsNull));
    if (compressOptionsPtr == NULL) compressOptionsPtr = &cOptionsNull;

    /* select compression function */
    compress = LZ3F_selectCompression(cctxPtr->prefs.frameInfo.blockMode, cctxPtr->prefs.compressionLevel);

    /* complete tmp buffer */
    if (cctxPtr->tmpInSize > 0)   /* some data already within tmp buffer */
    {
        size_t sizeToCopy = blockSize - cctxPtr->tmpInSize;
        if (sizeToCopy > srcSize)
        {
            /* add src to tmpIn buffer */
            memcpy(cctxPtr->tmpIn + cctxPtr->tmpInSize, srcBuffer, srcSize);
            srcPtr = srcEnd;
            cctxPtr->tmpInSize += srcSize;
            /* still needs some CRC */
        }
        else
        {
            /* complete tmpIn block and then compress it */
            lastBlockCompressed = fromTmpBuffer;
            memcpy(cctxPtr->tmpIn + cctxPtr->tmpInSize, srcBuffer, sizeToCopy);
            srcPtr += sizeToCopy;

            dstPtr += LZ3F_compressBlock(dstPtr, cctxPtr->tmpIn, blockSize, compress, cctxPtr->lz3CtxPtr, cctxPtr->prefs.compressionLevel);

            if (cctxPtr->prefs.frameInfo.blockMode==LZ3F_blockLinked) cctxPtr->tmpIn += blockSize;
            cctxPtr->tmpInSize = 0;
        }
    }

    while ((size_t)(srcEnd - srcPtr) >= blockSize)
    {
        /* compress full block */
        lastBlockCompressed = fromSrcBuffer;
        dstPtr += LZ3F_compressBlock(dstPtr, srcPtr, blockSize, compress, cctxPtr->lz3CtxPtr, cctxPtr->prefs.compressionLevel);
        srcPtr += blockSize;
    }

    if ((cctxPtr->prefs.autoFlush) && (srcPtr < srcEnd))
    {
        /* compress remaining input < blockSize */
        lastBlockCompressed = fromSrcBuffer;
        dstPtr += LZ3F_compressBlock(dstPtr, srcPtr, srcEnd - srcPtr, compress, cctxPtr->lz3CtxPtr, cctxPtr->prefs.compressionLevel);
        srcPtr  = srcEnd;
    }

    /* preserve dictionary if necessary */
    if ((cctxPtr->prefs.frameInfo.blockMode==LZ3F_blockLinked) && (lastBlockCompressed==fromSrcBuffer))
    {
        if (compressOptionsPtr->stableSrc)
        {
            cctxPtr->tmpIn = cctxPtr->tmpBuff;
        }
        else
        {
            int realDictSize = LZ3F_localSaveDict(cctxPtr);
            if (realDictSize==0) return (size_t)-LZ3F_ERROR_GENERIC;
            cctxPtr->tmpIn = cctxPtr->tmpBuff + realDictSize;
        }
    }

    /* keep tmpIn within limits */
    if ((cctxPtr->tmpIn + blockSize) > (cctxPtr->tmpBuff + cctxPtr->maxBufferSize)   /* necessarily LZ3F_blockLinked && lastBlockCompressed==fromTmpBuffer */
        && !(cctxPtr->prefs.autoFlush))
    {
        int realDictSize = LZ3F_localSaveDict(cctxPtr);
        cctxPtr->tmpIn = cctxPtr->tmpBuff + realDictSize;
    }

    /* some input data left, necessarily < blockSize */
    if (srcPtr < srcEnd)
    {
        /* fill tmp buffer */
        size_t sizeToCopy = srcEnd - srcPtr;
        memcpy(cctxPtr->tmpIn, srcPtr, sizeToCopy);
        cctxPtr->tmpInSize = sizeToCopy;
    }

    if (cctxPtr->prefs.frameInfo.contentChecksumFlag == LZ3F_contentChecksumEnabled)
        XX_H32_update(&(cctxPtr->xxh), srcBuffer, srcSize);

    cctxPtr->totalInSize += srcSize;
    return dstPtr - dstStart;
}


/* LZ3F_flush()
* Should you need to create compressed data immediately, without waiting for a block to be filled,
* you can call LZ3__flush(), which will immediately compress any remaining data stored within compressionContext.
* The result of the function is the number of bytes written into dstBuffer
* (it can be zero, this means there was no data left within compressionContext)
* The function outputs an error code if it fails (can be tested using LZ3F_isError())
* The LZ3F_compressOptions_t structure is optional : you can provide NULL as argument.
*/
size_t LZ3F_flush(LZ3F_compressionContext_t compressionContext, void* dstBuffer, size_t dstMaxSize, const LZ3F_compressOptions_t* compressOptionsPtr)
{
    LZ3F_cctx_t* cctxPtr = (LZ3F_cctx_t*)compressionContext;
    BYTE* const dstStart = (BYTE*)dstBuffer;
    BYTE* dstPtr = dstStart;
    compressFunc_t compress;


    if (cctxPtr->tmpInSize == 0) return 0;   /* nothing to flush */
    if (cctxPtr->cStage != 1) return (size_t)-LZ3F_ERROR_GENERIC;
    if (dstMaxSize < (cctxPtr->tmpInSize + 8)) return (size_t)-LZ3F_ERROR_dstMaxSize_tooSmall;   /* +8 : block header(4) + block checksum(4) */
    (void)compressOptionsPtr;   /* not yet useful */

    /* select compression function */
    compress = LZ3F_selectCompression(cctxPtr->prefs.frameInfo.blockMode, cctxPtr->prefs.compressionLevel);

    /* compress tmp buffer */
    dstPtr += LZ3F_compressBlock(dstPtr, cctxPtr->tmpIn, cctxPtr->tmpInSize, compress, cctxPtr->lz3CtxPtr, cctxPtr->prefs.compressionLevel);
    if (cctxPtr->prefs.frameInfo.blockMode==LZ3F_blockLinked) cctxPtr->tmpIn += cctxPtr->tmpInSize;
    cctxPtr->tmpInSize = 0;

    /* keep tmpIn within limits */
    if ((cctxPtr->tmpIn + cctxPtr->maxBlockSize) > (cctxPtr->tmpBuff + cctxPtr->maxBufferSize))   /* necessarily LZ3F_blockLinked */
    {
        int realDictSize = LZ3F_localSaveDict(cctxPtr);
        cctxPtr->tmpIn = cctxPtr->tmpBuff + realDictSize;
    }

    return dstPtr - dstStart;
}


/* LZ3F_compressEnd()
* When you want to properly finish the compressed frame, just call LZ3F_compressEnd().
* It will flush whatever data remained within compressionContext (like LZ3__flush())
* but also properly finalize the frame, with an endMark and a checksum.
* The result of the function is the number of bytes written into dstBuffer (necessarily >= 4 (endMark size))
* The function outputs an error code if it fails (can be tested using LZ3F_isError())
* The LZ3F_compressOptions_t structure is optional : you can provide NULL as argument.
* compressionContext can then be used again, starting with LZ3F_compressBegin(). The preferences will remain the same.
*/
size_t LZ3F_compressEnd(LZ3F_compressionContext_t compressionContext, void* dstBuffer, size_t dstMaxSize, const LZ3F_compressOptions_t* compressOptionsPtr)
{
    LZ3F_cctx_t* cctxPtr = (LZ3F_cctx_t*)compressionContext;
    BYTE* const dstStart = (BYTE*)dstBuffer;
    BYTE* dstPtr = dstStart;
    size_t errorCode;

    errorCode = LZ3F_flush(compressionContext, dstBuffer, dstMaxSize, compressOptionsPtr);
    if (LZ3F_isError(errorCode)) return errorCode;
    dstPtr += errorCode;

    LZ3F_writeLE32(dstPtr, 0);
    dstPtr+=4;   /* endMark */

    if (cctxPtr->prefs.frameInfo.contentChecksumFlag == LZ3F_contentChecksumEnabled)
    {
        U32 xxh = XX_H32_digest(&(cctxPtr->xxh));
        LZ3F_writeLE32(dstPtr, xxh);
        dstPtr+=4;   /* content Checksum */
    }

    cctxPtr->cStage = 0;   /* state is now re-usable (with identical preferences) */

    if (cctxPtr->prefs.frameInfo.contentSize)
    {
        if (cctxPtr->prefs.frameInfo.contentSize != cctxPtr->totalInSize)
            return (size_t)-LZ3F_ERROR_frameSize_wrong;
    }

    return dstPtr - dstStart;
}


/**********************************
*  Decompression functions
**********************************/

/* Resource management */

/* LZ3F_createDecompressionContext() :
* The first thing to do is to create a decompressionContext object, which will be used in all decompression operations.
* This is achieved using LZ3F_createDecompressionContext().
* The function will provide a pointer to a fully allocated and initialized LZ3F_decompressionContext object.
* If the result LZ3F_errorCode_t is not zero, there was an error during context creation.
* Object can release its memory using LZ3F_freeDecompressionContext();
*/
LZ3F_errorCode_t LZ3F_createDecompressionContext(LZ3F_decompressionContext_t* LZ3F_decompressionContextPtr, unsigned versionNumber)
{
    LZ3F_dctx_t* dctxPtr;

    dctxPtr = (LZ3F_dctx_t*)ALLOCATOR(sizeof(LZ3F_dctx_t));
    if (dctxPtr==NULL) return (LZ3F_errorCode_t)-LZ3F_ERROR_GENERIC;

    dctxPtr->version = versionNumber;
    *LZ3F_decompressionContextPtr = (LZ3F_decompressionContext_t)dctxPtr;
    return LZ3F_OK_NoError;
}

LZ3F_errorCode_t LZ3F_freeDecompressionContext(LZ3F_decompressionContext_t LZ3F_decompressionContext)
{
    LZ3F_errorCode_t result = LZ3F_OK_NoError;
    LZ3F_dctx_t* dctxPtr = (LZ3F_dctx_t*)LZ3F_decompressionContext;
    if (dctxPtr != NULL)   /* can accept NULL input, like free() */
    {
      result = (LZ3F_errorCode_t)dctxPtr->dStage;
      FREEMEM(dctxPtr->tmpIn);
      FREEMEM(dctxPtr->tmpOutBuffer);
      FREEMEM(dctxPtr);
    }
    return result;
}


/* ******************************************************************** */
/* ********************* Decompression ******************************** */
/* ******************************************************************** */

typedef enum { dstage_getHeader=0, dstage_storeHeader,
    dstage_getCBlockSize, dstage_storeCBlockSize,
    dstage_copyDirect,
    dstage_getCBlock, dstage_storeCBlock,
    dstage_decodeCBlock, dstage_decodeCBlock_intoDst,
    dstage_decodeCBlock_intoTmp, dstage_flushOut,
    dstage_getSuffix, dstage_storeSuffix,
    dstage_getSFrameSize, dstage_storeSFrameSize,
    dstage_skipSkippable
} dStage_t;


/* LZ3F_decodeHeader
   return : nb Bytes read from srcVoidPtr (necessarily <= srcSize)
            or an error code (testable with LZ3F_isError())
   output : set internal values of dctx, such as
            dctxPtr->frameInfo and dctxPtr->dStage.
   input  : srcVoidPtr points at the **beginning of the frame**
*/
static size_t LZ3F_decodeHeader(LZ3F_dctx_t* dctxPtr, const void* srcVoidPtr, size_t srcSize)
{
    BYTE FLG, BD, HC;
    unsigned version, blockMode, blockChecksumFlag, contentSizeFlag, contentChecksumFlag, blockSizeID;
    size_t bufferNeeded;
    size_t frameHeaderSize;
    const BYTE* srcPtr = (const BYTE*)srcVoidPtr;

    /* need to decode header to get frameInfo */
    if (srcSize < minFHSize) return (size_t)-LZ3F_ERROR_frameHeader_incomplete;   /* minimal frame header size */
    memset(&(dctxPtr->frameInfo), 0, sizeof(dctxPtr->frameInfo));

    /* special case : skippable frames */
    if ((LZ3F_readLE32(srcPtr) & 0xFFFFFFF0U) == LZ3F_MAGIC_SKIPPABLE_START)
    {
        dctxPtr->frameInfo.frameType = LZ3F_skippableFrame;
        if (srcVoidPtr == (void*)(dctxPtr->header))
        {
            dctxPtr->tmpInSize = srcSize;
            dctxPtr->tmpInTarget = 8;
            dctxPtr->dStage = dstage_storeSFrameSize;
            return srcSize;
        }
        else
        {
            dctxPtr->dStage = dstage_getSFrameSize;
            return 4;
        }
    }

    /* control magic number */
    if (LZ3F_readLE32(srcPtr) != LZ3F_MAGICNUMBER) return (size_t)-LZ3F_ERROR_frameType_unknown;
    dctxPtr->frameInfo.frameType = LZ3F_frame;

    /* Flags */
    FLG = srcPtr[4];
    version = (FLG>>6) & _2BITS;
    blockMode = (FLG>>5) & _1BIT;
    blockChecksumFlag = (FLG>>4) & _1BIT;
    contentSizeFlag = (FLG>>3) & _1BIT;
    contentChecksumFlag = (FLG>>2) & _1BIT;

    /* Frame Header Size */
    frameHeaderSize = contentSizeFlag ? maxFHSize : minFHSize;

    if (srcSize < frameHeaderSize)
    {
        /* not enough input to fully decode frame header */
        if (srcPtr != dctxPtr->header)
            memcpy(dctxPtr->header, srcPtr, srcSize);
        dctxPtr->tmpInSize = srcSize;
        dctxPtr->tmpInTarget = frameHeaderSize;
        dctxPtr->dStage = dstage_storeHeader;
        return srcSize;
    }

    BD = srcPtr[5];
    blockSizeID = (BD>>4) & _3BITS;

    /* validate */
    if (version != 1) return (size_t)-LZ3F_ERROR_headerVersion_wrong;        /* Version Number, only supported value */
    if (blockChecksumFlag != 0) return (size_t)-LZ3F_ERROR_blockChecksum_unsupported; /* Not supported for the time being */
    if (((FLG>>0)&_2BITS) != 0) return (size_t)-LZ3F_ERROR_reservedFlag_set; /* Reserved bits */
    if (((BD>>7)&_1BIT) != 0) return (size_t)-LZ3F_ERROR_reservedFlag_set;   /* Reserved bit */
    if (blockSizeID < 4) return (size_t)-LZ3F_ERROR_maxBlockSize_invalid;    /* 4-7 only supported values for the time being */
    if (((BD>>0)&_4BITS) != 0) return (size_t)-LZ3F_ERROR_reservedFlag_set;  /* Reserved bits */

    /* check */
    HC = LZ3F_headerChecksum(srcPtr+4, frameHeaderSize-5);
    if (HC != srcPtr[frameHeaderSize-1]) return (size_t)-LZ3F_ERROR_headerChecksum_invalid;   /* Bad header checksum error */

    /* save */
    dctxPtr->frameInfo.blockMode = (LZ3F_blockMode_t)blockMode;
    dctxPtr->frameInfo.contentChecksumFlag = (LZ3F_contentChecksum_t)contentChecksumFlag;
    dctxPtr->frameInfo.blockSizeID = (LZ3F_blockSizeID_t)blockSizeID;
    dctxPtr->maxBlockSize = LZ3F_getBlockSize(blockSizeID);
    if (contentSizeFlag)
        dctxPtr->frameRemainingSize = dctxPtr->frameInfo.contentSize = LZ3F_readLE64(srcPtr+6);

    /* init */
    if (contentChecksumFlag) XX_H32_reset(&(dctxPtr->xxh), 0);

    /* alloc */
    bufferNeeded = dctxPtr->maxBlockSize + ((dctxPtr->frameInfo.blockMode==LZ3F_blockLinked) * 128 KB);
    if (bufferNeeded > dctxPtr->maxBufferSize)   /* tmp buffers too small */
    {
        FREEMEM(dctxPtr->tmpIn);
        FREEMEM(dctxPtr->tmpOutBuffer);
        dctxPtr->maxBufferSize = bufferNeeded;
        dctxPtr->tmpIn = (BYTE*)ALLOCATOR(dctxPtr->maxBlockSize);
        if (dctxPtr->tmpIn == NULL) return (size_t)-LZ3F_ERROR_GENERIC;
        dctxPtr->tmpOutBuffer= (BYTE*)ALLOCATOR(dctxPtr->maxBufferSize);
        if (dctxPtr->tmpOutBuffer== NULL) return (size_t)-LZ3F_ERROR_GENERIC;
    }
    dctxPtr->tmpInSize = 0;
    dctxPtr->tmpInTarget = 0;
    dctxPtr->dict = dctxPtr->tmpOutBuffer;
    dctxPtr->dictSize = 0;
    dctxPtr->tmpOut = dctxPtr->tmpOutBuffer;
    dctxPtr->tmpOutStart = 0;
    dctxPtr->tmpOutSize = 0;

    dctxPtr->dStage = dstage_getCBlockSize;

    return frameHeaderSize;
}


/* LZ3F_getFrameInfo()
* This function decodes frame header information, such as blockSize.
* It is optional : you could start by calling directly LZ3F_decompress() instead.
* The objective is to extract header information without starting decompression, typically for allocation purposes.
* LZ3F_getFrameInfo() can also be used *after* starting decompression, on a valid LZ3F_decompressionContext_t.
* The number of bytes read from srcBuffer will be provided within *srcSizePtr (necessarily <= original value).
* You are expected to resume decompression from where it stopped (srcBuffer + *srcSizePtr)
* The function result is an hint of the better srcSize to use for next call to LZ3F_decompress,
* or an error code which can be tested using LZ3F_isError().
*/
LZ3F_errorCode_t LZ3F_getFrameInfo(LZ3F_decompressionContext_t dCtx, LZ3F_frameInfo_t* frameInfoPtr,
                                   const void* srcBuffer, size_t* srcSizePtr)
{
    LZ3F_dctx_t* dctxPtr = (LZ3F_dctx_t*)dCtx;

    if (dctxPtr->dStage > dstage_storeHeader)   /* note : requires dstage_* header related to be at beginning of enum */
    {
        size_t o=0, i=0;
        /* frameInfo already decoded */
        *srcSizePtr = 0;
        *frameInfoPtr = dctxPtr->frameInfo;
        return LZ3F_decompress(dCtx, NULL, &o, NULL, &i, NULL);
    }
    else
    {
        size_t o=0;
        size_t nextSrcSize = LZ3F_decompress(dCtx, NULL, &o, srcBuffer, srcSizePtr, NULL);
        if (dctxPtr->dStage <= dstage_storeHeader)   /* note : requires dstage_* header related to be at beginning of enum */
            return (size_t)-LZ3F_ERROR_frameHeader_incomplete;
        *frameInfoPtr = dctxPtr->frameInfo;
        return nextSrcSize;
    }
}


/* trivial redirector, for common prototype */
static int LZ3F_decompress_safe (const char* source, char* dest, int compressedSize, int maxDecompressedSize, const char* dictStart, int dictSize)
{
    (void)dictStart; (void)dictSize;
    return LZ3__decompress_safe (source, dest, compressedSize, maxDecompressedSize);
}


static void LZ3F_updateDict(LZ3F_dctx_t* dctxPtr, const BYTE* dstPtr, size_t dstSize, const BYTE* dstPtr0, unsigned withinTmp)
{
    if (dctxPtr->dictSize==0)
        dctxPtr->dict = (const BYTE*)dstPtr;   /* priority to dictionary continuity */

    if (dctxPtr->dict + dctxPtr->dictSize == dstPtr)   /* dictionary continuity */
    {
        dctxPtr->dictSize += dstSize;
        return;
    }

    if (dstPtr - dstPtr0 + dstSize >= 64 KB)   /* dstBuffer large enough to become dictionary */
    {
        dctxPtr->dict = (const BYTE*)dstPtr0;
        dctxPtr->dictSize = dstPtr - dstPtr0 + dstSize;
        return;
    }

    if ((withinTmp) && (dctxPtr->dict == dctxPtr->tmpOutBuffer))
    {
        /* assumption : dctxPtr->dict + dctxPtr->dictSize == dctxPtr->tmpOut + dctxPtr->tmpOutStart */
        dctxPtr->dictSize += dstSize;
        return;
    }

    if (withinTmp) /* copy relevant dict portion in front of tmpOut within tmpOutBuffer */
    {
        size_t preserveSize = dctxPtr->tmpOut - dctxPtr->tmpOutBuffer;
        size_t copySize = 64 KB - dctxPtr->tmpOutSize;
        const BYTE* oldDictEnd = dctxPtr->dict + dctxPtr->dictSize - dctxPtr->tmpOutStart;
        if (dctxPtr->tmpOutSize > 64 KB) copySize = 0;
        if (copySize > preserveSize) copySize = preserveSize;

        memcpy(dctxPtr->tmpOutBuffer + preserveSize - copySize, oldDictEnd - copySize, copySize);

        dctxPtr->dict = dctxPtr->tmpOutBuffer;
        dctxPtr->dictSize = preserveSize + dctxPtr->tmpOutStart + dstSize;
        return;
    }

    if (dctxPtr->dict == dctxPtr->tmpOutBuffer)     /* copy dst into tmp to complete dict */
    {
        if (dctxPtr->dictSize + dstSize > dctxPtr->maxBufferSize)   /* tmp buffer not large enough */
        {
            size_t preserveSize = 64 KB - dstSize;   /* note : dstSize < 64 KB */
            memcpy(dctxPtr->tmpOutBuffer, dctxPtr->dict + dctxPtr->dictSize - preserveSize, preserveSize);
            dctxPtr->dictSize = preserveSize;
        }
        memcpy(dctxPtr->tmpOutBuffer + dctxPtr->dictSize, dstPtr, dstSize);
        dctxPtr->dictSize += dstSize;
        return;
    }

    /* join dict & dest into tmp */
    {
        size_t preserveSize = 64 KB - dstSize;   /* note : dstSize < 64 KB */
        if (preserveSize > dctxPtr->dictSize) preserveSize = dctxPtr->dictSize;
        memcpy(dctxPtr->tmpOutBuffer, dctxPtr->dict + dctxPtr->dictSize - preserveSize, preserveSize);
        memcpy(dctxPtr->tmpOutBuffer + preserveSize, dstPtr, dstSize);
        dctxPtr->dict = dctxPtr->tmpOutBuffer;
        dctxPtr->dictSize = preserveSize + dstSize;
    }
}



/* LZ3F_decompress()
* Call this function repetitively to regenerate data compressed within srcBuffer.
* The function will attempt to decode *srcSizePtr from srcBuffer, into dstBuffer of maximum size *dstSizePtr.
*
* The number of bytes regenerated into dstBuffer will be provided within *dstSizePtr (necessarily <= original value).
*
* The number of bytes effectively read from srcBuffer will be provided within *srcSizePtr (necessarily <= original value).
* If the number of bytes read is < number of bytes provided, then the decompression operation is not complete.
* You will have to call it again, continuing from where it stopped.
*
* The function result is an hint of the better srcSize to use for next call to LZ3F_decompress.
* Basically, it's the size of the current (or remaining) compressed block + header of next block.
* Respecting the hint provides some boost to performance, since it allows less buffer shuffling.
* Note that this is just a hint, you can always provide any srcSize you want.
* When a frame is fully decoded, the function result will be 0.
* If decompression failed, function result is an error code which can be tested using LZ3F_isError().
*/
size_t LZ3F_decompress(LZ3F_decompressionContext_t decompressionContext,
                       void* dstBuffer, size_t* dstSizePtr,
                       const void* srcBuffer, size_t* srcSizePtr,
                       const LZ3F_decompressOptions_t* decompressOptionsPtr)
{
    LZ3F_dctx_t* dctxPtr = (LZ3F_dctx_t*)decompressionContext;
    LZ3F_decompressOptions_t optionsNull;
    const BYTE* const srcStart = (const BYTE*)srcBuffer;
    const BYTE* const srcEnd = srcStart + *srcSizePtr;
    const BYTE* srcPtr = srcStart;
    BYTE* const dstStart = (BYTE*)dstBuffer;
    BYTE* const dstEnd = dstStart + *dstSizePtr;
    BYTE* dstPtr = dstStart;
    const BYTE* selectedIn = NULL;
    unsigned doAnotherStage = 1;
    size_t nextSrcSizeHint = 1;


    memset(&optionsNull, 0, sizeof(optionsNull));
    if (decompressOptionsPtr==NULL) decompressOptionsPtr = &optionsNull;
    *srcSizePtr = 0;
    *dstSizePtr = 0;

    /* expect to continue decoding src buffer where it left previously */
    if (dctxPtr->srcExpect != NULL)
    {
        if (srcStart != dctxPtr->srcExpect) return (size_t)-LZ3F_ERROR_srcPtr_wrong;
    }

    /* programmed as a state machine */

    while (doAnotherStage)
    {

        switch(dctxPtr->dStage)
        {

        case dstage_getHeader:
            {
                if ((size_t)(srcEnd-srcPtr) >= maxFHSize)   /* enough to decode - shortcut */
                {
                    LZ3F_errorCode_t errorCode = LZ3F_decodeHeader(dctxPtr, srcPtr, srcEnd-srcPtr);
                    if (LZ3F_isError(errorCode)) return errorCode;
                    srcPtr += errorCode;
                    break;
                }
                dctxPtr->tmpInSize = 0;
                dctxPtr->tmpInTarget = minFHSize;   /* minimum to attempt decode */
                dctxPtr->dStage = dstage_storeHeader;
            }

        case dstage_storeHeader:
            {
                size_t sizeToCopy = dctxPtr->tmpInTarget - dctxPtr->tmpInSize;
                if (sizeToCopy > (size_t)(srcEnd - srcPtr)) sizeToCopy =  srcEnd - srcPtr;
                memcpy(dctxPtr->header + dctxPtr->tmpInSize, srcPtr, sizeToCopy);
                dctxPtr->tmpInSize += sizeToCopy;
                srcPtr += sizeToCopy;
                if (dctxPtr->tmpInSize < dctxPtr->tmpInTarget)
                {
                    nextSrcSizeHint = (dctxPtr->tmpInTarget - dctxPtr->tmpInSize) + BHSize;   /* rest of header + nextBlockHeader */
                    doAnotherStage = 0;   /* not enough src data, ask for some more */
                    break;
                }
                {
                    LZ3F_errorCode_t errorCode = LZ3F_decodeHeader(dctxPtr, dctxPtr->header, dctxPtr->tmpInTarget);
                    if (LZ3F_isError(errorCode)) return errorCode;
                }
                break;
            }

        case dstage_getCBlockSize:
            {
                if ((size_t)(srcEnd - srcPtr) >= BHSize)
                {
                    selectedIn = srcPtr;
                    srcPtr += BHSize;
                }
                else
                {
                /* not enough input to read cBlockSize field */
                    dctxPtr->tmpInSize = 0;
                    dctxPtr->dStage = dstage_storeCBlockSize;
                }
            }

            if (dctxPtr->dStage == dstage_storeCBlockSize)
        case dstage_storeCBlockSize:
            {
                size_t sizeToCopy = BHSize - dctxPtr->tmpInSize;
                if (sizeToCopy > (size_t)(srcEnd - srcPtr)) sizeToCopy = srcEnd - srcPtr;
                memcpy(dctxPtr->tmpIn + dctxPtr->tmpInSize, srcPtr, sizeToCopy);
                srcPtr += sizeToCopy;
                dctxPtr->tmpInSize += sizeToCopy;
                if (dctxPtr->tmpInSize < BHSize) /* not enough input to get full cBlockSize; wait for more */
                {
                    nextSrcSizeHint = BHSize - dctxPtr->tmpInSize;
                    doAnotherStage  = 0;
                    break;
                }
                selectedIn = dctxPtr->tmpIn;
            }

        /* case dstage_decodeCBlockSize: */   /* no more direct access, to prevent scan-build warning */
            {
                size_t nextCBlockSize = LZ3F_readLE32(selectedIn) & 0x7FFFFFFFU;
                if (nextCBlockSize==0)   /* frameEnd signal, no more CBlock */
                {
                    dctxPtr->dStage = dstage_getSuffix;
                    break;
                }
                if (nextCBlockSize > dctxPtr->maxBlockSize) return (size_t)-LZ3F_ERROR_GENERIC;   /* invalid cBlockSize */
                dctxPtr->tmpInTarget = nextCBlockSize;
                if (LZ3F_readLE32(selectedIn) & LZ3F_BLOCKUNCOMPRESSED_FLAG)
                {
                    dctxPtr->dStage = dstage_copyDirect;
                    break;
                }
                dctxPtr->dStage = dstage_getCBlock;
                if (dstPtr==dstEnd)
                {
                    nextSrcSizeHint = nextCBlockSize + BHSize;
                    doAnotherStage = 0;
                }
                break;
            }

        case dstage_copyDirect:   /* uncompressed block */
            {
                size_t sizeToCopy = dctxPtr->tmpInTarget;
                if ((size_t)(srcEnd-srcPtr) < sizeToCopy) sizeToCopy = srcEnd - srcPtr;  /* not enough input to read full block */
                if ((size_t)(dstEnd-dstPtr) < sizeToCopy) sizeToCopy = dstEnd - dstPtr;
                memcpy(dstPtr, srcPtr, sizeToCopy);
                if (dctxPtr->frameInfo.contentChecksumFlag) XX_H32_update(&(dctxPtr->xxh), srcPtr, sizeToCopy);
                if (dctxPtr->frameInfo.contentSize) dctxPtr->frameRemainingSize -= sizeToCopy;

                /* dictionary management */
                if (dctxPtr->frameInfo.blockMode==LZ3F_blockLinked)
                    LZ3F_updateDict(dctxPtr, dstPtr, sizeToCopy, dstStart, 0);

                srcPtr += sizeToCopy;
                dstPtr += sizeToCopy;
                if (sizeToCopy == dctxPtr->tmpInTarget)   /* all copied */
                {
                    dctxPtr->dStage = dstage_getCBlockSize;
                    break;
                }
                dctxPtr->tmpInTarget -= sizeToCopy;   /* still need to copy more */
                nextSrcSizeHint = dctxPtr->tmpInTarget + BHSize;
                doAnotherStage = 0;
                break;
            }

        case dstage_getCBlock:   /* entry from dstage_decodeCBlockSize */
            {
                if ((size_t)(srcEnd-srcPtr) < dctxPtr->tmpInTarget)
                {
                    dctxPtr->tmpInSize = 0;
                    dctxPtr->dStage = dstage_storeCBlock;
                    break;
                }
                selectedIn = srcPtr;
                srcPtr += dctxPtr->tmpInTarget;
                dctxPtr->dStage = dstage_decodeCBlock;
                break;
            }

        case dstage_storeCBlock:
            {
                size_t sizeToCopy = dctxPtr->tmpInTarget - dctxPtr->tmpInSize;
                if (sizeToCopy > (size_t)(srcEnd-srcPtr)) sizeToCopy = srcEnd-srcPtr;
                memcpy(dctxPtr->tmpIn + dctxPtr->tmpInSize, srcPtr, sizeToCopy);
                dctxPtr->tmpInSize += sizeToCopy;
                srcPtr += sizeToCopy;
                if (dctxPtr->tmpInSize < dctxPtr->tmpInTarget)  /* need more input */
                {
                    nextSrcSizeHint = (dctxPtr->tmpInTarget - dctxPtr->tmpInSize) + BHSize;
                    doAnotherStage=0;
                    break;
                }
                selectedIn = dctxPtr->tmpIn;
                dctxPtr->dStage = dstage_decodeCBlock;
                break;
            }

        case dstage_decodeCBlock:
            {
                if ((size_t)(dstEnd-dstPtr) < dctxPtr->maxBlockSize)   /* not enough place into dst : decode into tmpOut */
                    dctxPtr->dStage = dstage_decodeCBlock_intoTmp;
                else
                    dctxPtr->dStage = dstage_decodeCBlock_intoDst;
                break;
            }

        case dstage_decodeCBlock_intoDst:
            {
                int (*decoder)(const char*, char*, int, int, const char*, int);
                int decodedSize;

                if (dctxPtr->frameInfo.blockMode == LZ3F_blockLinked)
                    decoder = LZ3__decompress_safe_usingDict;
                else
                    decoder = LZ3F_decompress_safe;

                decodedSize = decoder((const char*)selectedIn, (char*)dstPtr, (int)dctxPtr->tmpInTarget, (int)dctxPtr->maxBlockSize, (const char*)dctxPtr->dict, (int)dctxPtr->dictSize);
                if (decodedSize < 0) return (size_t)-LZ3F_ERROR_GENERIC;   /* decompression failed */
                if (dctxPtr->frameInfo.contentChecksumFlag) XX_H32_update(&(dctxPtr->xxh), dstPtr, decodedSize);
                if (dctxPtr->frameInfo.contentSize) dctxPtr->frameRemainingSize -= decodedSize;

                /* dictionary management */
                if (dctxPtr->frameInfo.blockMode==LZ3F_blockLinked)
                    LZ3F_updateDict(dctxPtr, dstPtr, decodedSize, dstStart, 0);

                dstPtr += decodedSize;
                dctxPtr->dStage = dstage_getCBlockSize;
                break;
            }

        case dstage_decodeCBlock_intoTmp:
            {
                /* not enough place into dst : decode into tmpOut */
                int (*decoder)(const char*, char*, int, int, const char*, int);
                int decodedSize;

                if (dctxPtr->frameInfo.blockMode == LZ3F_blockLinked)
                    decoder = LZ3__decompress_safe_usingDict;
                else
                    decoder = LZ3F_decompress_safe;

                /* ensure enough place for tmpOut */
                if (dctxPtr->frameInfo.blockMode == LZ3F_blockLinked)
                {
                    if (dctxPtr->dict == dctxPtr->tmpOutBuffer)
                    {
                        if (dctxPtr->dictSize > 128 KB)
                        {
                            memcpy(dctxPtr->tmpOutBuffer, dctxPtr->dict + dctxPtr->dictSize - 64 KB, 64 KB);
                            dctxPtr->dictSize = 64 KB;
                        }
                        dctxPtr->tmpOut = dctxPtr->tmpOutBuffer + dctxPtr->dictSize;
                    }
                    else   /* dict not within tmp */
                    {
                        size_t reservedDictSpace = dctxPtr->dictSize;
                        if (reservedDictSpace > 64 KB) reservedDictSpace = 64 KB;
                        dctxPtr->tmpOut = dctxPtr->tmpOutBuffer + reservedDictSpace;
                    }
                }

                /* Decode */
                decodedSize = decoder((const char*)selectedIn, (char*)dctxPtr->tmpOut, (int)dctxPtr->tmpInTarget, (int)dctxPtr->maxBlockSize, (const char*)dctxPtr->dict, (int)dctxPtr->dictSize);
                if (decodedSize < 0) return (size_t)-LZ3F_ERROR_decompressionFailed;   /* decompression failed */
                if (dctxPtr->frameInfo.contentChecksumFlag) XX_H32_update(&(dctxPtr->xxh), dctxPtr->tmpOut, decodedSize);
                if (dctxPtr->frameInfo.contentSize) dctxPtr->frameRemainingSize -= decodedSize;
                dctxPtr->tmpOutSize = decodedSize;
                dctxPtr->tmpOutStart = 0;
                dctxPtr->dStage = dstage_flushOut;
                break;
            }

        case dstage_flushOut:  /* flush decoded data from tmpOut to dstBuffer */
            {
                size_t sizeToCopy = dctxPtr->tmpOutSize - dctxPtr->tmpOutStart;
                if (sizeToCopy > (size_t)(dstEnd-dstPtr)) sizeToCopy = dstEnd-dstPtr;
                memcpy(dstPtr, dctxPtr->tmpOut + dctxPtr->tmpOutStart, sizeToCopy);

                /* dictionary management */
                if (dctxPtr->frameInfo.blockMode==LZ3F_blockLinked)
                    LZ3F_updateDict(dctxPtr, dstPtr, sizeToCopy, dstStart, 1);

                dctxPtr->tmpOutStart += sizeToCopy;
                dstPtr += sizeToCopy;

                /* end of flush ? */
                if (dctxPtr->tmpOutStart == dctxPtr->tmpOutSize)
                {
                    dctxPtr->dStage = dstage_getCBlockSize;
                    break;
                }
                nextSrcSizeHint = BHSize;
                doAnotherStage = 0;   /* still some data to flush */
                break;
            }

        case dstage_getSuffix:
            {
                size_t suffixSize = dctxPtr->frameInfo.contentChecksumFlag * 4;
                if (dctxPtr->frameRemainingSize) return (size_t)-LZ3F_ERROR_frameSize_wrong;   /* incorrect frame size decoded */
                if (suffixSize == 0)   /* frame completed */
                {
                    nextSrcSizeHint = 0;
                    dctxPtr->dStage = dstage_getHeader;
                    doAnotherStage = 0;
                    break;
                }
                if ((srcEnd - srcPtr) < 4)   /* not enough size for entire CRC */
                {
                    dctxPtr->tmpInSize = 0;
                    dctxPtr->dStage = dstage_storeSuffix;
                }
                else
                {
                    selectedIn = srcPtr;
                    srcPtr += 4;
                }
            }

            if (dctxPtr->dStage == dstage_storeSuffix)
        case dstage_storeSuffix:
            {
                size_t sizeToCopy = 4 - dctxPtr->tmpInSize;
                if (sizeToCopy > (size_t)(srcEnd - srcPtr)) sizeToCopy = srcEnd - srcPtr;
                memcpy(dctxPtr->tmpIn + dctxPtr->tmpInSize, srcPtr, sizeToCopy);
                srcPtr += sizeToCopy;
                dctxPtr->tmpInSize += sizeToCopy;
                if (dctxPtr->tmpInSize < 4)  /* not enough input to read complete suffix */
                {
                    nextSrcSizeHint = 4 - dctxPtr->tmpInSize;
                    doAnotherStage=0;
                    break;
                }
                selectedIn = dctxPtr->tmpIn;
            }

        /* case dstage_checkSuffix: */   /* no direct call, to avoid scan-build warning */
            {
                U32 readCRC = LZ3F_readLE32(selectedIn);
                U32 resultCRC = XX_H32_digest(&(dctxPtr->xxh));
                if (readCRC != resultCRC) return (size_t)-LZ3F_ERROR_contentChecksum_invalid;
                nextSrcSizeHint = 0;
                dctxPtr->dStage = dstage_getHeader;
                doAnotherStage = 0;
                break;
            }

        case dstage_getSFrameSize:
            {
                if ((srcEnd - srcPtr) >= 4)
                {
                    selectedIn = srcPtr;
                    srcPtr += 4;
                }
                else
                {
                /* not enough input to read cBlockSize field */
                    dctxPtr->tmpInSize = 4;
                    dctxPtr->tmpInTarget = 8;
                    dctxPtr->dStage = dstage_storeSFrameSize;
                }
            }

            if (dctxPtr->dStage == dstage_storeSFrameSize)
        case dstage_storeSFrameSize:
            {
                size_t sizeToCopy = dctxPtr->tmpInTarget - dctxPtr->tmpInSize;
                if (sizeToCopy > (size_t)(srcEnd - srcPtr)) sizeToCopy = srcEnd - srcPtr;
                memcpy(dctxPtr->header + dctxPtr->tmpInSize, srcPtr, sizeToCopy);
                srcPtr += sizeToCopy;
                dctxPtr->tmpInSize += sizeToCopy;
                if (dctxPtr->tmpInSize < dctxPtr->tmpInTarget) /* not enough input to get full sBlockSize; wait for more */
                {
                    nextSrcSizeHint = dctxPtr->tmpInTarget - dctxPtr->tmpInSize;
                    doAnotherStage = 0;
                    break;
                }
                selectedIn = dctxPtr->header + 4;
            }

        /* case dstage_decodeSFrameSize: */   /* no direct access */
            {
                size_t SFrameSize = LZ3F_readLE32(selectedIn);
                dctxPtr->frameInfo.contentSize = SFrameSize;
                dctxPtr->tmpInTarget = SFrameSize;
                dctxPtr->dStage = dstage_skipSkippable;
                break;
            }

        case dstage_skipSkippable:
            {
                size_t skipSize = dctxPtr->tmpInTarget;
                if (skipSize > (size_t)(srcEnd-srcPtr)) skipSize = srcEnd-srcPtr;
                srcPtr += skipSize;
                dctxPtr->tmpInTarget -= skipSize;
                doAnotherStage = 0;
                nextSrcSizeHint = dctxPtr->tmpInTarget;
                if (nextSrcSizeHint) break;
                dctxPtr->dStage = dstage_getHeader;
                break;
            }
        }
    }

    /* preserve dictionary within tmp if necessary */
    if ( (dctxPtr->frameInfo.blockMode==LZ3F_blockLinked)
        &&(dctxPtr->dict != dctxPtr->tmpOutBuffer)
        &&(!decompressOptionsPtr->stableDst)
        &&((unsigned)(dctxPtr->dStage-1) < (unsigned)(dstage_getSuffix-1))
        )
    {
        if (dctxPtr->dStage == dstage_flushOut)
        {
            size_t preserveSize = dctxPtr->tmpOut - dctxPtr->tmpOutBuffer;
            size_t copySize = 64 KB - dctxPtr->tmpOutSize;
            const BYTE* oldDictEnd = dctxPtr->dict + dctxPtr->dictSize - dctxPtr->tmpOutStart;
            if (dctxPtr->tmpOutSize > 64 KB) copySize = 0;
            if (copySize > preserveSize) copySize = preserveSize;

            memcpy(dctxPtr->tmpOutBuffer + preserveSize - copySize, oldDictEnd - copySize, copySize);

            dctxPtr->dict = dctxPtr->tmpOutBuffer;
            dctxPtr->dictSize = preserveSize + dctxPtr->tmpOutStart;
        }
        else
        {
            size_t newDictSize = dctxPtr->dictSize;
            const BYTE* oldDictEnd = dctxPtr->dict + dctxPtr->dictSize;
            if ((newDictSize) > 64 KB) newDictSize = 64 KB;

            memcpy(dctxPtr->tmpOutBuffer, oldDictEnd - newDictSize, newDictSize);

            dctxPtr->dict = dctxPtr->tmpOutBuffer;
            dctxPtr->dictSize = newDictSize;
            dctxPtr->tmpOut = dctxPtr->tmpOutBuffer + newDictSize;
        }
    }

    /* require function to be called again from position where it stopped */
    if (srcPtr<srcEnd)
        dctxPtr->srcExpect = srcPtr;
    else
        dctxPtr->srcExpect = NULL;

    *srcSizePtr = (srcPtr - srcStart);
    *dstSizePtr = (dstPtr - dstStart);
    return nextSrcSizeHint;
}
