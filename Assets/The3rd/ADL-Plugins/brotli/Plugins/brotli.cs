using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class brotli{

#if !UNITY_WEBPLAYER  || UNITY_EDITOR


#if UNITY_5_4_OR_NEWER
	#if (UNITY_ANDROID || UNITY_STANDALONE_LINUX) && !UNITY_EDITOR || UNITY_EDITOR_LINUX
		private const string libname = "brotli";
	#elif UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		private const string libname = "libbrotli";
	#endif
#else
	#if (UNITY_ANDROID || UNITY_STANDALONE_LINUX) && !UNITY_EDITOR
		private const string libname = "brotli";
	#endif
	#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		private const string libname = "libbrotli";
	#endif
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX
	#if (UNITY_STANDALONE_OSX  || UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)&& !UNITY_EDITOR_WIN
			[DllImport(libname, EntryPoint = "setPermissions")]
			internal static extern int setPermissions(string filePath, string _user, string _group, string _other);
		#endif

        [DllImport(libname, EntryPoint = "brCompress"
        #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		, CallingConvention = CallingConvention.Cdecl
        #endif
        )]
		internal static extern int brCompress(string inFile, string outFile, IntPtr proc, int quality, int lgwin, int lgblock, int mode);

        [DllImport(libname, EntryPoint = "brDecompresss"
        #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		, CallingConvention = CallingConvention.Cdecl
        #endif
        )]
		internal static extern int brDecompresss(string inFile, string outFile, IntPtr proc, IntPtr FileBuffer, int fileBufferLength);

        [DllImport(libname, EntryPoint = "brReleaseBuffer"
        #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		, CallingConvention = CallingConvention.Cdecl
        #endif
        )]
        public static extern void brReleaseBuffer(IntPtr buffer);

        [DllImport(libname, EntryPoint = "brCreate_Buffer"
        #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		    , CallingConvention = CallingConvention.Cdecl
        #endif
        )]
        public static extern IntPtr brCreate_Buffer(int size);

        [DllImport(libname, EntryPoint = "brAddTo_Buffer"
        #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		    , CallingConvention = CallingConvention.Cdecl
        #endif
        )]
        private static extern void brAddTo_Buffer(IntPtr destination, int offset, IntPtr buffer, int len);

        [DllImport(libname, EntryPoint = "brCompressBuffer"
        #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		, CallingConvention = CallingConvention.Cdecl
        #endif
        )]
        internal static extern IntPtr brCompressBuffer( int bufferLength, IntPtr buffer, IntPtr encodedSize, IntPtr proc, int quality, int lgwin, int lgblock, int mode);

		//this will work on small files with one meta block
        [DllImport(libname, EntryPoint = "brGetDecodedSize"
        #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		, CallingConvention = CallingConvention.Cdecl
        #endif
        )]
        internal static extern int brGetDecodedSize(int bufferLength, IntPtr buffer);

        [DllImport(libname, EntryPoint = "brDecompressBuffer"
        #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		, CallingConvention = CallingConvention.Cdecl
        #endif
        )]
        internal static extern int brDecompressBuffer(int bufferLength, IntPtr buffer, int outLength, IntPtr outbuffer);
#endif

#if (UNITY_IOS || UNITY_TVOS || UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR

#if !UNITY_TVOS && !UNITY_WEBGL
		[DllImport("__Internal")]
		internal static extern int setPermissions(string filePath, string _user, string _group, string _other);
		[DllImport("__Internal")]
		internal static extern int brCompress( string inFile, string outFile, IntPtr proc, int quality, int lgwin, int lgblock, int mode);
        [DllImport("__Internal")]
		internal static extern int brDecompresss(string inFile, string outFile, IntPtr proc, IntPtr FileBuffer, int fileBufferLength);
        
#endif
        #if !UNITY_WEBGL
        [DllImport("__Internal")]
        public static extern IntPtr brCreate_Buffer(int size);
        [DllImport("__Internal")]
        internal static extern void brAddTo_Buffer(IntPtr destination, int offset, IntPtr buffer, int len);
        [DllImport("__Internal")]
        internal static extern IntPtr brCompressBuffer( int bufferLength, IntPtr buffer, IntPtr encodedSize, IntPtr proc, int quality, int lgwin, int lgblock, int mode);
        #endif

        [DllImport("__Internal")]
		public static extern void brReleaseBuffer(IntPtr buffer);
		//this will work on small files with one meta block
		[DllImport("__Internal")]
		internal static extern int brGetDecodedSize(int bufferLength, IntPtr buffer);
        [DllImport("__Internal")]
        internal static extern int brDecompressBuffer(int bufferLength, IntPtr buffer,  int outLength, IntPtr outbuffer);

#endif

#if (!UNITY_TVOS && !UNITY_WEBGL) || UNITY_EDITOR
    // set permissions of a file in user, group, other.
    // Each string should contain any or all chars of "rwx".
    // returns 0 on success
    public static int setFilePermissions(string filePath, string _user, string _group, string _other){
#if (UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX || UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR_WIN
            if(!File.Exists(filePath)) return -1;
			return setPermissions(filePath, _user, _group, _other);
#else
			return -1;
#endif
	}


    //Helper functions
    internal static GCHandle gcA(object o) {
        return GCHandle.Alloc(o, GCHandleType.Pinned);
    }

    private static bool checkObject(object fileBuffer, string filePath, ref GCHandle fbuf, ref IntPtr fileBufferPointer, ref int fileBufferLength) {
		if(fileBuffer is byte[]) { byte[] tempBuf = (byte[])fileBuffer; fbuf = gcA(tempBuf); fileBufferPointer = fbuf.AddrOfPinnedObject(); fileBufferLength = tempBuf.Length; return true; }
		if(fileBuffer is IntPtr) { fileBufferPointer = (IntPtr)fileBuffer; fileBufferLength = Convert.ToInt32(filePath); }
        return false;
    }

    // Compress a file to brotli format.
    //
    // Full paths to the files should be provided.
	// inFile:		The input file
	// outFile:		The output file
	// proc:		A single item ulong array to provide progress of compression
	//
    // quality:    (0  - 11) quality of compression (0 = faster/bigger - 11 = slower/smaller).
	//
	// Base 2 logarithm of the sliding window size. Range is 10 to 24.
	// lgwin  :    (10 - 24) memory used for compression (higher numbers use more ram)
	//
	// Base 2 logarithm of the maximum input block size. Range is 16 to 24.
	// If set to 0, the value will be set based on the quality.
	// lgblock:    0 for auto or 16-24
	// mode   :	  (0  -  2) 0 = default, 1 = utf8 text, 2 = woff 2.0 font
    //
	// error codes:	 1  : OK
	//				-1  : compression failed
	//				-2  : not enough memory
	//				-3  : could not close in file
	//				-4  : could not close out file
    //              -5  : no input file found
	//
    public static int compressFile(string inFile, string outFile,  ulong[] proc, int quality = 9, int lgwin = 19, int lgblock = 0, int mode = 0) {
        if(!File.Exists(inFile)) return -5;
        if (quality < 0) quality = 1; if (quality > 11) quality = 11;
		if (lgwin < 10) lgwin = 10; if(lgwin > 24) lgwin = 24;
        if(proc == null) proc = new ulong[1];
		GCHandle cbuf = GCHandle.Alloc(proc, GCHandleType.Pinned);
        int res = brCompress( @inFile, @outFile,cbuf.AddrOfPinnedObject(), quality, lgwin, lgblock, mode);
		cbuf.Free();
		return res;
    }

    // Decompress a brotli file.
    //
    // Full paths to the files should be provided.
	// inFile:		The input file
	// outFile:		The output file
	// proc:		A single item ulong array to provide progress of decompression
	// FileBuffer:	A buffer that holds a brotli file. When assigned the function will read from this buffer and will ignore the filePath. (Linux, iOS, Android, MacOSX)
    // returns: 1 on success.
	//
	// error codes:	 1  : OK
	//				-1  : failed to write output
	//				-2  : corrupt input
	//				-3  : could not close in file
	//				-4  : could not close out file
    //              -5  : no input file found
	//
    public static int decompressFile(string inFile, string outFile, ulong[] proc, object fileBuffer = null) {
        if(fileBuffer == null && !File.Exists(inFile)) return -5;
        if(proc == null) proc = new ulong[1];
		GCHandle cbuf = GCHandle.Alloc(proc, GCHandleType.Pinned);
		int res;

        #if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
            if(fileBuffer != null) {
                int fileBufferLength = 0;
                IntPtr fileBufferPointer = IntPtr.Zero;
                GCHandle fbuf;
                bool managed = checkObject(fileBuffer, inFile, ref fbuf, ref fileBufferPointer, ref fileBufferLength);
                
                if (!managed && fileBufferLength == 0) { Debug.Log("Please provide a valid native buffer size as a string in filePath"); return -5; }

				res = brDecompresss(null, @outFile, cbuf.AddrOfPinnedObject(), fileBufferPointer, fileBufferLength);

				if (managed) fbuf.Free();
                cbuf.Free();
				return res;
			}
        #endif

		res = brDecompresss(@inFile, @outFile, cbuf.AddrOfPinnedObject(), IntPtr.Zero, 0);
		cbuf.Free();
		return res;
    }

#endif



    // Get the uncompressed size of a brotli buffer.
    // This will work only on small buffers with one metablock. Otherwise use the includeSize/hasFooter flags with the buffer functions.
    //
    // inBuffer:	the input buffer that stores a brotli compressed buffer.
    public static int getDecodedSize(byte[] inBuffer)
    {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        int res = brGetDecodedSize(inBuffer.Length, cbuf.AddrOfPinnedObject());
        cbuf.Free();
        return res;
    }

#if !UNITY_WEBGL || UNITY_EDITOR
    // Compress a byte buffer in brotli format.
    //
    // inBuffer:     the uncompressed buffer.
    // outBuffer:    a referenced buffer that will store the compressed data. (it should be large enough to store it.)
    // proc:		 A single item referenced ulong array to provide progress of compression
    //
    // includeSize:  include the uncompressed size of the buffer in the resulted compressed one because brotli does not support it for larger then 1 metablock.
    //
    // quality:    (0  - 11) quality of compression (0 = faster/bigger - 11 = slower/smaller).
    //
    // Base 2 logarithm of the sliding window size. Range is 10 to 24.
    // lgwin  :    (10 - 24) memory used for compression (higher numbers use more ram)
    //
    // Base 2 logarithm of the maximum input block size. Range is 16 to 24.
    // If set to 0, the value will be set based on the quality.
    // lgblock:    0 for auto or 16-24
    // mode   :	  (0  -  2) 0 = default, 1 = utf8 text, 2 = woff 2.0 font
    // returns true on success
    //
    public static bool compressBuffer(byte[] inBuffer, ref byte[] outBuffer, ulong[] proc, bool includeSize = false, int quality = 9, int lgwin = 19, int lgblock = 0, int mode = 0) {
	    if (quality < 0) quality = 1; if (quality > 11) quality = 11;
		if (lgwin < 10) lgwin = 10; if(lgwin > 24) lgwin = 24;

        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int  size = 0;
		byte[] bsiz = null;
		int[] esiz = new int[1];//the compressed size
		GCHandle ebuf = GCHandle.Alloc(esiz, GCHandleType.Pinned);

        //if the uncompressed size of the buffer should be included. This is a hack since brotli lib does not support this on larger buffers.
        if (includeSize) {
			bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!BitConverter.IsLittleEndian) Array.Reverse(bsiz);
        }

        if(proc == null) proc = new ulong[1];
		GCHandle pbuf = GCHandle.Alloc(proc, GCHandleType.Pinned);

        ptr = brCompressBuffer(inBuffer.Length, cbuf.AddrOfPinnedObject(), ebuf.AddrOfPinnedObject(), pbuf.AddrOfPinnedObject(), quality, lgwin, lgblock, mode);

        cbuf.Free(); ebuf.Free(); pbuf.Free();

        if (ptr == IntPtr.Zero) { brReleaseBuffer(ptr); esiz = null; bsiz = null; return false; }

        System.Array.Resize(ref outBuffer, esiz[0] + size);

		//add the uncompressed size to the buffer
        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[i + esiz[0]] = bsiz[i]; }

        Marshal.Copy( ptr, outBuffer, 0, esiz[0] );

        brReleaseBuffer(ptr);
		esiz = null;
		bsiz = null;

        return true;
    }

	// same as above only this function returns a new created buffer with the compressed data.
	//
    public static byte[] compressBuffer(byte[] inBuffer,  int[] proc, bool includeSize = false, int quality = 9, int lgwin = 19, int lgblock = 0, int mode = 0) {
	    if (quality < 0) quality = 1; if (quality > 11) quality = 11;
		if (lgwin < 10) lgwin = 10; if(lgwin > 24) lgwin = 24;

        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int  size = 0;
		byte[] bsiz = null;
		int[] esiz = new int[1];//the compressed size
		GCHandle ebuf = GCHandle.Alloc(esiz, GCHandleType.Pinned);

        // if the uncompressed size of the buffer should be included. This is a hack since brotli lib does not support this on larger buffers.
        if (includeSize) {
			bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!BitConverter.IsLittleEndian) Array.Reverse(bsiz);
        }

        if(proc == null) proc = new int[1];
		GCHandle pbuf = GCHandle.Alloc(proc, GCHandleType.Pinned);

        ptr = brCompressBuffer(inBuffer.Length, cbuf.AddrOfPinnedObject(), ebuf.AddrOfPinnedObject(), pbuf.AddrOfPinnedObject(), quality, lgwin, lgblock, mode);

        cbuf.Free(); ebuf.Free(); pbuf.Free();

        if (ptr == IntPtr.Zero) { brReleaseBuffer(ptr); esiz = null; bsiz = null; return null; }

		byte[] outBuffer = new byte[esiz[0] + size];

		//add the uncompressed size to the buffer
        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[i + esiz[0]] = bsiz[i]; }

        Marshal.Copy( ptr, outBuffer, 0, esiz[0] );

        brReleaseBuffer(ptr);
		esiz = null;
		bsiz = null;

        return outBuffer;
    }

	// Same as above, only this time the compressed buffer is written in a fixed size buffer.
	// The compressed size in bytes is returned. If includeSize is true, then 4 extra bytes are written at the end of the buffer.
	// The fixed size buffer should be larger then the compressed size returned.
    //
    public static int compressBuffer(byte[] inBuffer, byte[] outBuffer, int[] proc, bool includeSize = false, int quality = 9, int lgwin = 19, int lgblock = 0, int mode = 0) {
	    if (quality < 0) quality = 1; if (quality > 11) quality = 11;
		if (lgwin < 10) lgwin = 10; if(lgwin > 24) lgwin = 24;

        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int  size = 0;
        byte[] bsiz = null;
		int res = 0;
		int[] esiz = new int[1];//the compressed size
		GCHandle ebuf = GCHandle.Alloc(esiz, GCHandleType.Pinned);

        // if the uncompressed size of the buffer should be included. This is a hack since brotli lib does not support this on larger buffers.
        if (includeSize) {
			bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!BitConverter.IsLittleEndian) Array.Reverse(bsiz);
        }

        if(proc == null) proc = new int[1];
		GCHandle pbuf = GCHandle.Alloc(proc, GCHandleType.Pinned);

        ptr = brCompressBuffer(inBuffer.Length, cbuf.AddrOfPinnedObject(), ebuf.AddrOfPinnedObject(), pbuf.AddrOfPinnedObject(), quality, lgwin, lgblock, mode);

        cbuf.Free(); ebuf.Free(); pbuf.Free();
		res = esiz[0];

        if (ptr == IntPtr.Zero || outBuffer.Length < (esiz[0] + size)) { brReleaseBuffer(ptr); esiz = null; bsiz = null; return 0; }

        Marshal.Copy( ptr, outBuffer, 0, esiz[0] );

        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[i + esiz[0]] = bsiz[i]; }

        brReleaseBuffer(ptr);
		esiz = null;
        bsiz = null;

        return res + size;
    }

#endif

    // Decompress a brotli compressed buffer to a referenced buffer.
    //
    // inBuffer:            the brotli compressed buffer
    // outBuffer:           a referenced buffer that will be resized to store the uncompressed data.
    // useFooter:           if the input Buffer has the uncompressed size info.
    // unCompressedSize:    if unCompressedSize is > 0 then this is the uncompressed size that will be used. Useful when decompressing brotli buffers created from servers.
    //
    // returns true on success
    //
    public static bool decompressBuffer(byte[] inBuffer, ref byte[] outBuffer, bool useFooter = false, int unCompressedSize = 0) {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
		int uncompressedSize = 0, res2 = inBuffer.Length;

        if (unCompressedSize == 0) { 
            if (useFooter) {
                res2 -= 4;
                uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
            } else {
			    //use the brotli native method to get the uncompressed size (this will work on buffers with one metablock)
                uncompressedSize = getDecodedSize(inBuffer); 
            }
        } else {
            uncompressedSize = unCompressedSize;
        }

        System.Array.Resize(ref outBuffer, uncompressedSize);

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

        int res = brDecompressBuffer(inBuffer.Length, cbuf.AddrOfPinnedObject(), uncompressedSize, obuf.AddrOfPinnedObject());

        cbuf.Free();
        obuf.Free();

        if(res == 1) return true; else return false;
    }

	// same as above only this time the uncompressed data is returned in a new created buffer
	//
    public static byte[] decompressBuffer(byte[] inBuffer, bool useFooter = false, int unCompressedSize = 0) {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
		int uncompressedSize = 0, res2 = inBuffer.Length;

        if (unCompressedSize == 0) {
            if (useFooter) {
                res2 -= 4;
                uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
            } else {
			    //use the brotli native method to get the uncompressed size (this will work on buffers with one metablock)
                uncompressedSize = getDecodedSize(inBuffer); 
            }
        } else {
            uncompressedSize = unCompressedSize;
        }

        byte[] outBuffer = new byte[uncompressedSize];

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

        int res = brDecompressBuffer(inBuffer.Length, cbuf.AddrOfPinnedObject(), uncompressedSize, obuf.AddrOfPinnedObject());

        cbuf.Free();
        obuf.Free();

        if(res == 1) return outBuffer; else return null;
    }


	// same as above only the decompressed data will be stored in a fixed size outBuffer.
	// make sure the fixed buffer is big enough to store the data.
	//
	// returns: uncompressed size in bytes.
	//
    public static int decompressBuffer(byte[] inBuffer, byte[] outBuffer, bool useFooter = false, int unCompressedSize = 0) {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
		int uncompressedSize = 0, res2 = inBuffer.Length;

        if (unCompressedSize == 0) {
            if (useFooter) {
                res2 -= 4;
                uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
            } else {
                //use the brotli native method to get the uncompressed size (this will work on buffers with one metablock)
                uncompressedSize = getDecodedSize(inBuffer);
            }
        } else {
            uncompressedSize = unCompressedSize;
        }

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

        int res = brDecompressBuffer(inBuffer.Length, cbuf.AddrOfPinnedObject(), uncompressedSize, obuf.AddrOfPinnedObject());

        cbuf.Free();
        obuf.Free();

        if(res == 1) return uncompressedSize; else return 0;
    }

    #if !UNITY_WEBGL && !UNITY_TVOS
    // A reusable native memory pointer for downloading files.
    public static  IntPtr nativeBuffer = IntPtr.Zero;
    public static bool nativeBufferIsBeingUsed = false;
    public static int nativeOffset = 0;

    // A Coroutine to dowload a file to a native/unmaged memory buffer.
    // You can call it for an IntPtr.
    // 
    //
    // This function can only be called for one file at a time. Don't use it to call multiple files at once.
    // 
    // This is useful to avoid memory spikes when downloading large files and intend to decompress from memory.
    // With the old method, a copy of the downloaded file to memory would be produced by pinning the buffer to memory.
    // Now with this method, it is downloaded to memory and can be manipulated with no memory spikes.
    //
    // In any case, if you don't need the created in-Memory file, you should use the brotli.brReleaseBuffer function to free the memory!
    //
    // Parameters:
    //
    // url:             The url of the file you want to download to a native memory buffer.
    // downloadDone:    Informs a bool that the download of the file to memory is done.
    // pointer:         An IntPtr for a native memory buffer
    // fileSize:        The size of the downloaded file will be returned here.
    public static IEnumerator downloadBrFileNative(string url, Action<bool> downloadDone, Action<IntPtr> pointer = null, Action<int> fileSize = null) {
        // Get the file lenght first, so we create a correct size native memory buffer.
        UnityWebRequest wr = UnityWebRequest.Head(url);

        nativeBufferIsBeingUsed = true;

        yield return wr.SendWebRequest();
        string size = wr.GetResponseHeader("Content-Length");

        nativeBufferIsBeingUsed = false;

        #if UNITY_2020_1_OR_NEWER
        if (wr.result ==  UnityWebRequest.Result.ConnectionError || wr.result == UnityWebRequest.Result.ProtocolError) {
        #else
        if (wr.isNetworkError || wr.isHttpError) {
        #endif
            Debug.LogError("Error While Getting Length: " + wr.error);
        } else {
            if (!nativeBufferIsBeingUsed) { 

                //get the size of the zip
                int zipSize = Convert.ToInt32(size);

                // If the zip size is larger then 0
                if (zipSize > 0) {

                    nativeBuffer = brCreate_Buffer(zipSize);
                    nativeBufferIsBeingUsed = true;

                    // buffer for the download
                    byte[] bytes = new byte[2048];
                    nativeOffset = 0;

                    using (UnityWebRequest wwwSK = UnityWebRequest.Get(url)) {

                        // Here we call our custom webrequest function to download our archive to a native memory buffer.
                        wwwSK.downloadHandler = new CustomWebRequest5(bytes);
                        
                        yield return wwwSK.SendWebRequest();

                        if (wwwSK.error != null) {
                            Debug.Log(wwwSK.error);
                        } else {
                            downloadDone(true);

                            if(pointer != null) { pointer(nativeBuffer); fileSize(zipSize); }

                            //reset intermediate buffer params.
                            nativeBufferIsBeingUsed = false;
                            nativeOffset = 0;
                            nativeBuffer = IntPtr.Zero;

                            //Debug.Log("Custom download done");
                        }
                    }
                    
                }

            } else { Debug.LogError("Native buffer is being used, or not yet freed!"); }
        }
    }


    // A custom WebRequest Override to download data to a native-unmanaged memory buffer.
    public class CustomWebRequest5 : DownloadHandlerScript {

        public CustomWebRequest5()
            : base()
        {
        }

        public CustomWebRequest5(byte[] buffer)
            : base(buffer)
        {
        }

        protected override byte[] GetData() { return null; }


        protected override bool ReceiveData(byte[] bytesFromServer, int dataLength) {
            if (bytesFromServer == null || bytesFromServer.Length < 1) {
                Debug.Log("CustomWebRequest: Received a null/empty buffer");
                return false;
            }

            var pbuf = gcA(bytesFromServer);
            
            //Process byteFromServer
            brAddTo_Buffer(nativeBuffer, nativeOffset, pbuf.AddrOfPinnedObject(), dataLength );
            nativeOffset += dataLength;
            pbuf.Free();
            
            return true;
        }

        // Use the below functions only when needed. You get the same functionality from the main coroutine.
        /*
        // If all data has been received from the server
        protected override void CompleteContent()
        {
            //Debug.Log(Download Complete.");
        }

        // If a Content-Length header is received from the server.
        protected override void ReceiveContentLength(int fileLength)
        {
             //Debug.Log("ReceiveContentLength: " + fileLength);
        }
        */
    }
    #endif
#endif
}

