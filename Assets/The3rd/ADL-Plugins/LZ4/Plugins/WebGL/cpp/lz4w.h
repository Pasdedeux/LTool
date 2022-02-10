

#pragma once

/* ************************************************** */
/* Special input/output values                        */
/* ************************************************** */
#define NULL_OUTPUT "null"
static char const stdinmark[] = "stdin";
static char const stdoutmark[] = "stdout";
#ifdef _WIN32
static char const nulmark[] = "nul";
#else
static char const nulmark[] = "/dev/null";
#endif


/* ************************************************** */
/* ****************** Functions ********************* */
/* ************************************************** */

//int __declspec(dllexport) LZ3CompressFile  (const char* input_filename, const char* output_filename, int compressionlevel);
//int __declspec(dllexport) LZ3DecompressFile(const char* input_filename, const char* output_filename);


//int LZ3ComressFiles(const char** inFileNamesTable, int ifntSize, const char* suffix, int compressionlevel);
//int LZ3DecomressFiles(const char** inFileNamesTable, int ifntSize, const char* suffix);

/* ************************************************** */
/* ****************** Parameters ******************** */
/* ************************************************** */

