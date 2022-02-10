/*
   LZ3 HC - High Compression Mode of LZ3
   Header File
   Copyright (C) 2011-2015, Yann Collet.
   BSD 2-Clause License (http://www.opensource.org/licenses/bsd-license.php)

   Redistribution and use in source and binary forms, with or without
   modification, are permitted provided that the following conditions are
   met:

       * Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
       * Redistributions in binary form must reproduce the above
   copyright notice, this list of conditions and the following disclaimer
   in the documentation and/or other materials provided with the
   distribution.

   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
   A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
   OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
   SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
   LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
   DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
   THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
   (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
   OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

   You can contact the author at :
   - LZ3 source repository : https://github.com/Cyan4973/lz3
   - LZ3 public forum : https://groups.google.com/forum/#!forum/lz3c
*/
#pragma once


#if defined (__cplusplus)
extern "C" {
#endif

/*****************************
*  Includes
*****************************/
#include <stddef.h>   /* size_t */


/**************************************
*  Block Compression
**************************************/
int LZ3__compress_HC (const char* src, char* dst, int srcSize, int maxDstSize, int compressionLevel);
/*
LZ3__compress_HC :
    Destination buffer 'dst' must be already allocated.
    Compression completion is guaranteed if 'dst' buffer is sized to handle worst circumstances (data not compressible)
    Worst size evaluation is provided by function LZ3__compressBound() (see "lz3.h")
      srcSize  : Max supported value is LZ3__MAX_INPUT_SIZE (see "lz3.h")
      compressionLevel : Recommended values are between 4 and 9, although any value between 0 and 16 will work.
                         0 means "use default value" (see lz3hc.c).
                         Values >16 behave the same as 16.
      return : the number of bytes written into buffer 'dst'
            or 0 if compression fails.
*/


/* Note :
   Decompression functions are provided within LZ3 source code (see "lz3.h") (BSD license)
*/


int LZ3__sizeofStateHC(void);
int LZ3__compress_HC_extStateHC(void* state, const char* src, char* dst, int srcSize, int maxDstSize, int compressionLevel);
/*
LZ3__compress_HC_extStateHC() :
   Use this function if you prefer to manually allocate memory for compression tables.
   To know how much memory must be allocated for the compression tables, use :
      int LZ3__sizeofStateHC();

   Allocated memory must be aligned on 8-bytes boundaries (which a normal malloc() will do properly).

   The allocated memory can then be provided to the compression functions using 'void* state' parameter.
   LZ3__compress_HC_extStateHC() is equivalent to previously described function.
   It just uses externally allocated memory for stateHC.
*/


/**************************************
*  Streaming Compression
**************************************/
#define LZ3__STREAMHCSIZE        262192
#define LZ3__STREAMHCSIZE_SIZET (LZ3__STREAMHCSIZE / sizeof(size_t))
typedef struct { size_t table[LZ3__STREAMHCSIZE_SIZET]; } LZ3__streamHC_t;
/*
  LZ3__streamHC_t
  This structure allows static allocation of LZ3 HC streaming state.
  State must then be initialized using LZ3__resetStreamHC() before first use.

  Static allocation should only be used in combination with static linking.
  If you want to use LZ3 as a DLL, please use construction functions below, which are future-proof.
*/


LZ3__streamHC_t* LZ3__createStreamHC(void);
int             LZ3__freeStreamHC (LZ3__streamHC_t* streamHCPtr);
/*
  These functions create and release memory for LZ3 HC streaming state.
  Newly created states are already initialized.
  Existing state space can be re-used anytime using LZ3__resetStreamHC().
  If you use LZ3 as a DLL, use these functions instead of static structure allocation,
  to avoid size mismatch between different versions.
*/

void LZ3__resetStreamHC (LZ3__streamHC_t* streamHCPtr, int compressionLevel);
int  LZ3__loadDictHC (LZ3__streamHC_t* streamHCPtr, const char* dictionary, int dictSize);

int LZ3__compress_HC_continue (LZ3__streamHC_t* streamHCPtr, const char* src, char* dst, int srcSize, int maxDstSize);

int LZ3__saveDictHC (LZ3__streamHC_t* streamHCPtr, char* safeBuffer, int maxDictSize);

/*
  These functions compress data in successive blocks of any size, using previous blocks as dictionary.
  One key assumption is that previous blocks (up to 64 KB) remain read-accessible while compressing next blocks.
  There is an exception for ring buffers, which can be smaller 64 KB.
  Such case is automatically detected and correctly handled by LZ3__compress_HC_continue().

  Before starting compression, state must be properly initialized, using LZ3__resetStreamHC().
  A first "fictional block" can then be designated as initial dictionary, using LZ3__loadDictHC() (Optional).

  Then, use LZ3__compress_HC_continue() to compress each successive block.
  It works like LZ3__compress_HC(), but use previous memory blocks as dictionary to improve compression.
  Previous memory blocks (including initial dictionary when present) must remain accessible and unmodified during compression.
  As a reminder, size 'dst' buffer to handle worst cases, using LZ3__compressBound(), to ensure success of compression operation.

  If, for any reason, previous data blocks can't be preserved unmodified in memory during next compression block,
  you must save it to a safer memory space, using LZ3__saveDictHC().
  Return value of LZ3__saveDictHC() is the size of dictionary effectively saved into 'safeBuffer'.
*/



/**************************************
*  Deprecated Functions
**************************************/
/* Deprecate Warnings */
/* Should these warnings messages be a problem,
   it is generally possible to disable them,
   with -Wno-deprecated-declarations for gcc
   or _CRT_SECURE_NO_WARNINGS in Visual for example.
   You can also define LZ3__DEPRECATE_WARNING_DEFBLOCK. */
#ifndef LZ3__DEPRECATE_WARNING_DEFBLOCK
#  define LZ3__DEPRECATE_WARNING_DEFBLOCK
#  define LZ3__GCC_VERSION (__GNUC__ * 100 + __GNUC_MINOR__)
#  if (LZ3__GCC_VERSION >= 405) || defined(__clang__)
#    define LZ3__DEPRECATED(message) __attribute__((deprecated(message)))
#  elif (LZ3__GCC_VERSION >= 301)
#    define LZ3__DEPRECATED(message) __attribute__((deprecated))
#  elif defined(_MSC_VER)
#    define LZ3__DEPRECATED(message) __declspec(deprecated(message))
#  else
#    pragma message("WARNING: You need to implement LZ3__DEPRECATED for this compiler")
#    define LZ3__DEPRECATED(message)
#  endif
#endif // LZ3__DEPRECATE_WARNING_DEFBLOCK

/* compression functions */
/* these functions are planned to trigger warning messages by r131 approximately */
int LZ3__compressHC                (const char* source, char* dest, int inputSize);
int LZ3__compressHC_limitedOutput  (const char* source, char* dest, int inputSize, int maxOutputSize);
int LZ3__compressHC2               (const char* source, char* dest, int inputSize, int compressionLevel);
int LZ3__compressHC2_limitedOutput (const char* source, char* dest, int inputSize, int maxOutputSize, int compressionLevel);
int LZ3__compressHC_withStateHC               (void* state, const char* source, char* dest, int inputSize);
int LZ3__compressHC_limitedOutput_withStateHC (void* state, const char* source, char* dest, int inputSize, int maxOutputSize);
int LZ3__compressHC2_withStateHC              (void* state, const char* source, char* dest, int inputSize, int compressionLevel);
int LZ3__compressHC2_limitedOutput_withStateHC(void* state, const char* source, char* dest, int inputSize, int maxOutputSize, int compressionLevel);
int LZ3__compressHC_continue               (LZ3__streamHC_t* LZ3__streamHCPtr, const char* source, char* dest, int inputSize);
int LZ3__compressHC_limitedOutput_continue (LZ3__streamHC_t* LZ3__streamHCPtr, const char* source, char* dest, int inputSize, int maxOutputSize);

/* Streaming functions following the older model; should no longer be used */
LZ3__DEPRECATED("use LZ3__createStreamHC() instead") void* LZ3__createHC (char* inputBuffer);
LZ3__DEPRECATED("use LZ3__saveDictHC() instead")     char* LZ3__slideInputBufferHC (void* LZ3HC_Data);
LZ3__DEPRECATED("use LZ3__freeStreamHC() instead")   int   LZ3__freeHC (void* LZ3HC_Data);
LZ3__DEPRECATED("use LZ3__compress_HC_continue() instead") int   LZ3__compressHC2_continue (void* LZ3HC_Data, const char* source, char* dest, int inputSize, int compressionLevel);
LZ3__DEPRECATED("use LZ3__compress_HC_continue() instead") int   LZ3__compressHC2_limitedOutput_continue (void* LZ3HC_Data, const char* source, char* dest, int inputSize, int maxOutputSize, int compressionLevel);
LZ3__DEPRECATED("use LZ3__createStreamHC() instead") int   LZ3__sizeofStreamStateHC(void);
LZ3__DEPRECATED("use LZ3__resetStreamHC() instead")  int   LZ3__resetStreamStateHC(void* state, char* inputBuffer);


#if defined (__cplusplus)
}
#endif
