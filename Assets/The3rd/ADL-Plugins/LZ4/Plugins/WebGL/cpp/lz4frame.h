
/* LZ3F is a stand-alone API to create LZ3-compressed frames
 * fully conformant to specification v1.5.1.
 * All related operations, including memory management, are handled by the library.
 * You don't need lz3.h when using lz3frame.h.
 * */

#pragma once

#if defined (__cplusplus)
extern "C" {
#endif

/**************************************
*  Includes
**************************************/
#include <stddef.h>   /* size_t */


/**************************************
 * Error management
 * ************************************/
typedef size_t LZ3F_errorCode_t;

unsigned    LZ3F_isError(LZ3F_errorCode_t code);
const char* LZ3F_getErrorName(LZ3F_errorCode_t code);   /* return error code string; useful for debugging */


/**************************************
 * Frame compression types
 * ************************************/
//#define LZ3F_DISABLE_OBSOLETE_ENUMS
#ifndef LZ3F_DISABLE_OBSOLETE_ENUMS
#  define LZ3F_OBSOLETE_ENUM(x) ,x
#else
#  define LZ3F_OBSOLETE_ENUM(x)
#endif

typedef enum {
    LZ3F_default=0,
    LZ3F_max64KB=4,
    LZ3F_max256KB=5,
    LZ3F_max1MB=6,
    LZ3F_max4MB=7
    LZ3F_OBSOLETE_ENUM(max64KB = LZ3F_max64KB)
    LZ3F_OBSOLETE_ENUM(max256KB = LZ3F_max256KB)
    LZ3F_OBSOLETE_ENUM(max1MB = LZ3F_max1MB)
    LZ3F_OBSOLETE_ENUM(max4MB = LZ3F_max4MB)
} LZ3F_blockSizeID_t;

typedef enum {
    LZ3F_blockLinked=0,
    LZ3F_blockIndependent
    LZ3F_OBSOLETE_ENUM(blockLinked = LZ3F_blockLinked)
    LZ3F_OBSOLETE_ENUM(blockIndependent = LZ3F_blockIndependent)
} LZ3F_blockMode_t;

typedef enum {
    LZ3F_noContentChecksum=0,
    LZ3F_contentChecksumEnabled
    LZ3F_OBSOLETE_ENUM(noContentChecksum = LZ3F_noContentChecksum)
    LZ3F_OBSOLETE_ENUM(contentChecksumEnabled = LZ3F_contentChecksumEnabled)
} LZ3F_contentChecksum_t;

typedef enum {
    LZ3F_frame=0,
    LZ3F_skippableFrame
    LZ3F_OBSOLETE_ENUM(skippableFrame = LZ3F_skippableFrame)
} LZ3F_frameType_t;

#ifndef LZ3F_DISABLE_OBSOLETE_ENUMS
typedef LZ3F_blockSizeID_t blockSizeID_t;
typedef LZ3F_blockMode_t blockMode_t;
typedef LZ3F_frameType_t frameType_t;
typedef LZ3F_contentChecksum_t contentChecksum_t;
#endif

typedef struct {
  LZ3F_blockSizeID_t     blockSizeID;           /* max64KB, max256KB, max1MB, max4MB ; 0 == default */
  LZ3F_blockMode_t       blockMode;             /* blockLinked, blockIndependent ; 0 == default */
  LZ3F_contentChecksum_t contentChecksumFlag;   /* noContentChecksum, contentChecksumEnabled ; 0 == default  */
  LZ3F_frameType_t       frameType;             /* LZ3F_frame, skippableFrame ; 0 == default */
  unsigned long long     contentSize;           /* Size of uncompressed (original) content ; 0 == unknown */
  unsigned               reserved[2];           /* must be zero for forward compatibility */
} LZ3F_frameInfo_t;

typedef struct {
  LZ3F_frameInfo_t frameInfo;
  int      compressionLevel;       /* 0 == default (fast mode); values above 16 count as 16; values below 0 count as 0 */
  unsigned autoFlush;              /* 1 == always flush (reduce need for tmp buffer) */
  unsigned reserved[4];            /* must be zero for forward compatibility */
} LZ3F_preferences_t;


/***********************************
 * Simple compression function
 * *********************************/
size_t LZ3F_compressFrameBound(size_t srcSize, const LZ3F_preferences_t* preferencesPtr);

size_t LZ3F_compressFrame(void* dstBuffer, size_t dstMaxSize, const void* srcBuffer, size_t srcSize, const LZ3F_preferences_t* preferencesPtr);
/* LZ3F_compressFrame()
 * Compress an entire srcBuffer into a valid LZ3 frame, as defined by specification v1.5.1
 * The most important rule is that dstBuffer MUST be large enough (dstMaxSize) to ensure compression completion even in worst case.
 * You can get the minimum value of dstMaxSize by using LZ3F_compressFrameBound()
 * If this condition is not respected, LZ3F_compressFrame() will fail (result is an errorCode)
 * The LZ3F_preferences_t structure is optional : you can provide NULL as argument. All preferences will be set to default.
 * The result of the function is the number of bytes written into dstBuffer.
 * The function outputs an error code if it fails (can be tested using LZ3F_isError())
 */



/**********************************
*  Advanced compression functions
**********************************/
typedef struct LZ3F_cctx_s* LZ3F_compressionContext_t;   /* must be aligned on 8-bytes */

typedef struct {
  unsigned stableSrc;    /* 1 == src content will remain available on future calls to LZ3F_compress(); avoid saving src content within tmp buffer as future dictionary */
  unsigned reserved[3];
} LZ3F_compressOptions_t;

/* Resource Management */

#define LZ3F_VERSION 100
LZ3F_errorCode_t LZ3F_createCompressionContext(LZ3F_compressionContext_t* cctxPtr, unsigned version);
LZ3F_errorCode_t LZ3F_freeCompressionContext(LZ3F_compressionContext_t cctx);
/* LZ3F_createCompressionContext() :
 * The first thing to do is to create a compressionContext object, which will be used in all compression operations.
 * This is achieved using LZ3F_createCompressionContext(), which takes as argument a version and an LZ3F_preferences_t structure.
 * The version provided MUST be LZ3F_VERSION. It is intended to track potential version differences between different binaries.
 * The function will provide a pointer to a fully allocated LZ3F_compressionContext_t object.
 * If the result LZ3F_errorCode_t is not zero, there was an error during context creation.
 * Object can release its memory using LZ3F_freeCompressionContext();
 */


/* Compression */

size_t LZ3F_compressBegin(LZ3F_compressionContext_t cctx, void* dstBuffer, size_t dstMaxSize, const LZ3F_preferences_t* prefsPtr);
/* LZ3F_compressBegin() :
 * will write the frame header into dstBuffer.
 * dstBuffer must be large enough to accommodate a header (dstMaxSize). Maximum header size is 15 bytes.
 * The LZ3F_preferences_t structure is optional : you can provide NULL as argument, all preferences will then be set to default.
 * The result of the function is the number of bytes written into dstBuffer for the header
 * or an error code (can be tested using LZ3F_isError())
 */

size_t LZ3F_compressBound(size_t srcSize, const LZ3F_preferences_t* prefsPtr);
/* LZ3F_compressBound() :
 * Provides the minimum size of Dst buffer given srcSize to handle worst case situations.
 * Different preferences can produce different results.
 * prefsPtr is optional : you can provide NULL as argument, all preferences will then be set to cover worst case.
 * This function includes frame termination cost (4 bytes, or 8 if frame checksum is enabled)
 */

size_t LZ3F_compressUpdate(LZ3F_compressionContext_t cctx, void* dstBuffer, size_t dstMaxSize, const void* srcBuffer, size_t srcSize, const LZ3F_compressOptions_t* cOptPtr);
/* LZ3F_compressUpdate()
 * LZ3F_compressUpdate() can be called repetitively to compress as much data as necessary.
 * The most important rule is that dstBuffer MUST be large enough (dstMaxSize) to ensure compression completion even in worst case.
 * You can get the minimum value of dstMaxSize by using LZ3F_compressBound().
 * If this condition is not respected, LZ3F_compress() will fail (result is an errorCode).
 * LZ3F_compressUpdate() doesn't guarantee error recovery, so you have to reset compression context when an error occurs.
 * The LZ3F_compressOptions_t structure is optional : you can provide NULL as argument.
 * The result of the function is the number of bytes written into dstBuffer : it can be zero, meaning input data was just buffered.
 * The function outputs an error code if it fails (can be tested using LZ3F_isError())
 */

size_t LZ3F_flush(LZ3F_compressionContext_t cctx, void* dstBuffer, size_t dstMaxSize, const LZ3F_compressOptions_t* cOptPtr);
/* LZ3F_flush()
 * Should you need to generate compressed data immediately, without waiting for the current block to be filled,
 * you can call LZ3__flush(), which will immediately compress any remaining data buffered within cctx.
 * Note that dstMaxSize must be large enough to ensure the operation will be successful.
 * LZ3F_compressOptions_t structure is optional : you can provide NULL as argument.
 * The result of the function is the number of bytes written into dstBuffer
 * (it can be zero, this means there was no data left within cctx)
 * The function outputs an error code if it fails (can be tested using LZ3F_isError())
 */

size_t LZ3F_compressEnd(LZ3F_compressionContext_t cctx, void* dstBuffer, size_t dstMaxSize, const LZ3F_compressOptions_t* cOptPtr);
/* LZ3F_compressEnd()
 * When you want to properly finish the compressed frame, just call LZ3F_compressEnd().
 * It will flush whatever data remained within compressionContext (like LZ3__flush())
 * but also properly finalize the frame, with an endMark and a checksum.
 * The result of the function is the number of bytes written into dstBuffer (necessarily >= 4 (endMark), or 8 if optional frame checksum is enabled)
 * The function outputs an error code if it fails (can be tested using LZ3F_isError())
 * The LZ3F_compressOptions_t structure is optional : you can provide NULL as argument.
 * A successful call to LZ3F_compressEnd() makes cctx available again for next compression task.
 */


/***********************************
*  Decompression functions
***********************************/

typedef struct LZ3F_dctx_s* LZ3F_decompressionContext_t;   /* must be aligned on 8-bytes */

typedef struct {
  unsigned stableDst;       /* guarantee that decompressed data will still be there on next function calls (avoid storage into tmp buffers) */
  unsigned reserved[3];
} LZ3F_decompressOptions_t;


/* Resource management */

LZ3F_errorCode_t LZ3F_createDecompressionContext(LZ3F_decompressionContext_t* dctxPtr, unsigned version);
LZ3F_errorCode_t LZ3F_freeDecompressionContext(LZ3F_decompressionContext_t dctx);
/* LZ3F_createDecompressionContext() :
 * The first thing to do is to create an LZ3F_decompressionContext_t object, which will be used in all decompression operations.
 * This is achieved using LZ3F_createDecompressionContext().
 * The version provided MUST be LZ3F_VERSION. It is intended to track potential breaking differences between different versions.
 * The function will provide a pointer to a fully allocated and initialized LZ3F_decompressionContext_t object.
 * The result is an errorCode, which can be tested using LZ3F_isError().
 * dctx memory can be released using LZ3F_freeDecompressionContext();
 * The result of LZ3F_freeDecompressionContext() is indicative of the current state of decompressionContext when being released.
 * That is, it should be == 0 if decompression has been completed fully and correctly.
 */


/* Decompression */

size_t LZ3F_getFrameInfo(LZ3F_decompressionContext_t dctx,
                         LZ3F_frameInfo_t* frameInfoPtr,
                         const void* srcBuffer, size_t* srcSizePtr);
/* LZ3F_getFrameInfo()
 * This function decodes frame header information (such as max blockSize, frame checksum, etc.).
 * Its usage is optional : you can start by calling directly LZ3F_decompress() instead.
 * The objective is to extract frame header information, typically for allocation purposes.
 * LZ3F_getFrameInfo() can also be used anytime *after* starting decompression, on any valid LZ3F_decompressionContext_t.
 * The result is *copied* into an existing LZ3F_frameInfo_t structure which must be already allocated.
 * The number of bytes read from srcBuffer will be provided within *srcSizePtr (necessarily <= original value).
 * The function result is an hint of how many srcSize bytes LZ3F_decompress() expects for next call,
 *                        or an error code which can be tested using LZ3F_isError()
 *                        (typically, when there is not enough src bytes to fully decode the frame header)
 * You are expected to resume decompression from where it stopped (srcBuffer + *srcSizePtr)
 */

size_t LZ3F_decompress(LZ3F_decompressionContext_t dctx,
                       void* dstBuffer, size_t* dstSizePtr,
                       const void* srcBuffer, size_t* srcSizePtr,
                       const LZ3F_decompressOptions_t* dOptPtr);
/* LZ3F_decompress()
 * Call this function repetitively to regenerate data compressed within srcBuffer.
 * The function will attempt to decode *srcSizePtr bytes from srcBuffer, into dstBuffer of maximum size *dstSizePtr.
 *
 * The number of bytes regenerated into dstBuffer will be provided within *dstSizePtr (necessarily <= original value).
 *
 * The number of bytes read from srcBuffer will be provided within *srcSizePtr (necessarily <= original value).
 * If number of bytes read is < number of bytes provided, then decompression operation is not completed.
 * It typically happens when dstBuffer is not large enough to contain all decoded data.
 * LZ3F_decompress() must be called again, starting from where it stopped (srcBuffer + *srcSizePtr)
 * The function will check this condition, and refuse to continue if it is not respected.
 *
 * dstBuffer is supposed to be flushed between each call to the function, since its content will be overwritten.
 * dst arguments can be changed at will with each consecutive call to the function.
 *
 * The function result is an hint of how many srcSize bytes LZ3F_decompress() expects for next call.
 * Schematically, it's the size of the current (or remaining) compressed block + header of next block.
 * Respecting the hint provides some boost to performance, since it does skip intermediate buffers.
 * This is just a hint, you can always provide any srcSize you want.
 * When a frame is fully decoded, the function result will be 0 (no more data expected).
 * If decompression failed, function result is an error code, which can be tested using LZ3F_isError().
 *
 * After a frame is fully decoded, dctx can be used again to decompress another frame.
 */


#if defined (__cplusplus)
}
#endif
