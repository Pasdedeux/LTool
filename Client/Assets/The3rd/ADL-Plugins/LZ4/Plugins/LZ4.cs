using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LZ4
{
#if !UNITY_WEBPLAYER  || UNITY_EDITOR

    internal static bool isle = BitConverter.IsLittleEndian;

#if UNITY_5_4_OR_NEWER
	#if (UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_WEBGL) && !UNITY_EDITOR || UNITY_EDITOR_LINUX
		private const string libname = "lz4";
	#elif UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		private const string libname = "liblz4";
	#endif
#else
	#if (UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_WEBGL) && !UNITY_EDITOR
		private const string libname = "lz4";
	#endif
	#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		private const string libname = "liblz4";
	#endif
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX
	#if (!UNITY_WEBGL || UNITY_EDITOR)
		#if (UNITY_STANDALONE_OSX  || UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)&& !UNITY_EDITOR_WIN
			[DllImport(libname, EntryPoint = "z4setPermissions")]
			internal static extern int z4setPermissions(string filePath, string _user, string _group, string _other);
		#endif
        [DllImport(libname, EntryPoint = "LZ4DecompressFile"
            #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		    , CallingConvention = CallingConvention.Cdecl
            #endif
        )]
		internal static extern int LZ4DecompressFile(string inFile, string outFile, IntPtr bytes, IntPtr FileBuffer, int fileBufferLength);

        [DllImport(libname, EntryPoint = "LZ4CompressFile"
            #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		    , CallingConvention = CallingConvention.Cdecl
            #endif
        )]
		internal static extern int LZ4CompressFile(string inFile, string outFile, int level, IntPtr percentage, ref float rate);
	#endif

        [DllImport(libname, EntryPoint = "LZ4releaseBuffer"
            #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		    , CallingConvention = CallingConvention.Cdecl
            #endif
        )]
        public static extern void LZ4releaseBuffer(IntPtr buffer);

        [DllImport(libname, EntryPoint = "LZ4Create_Buffer"
        #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		    , CallingConvention = CallingConvention.Cdecl
        #endif
        )]
        public static extern IntPtr LZ4Create_Buffer(int size);

        [DllImport(libname, EntryPoint = "LZ4AddTo_Buffer"
        #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		    , CallingConvention = CallingConvention.Cdecl
        #endif
        )]
        private static extern void LZ4AddTo_Buffer(IntPtr destination, int offset, IntPtr buffer, int len);

        [DllImport(libname, EntryPoint = "LZ4CompressBuffer"
            #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		    , CallingConvention = CallingConvention.Cdecl
            #endif
        )]
        internal static extern IntPtr LZ4CompressBuffer(IntPtr buffer, int bufferLength, ref int v, int level);

        [DllImport(libname, EntryPoint = "LZ4DecompressBuffer"
            #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		    , CallingConvention = CallingConvention.Cdecl
            #endif
        )]
        internal static extern int LZ4DecompressBuffer(IntPtr buffer, IntPtr outbuffer, int bufferLength);
#endif


#if (UNITY_IOS || UNITY_TVOS || UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
	#if (UNITY_IOS || UNITY_IPHONE) && !UNITY_WEBGL
		[DllImport("__Internal")]
		private static extern int z4setPermissions(string filePath, string _user, string _group, string _other);
		[DllImport("__Internal")]
		private static extern int LZ4DecompressFile(string inFile, string outFile, IntPtr bytes, IntPtr FileBuffer, int fileBufferLength);
       	[DllImport("__Internal")]
		private static extern int LZ4CompressFile(string inFile, string outFile, int level, IntPtr percentage, ref float rate);
	#endif
	#if (UNITY_IOS || UNITY_TVOS || UNITY_IPHONE || UNITY_WEBGL)
        [DllImport("__Internal")]
		public static extern void LZ4releaseBuffer(IntPtr buffer);
        [DllImport("__Internal")]
        public static extern IntPtr LZ4Create_Buffer(int size);
        [DllImport("__Internal")]
        internal static extern void LZ4AddTo_Buffer(IntPtr destination, int offset, IntPtr buffer, int len);
       	[DllImport("__Internal")]
        private static extern IntPtr LZ4CompressBuffer(IntPtr buffer, int bufferLength, ref int v, int level);
       	[DllImport("__Internal")]
        private static extern int LZ4DecompressBuffer(IntPtr buffer, IntPtr outbuffer, int bufferLength);
	#endif
#endif

    //Helper function
    internal static GCHandle gcA(object o) {
        return GCHandle.Alloc(o, GCHandleType.Pinned);
    }

#if ((!UNITY_WEBGL && !UNITY_TVOS) || UNITY_EDITOR)
	// set permissions of a file in user, group, other.
	// Each string should contain any or all chars of "rwx".
	// returns 0 on success
	public static int setFilePermissions(string filePath, string _user, string _group, string _other){
		#if (UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX || UNITY_IOS || UNITY_TVOS || UNITY_IPHONE) && !UNITY_EDITOR_WIN
			return z4setPermissions(filePath, _user, _group, _other);
		#else
			return -1;
		#endif
	}

    //Helper function
    private static bool checkObject(object fileBuffer, string filePath, ref GCHandle fbuf, ref IntPtr fileBufferPointer, ref int fileBufferLength) {
		if(fileBuffer is byte[]) { byte[] tempBuf = (byte[])fileBuffer; fbuf = gcA(tempBuf); fileBufferPointer = fbuf.AddrOfPinnedObject(); fileBufferLength = tempBuf.Length; return true; }
		if(fileBuffer is IntPtr) { fileBufferPointer = (IntPtr)fileBuffer; fileBufferLength = Convert.ToInt32(filePath); }
        return false;
    }

    //Compress a file to LZ4.
    //
    //Full paths to the files should be provided.
    //level: level of compression (1 - 9).
    //returns: rate of compression.
    //progress: provide a single item float array to get the progress of the compression in real time. (only when called from a thread/task)
    //
    public static float compress(string inFile, string outFile, int level, float[] progress) {
        if (level < 1) level = 1;
        if (level > 9) level = 9;
        float rate = 0;
        if(progress == null) progress = new float[1];
        progress[0] = 0;
		GCHandle ibuf = GCHandle.Alloc(progress, GCHandleType.Pinned);
		
        int res =  LZ4CompressFile(@inFile, @outFile, level, ibuf.AddrOfPinnedObject(), ref rate);
        
        ibuf.Free();
        if (res != 0) return -1;
        return rate;
    }

    //Decompress an LZ4 file.
    //
    //Full paths to the files should be provided.
    //returns: 0 on success.
    //bytes: provide a single item ulong array to get the bytes currently decompressed in real time.  (only when called from a thread/task)
	// fileBuffer		: A buffer that holds an LZ4 file. When assigned the function will decompress from this buffer and will ignore the filePath. (iOS, Android, MacOSX, Linux)
    //                  : It can be a byte[] buffer or a native IntPtr buffer (downloaded using the helper function: downloadLZ4FileNative)
    //                  : When an IntPtr is used as the input buffer, the size of it must be passed to the function as a string with the inFile parameter!
    //
    public static int decompress(string inFile, string outFile, ulong[] bytes, object fileBuffer = null) {
        if(bytes == null) bytes = new ulong[1];
        bytes[0] = 0;
		int res = 0;
		GCHandle ibuf = GCHandle.Alloc(bytes , GCHandleType.Pinned);
		
		#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
            if(fileBuffer != null) {
                int fileBufferLength = 0;
                IntPtr fileBufferPointer = IntPtr.Zero;
                GCHandle fbuf;
                bool managed = checkObject(fileBuffer, inFile, ref fbuf, ref fileBufferPointer, ref fileBufferLength);
                
                if (!managed && fileBufferLength == 0) { Debug.Log("Please provide a valid native buffer size as a string in filePath"); return -5; }

				res = LZ4DecompressFile(null, @outFile, ibuf.AddrOfPinnedObject(), fileBufferPointer, fileBufferLength);

				if (managed) fbuf.Free();
                ibuf.Free();
				return res;
			}
		#endif
		res =LZ4DecompressFile(@inFile, @outFile, ibuf.AddrOfPinnedObject(), IntPtr.Zero, 0);
		ibuf.Free();
        return res;
    }
	 
#endif




    //Compress a byte buffer in LZ4 format.
    //
    //inBuffer: the uncompressed buffer.
    //outBuffer: a referenced buffer that will be resized to fit the lz4 compressed data.
	//level: level of compression (1 - 9).
    //includeSize: include the uncompressed size of the buffer in the resulted compressed one because lz4 does not include this.
    //returns true on success
	//
    public static bool compressBuffer(byte[] inBuffer, ref byte[] outBuffer, int level, bool includeSize = true) {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int res = 0, size = 0;
		byte[] bsiz = null;

        //if the uncompressed size of the buffer should be included. This is a hack since lz4 lib does not support this.
        if (includeSize){
			bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!isle) Array.Reverse(bsiz);
        }

        if (level < 1) level = 1;
        if (level > 9) level = 9;

        ptr = LZ4CompressBuffer(cbuf.AddrOfPinnedObject(), inBuffer.Length, ref res, level);

        cbuf.Free();

        if (res == 0 || ptr == IntPtr.Zero) { LZ4releaseBuffer(ptr); return false; }

        System.Array.Resize(ref outBuffer, res + size);

        //add the uncompressed size to the buffer
        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[i+res] = bsiz[i];  /*Debug.Log(BitConverter.ToInt32(bsiz, 0));*/ }

        Marshal.Copy(ptr, outBuffer, 0, res  );

        LZ4releaseBuffer(ptr);
		bsiz = null;
        return true;
    }



    //Compress a byte buffer in LZ4 format and return a new buffer compressed.
    //
    //inBuffer: the uncompressed buffer.
	//level: level of compression (1 - 9).
    //includeSize: include the uncompressed size of the buffer in the resulted compressed one because lz4 does not include this.
	//returns: a new buffer with the compressed data.
    //
    public static byte[] compressBuffer(byte[] inBuffer, int level, bool includeSize = true) {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int res = 0, size = 0;
		byte[] bsiz = null;

        //if the uncompressed size of the buffer should be included. This is a hack since lz4 lib does not support this.
        if (includeSize){
			bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!isle) Array.Reverse(bsiz);
        }

        if (level < 1) level = 1;
        if (level > 9) level = 9;

        ptr = LZ4CompressBuffer(cbuf.AddrOfPinnedObject(), inBuffer.Length, ref res, level);
        cbuf.Free();

        if (res == 0 || ptr == IntPtr.Zero) { LZ4releaseBuffer(ptr); return null; }

        byte[] outBuffer = new byte[res + size];

        //add the uncompressed size to the buffer
        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[i + res] = bsiz[i];  /*Debug.Log(BitConverter.ToInt32(bsiz, 0));*/ }

        Marshal.Copy(ptr, outBuffer, 0, res);

        LZ4releaseBuffer(ptr);
		bsiz = null;

        return outBuffer;
    }


	//Compress a byte buffer in LZ4 format at a specific position of a fixed size outBuffer
	//
	//inBuffer: the uncompressed buffer.
	//outBuffer: a referenced buffer of fixed size that could have already some lz4 compressed buffers stored.
	//outBufferPartialIndex: the position at which the compressed data will be written to.
	//level: level of compression (1 - 9).
	//
	//returns compressed size (+4 bytes if footer is used)
	//
	public static int compressBufferPartialFixed (byte[] inBuffer, ref byte[] outBuffer,int outBufferPartialIndex, int level, bool includeSize = true) {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int res = 0, size = 0;
		byte[] bsiz = null;

        //if the uncompressed size of the buffer should be included. This is a hack since lz4 lib does not support this.
        if (includeSize){
			bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!isle) Array.Reverse(bsiz);
        }

        if (level < 1) level = 1;
        if (level > 9) level = 9;

        ptr = LZ4CompressBuffer(cbuf.AddrOfPinnedObject(), inBuffer.Length, ref res, level);

        cbuf.Free();

        if (res == 0 || ptr == IntPtr.Zero) { LZ4releaseBuffer(ptr); return 0; }

        //add the uncompressed size to the buffer
        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[outBufferPartialIndex + res + i ] = bsiz[i];   }

        Marshal.Copy(ptr, outBuffer, outBufferPartialIndex, res );

        LZ4releaseBuffer(ptr);
		bsiz = null;
        return res + size;
    }


	//compressedBufferSize: compressed size of the buffer to be decompressed 
	//partialIndex: position of an lz4 compressed buffer
	//
	//returns the uncompressed size
	public static int decompressBufferPartialFixed (byte[] inBuffer, ref byte[] outBuffer, int partialIndex , int compressedBufferSize, bool safe = true, bool useFooter = true, int customLength = 0) {

        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);

        int uncompressedSize = 0;

		//be carefull with this. You must know exactly where your compressed data lies in the inBuffer
		int res2 = partialIndex + compressedBufferSize;


        //if the hacked in LZ4 footer will be used to extract the uncompressed size of the buffer. If the buffer does not have a footer 
        //provide the known uncompressed size through the customLength integer.
        if (useFooter){
            res2 -= 4;
            uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
        }
        else{
            uncompressedSize = customLength; 
        }

		//Check if the uncompressed size is bigger then the size of the fixed buffer. Then:
		//1. write only the data that fit in it.
		//2. or return a negative number. 
		//It depends on if we set the safe flag to true or not.
		if(uncompressedSize > outBuffer.Length) {
			if(safe) return -101;  else  uncompressedSize = outBuffer.Length;
		 }

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

		IntPtr ptrPartial;
		ptrPartial = new IntPtr(cbuf.AddrOfPinnedObject().ToInt64() + partialIndex);

        //res should be the compressed size
        LZ4DecompressBuffer(ptrPartial,  obuf.AddrOfPinnedObject(), uncompressedSize);

        cbuf.Free();
        obuf.Free();

        return uncompressedSize;
    }


    //Decompress an lz4 compressed buffer to a referenced buffer.
    //
    //inBuffer: the lz4 compressed buffer
    //outBuffer: a referenced buffer that will be resized to store the uncompressed data.
    //useFooter: if the input Buffer has the uncompressed size info.
    //customLength: provide the uncompressed size of the compressed buffer. Not needed if the usefooter is used!
    //returns true on success
    //
    public static bool decompressBuffer(byte[] inBuffer, ref byte[] outBuffer, bool useFooter = true, int customLength = 0) {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        int uncompressedSize = 0, res2 = inBuffer.Length;

        //if the hacked in LZ4 footer will be used to extract the uncompressed size of the buffer. If the buffer does not have a footer 
        //provide the known uncompressed size through the customLength integer.
        if (useFooter){
            res2 -= 4;
            uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
        }
        else{
            uncompressedSize = customLength; 
        }

        System.Array.Resize(ref outBuffer, uncompressedSize);

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

        //res should be the compressed size
        int res = LZ4DecompressBuffer(cbuf.AddrOfPinnedObject(),  obuf.AddrOfPinnedObject(), uncompressedSize);

        cbuf.Free();
        obuf.Free();

        if (res != res2) { /*Debug.Log("ERROR: " + res.ToString());*/ return false; }

        return true;
    }

	//Decompress an lz4 compressed buffer to a referenced fixed size buffer.
    //
    //inBuffer: the lz4 compressed buffer
    //outBuffer: a referenced fixed size buffer where the data will get decompressed
    //usefooter: if the input Buffer has the uncompressed size info.
    //customLength: provide the uncompressed size of the compressed buffer. Not needed if the usefooter is used!
    //returns uncompressedSize
    //
	public static int decompressBufferFixed(byte[] inBuffer, ref byte[] outBuffer, bool safe = true, bool useFooter = true, int customLength = 0) {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        int uncompressedSize = 0, res2 = inBuffer.Length;

        //if the hacked in LZ4 footer will be used to extract the uncompressed size of the buffer. If the buffer does not have a footer 
        //provide the known uncompressed size through the customLength integer.
        if (useFooter){
            res2 -= 4;
            uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
        }
        else{
            uncompressedSize = customLength; 
        }

		//Check if the uncompressed size is bigger then the size of the fixed buffer. Then:
		//1. write only the data that fit in it.
		//2. or return a negative number. 
		//It depends on if we set the safe flag to true or not.
		if(uncompressedSize > outBuffer.Length) {
			if(safe) return -101;  else  uncompressedSize = outBuffer.Length;
		 }

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

        //res should be the compressed size
        int res = LZ4DecompressBuffer(cbuf.AddrOfPinnedObject(),  obuf.AddrOfPinnedObject(), uncompressedSize);

        cbuf.Free();
        obuf.Free();

		if(safe) {
			if (res != res2) { /*Debug.Log("ERROR: " + res.ToString());*/ return -101; }
		}

        return uncompressedSize;
    }


    //Decompress an lz4 compressed buffer to a new buffer.
    //
    //inBuffer: the lz4 compressed buffer
    //useFooter: if the input Buffer has the uncompressed size info.
    //customLength: provide the uncompressed size of the compressed buffer. Not needed if the usefooter is used!
	//returns: a new buffer with the uncompressed data.
    //
    public static byte[] decompressBuffer(byte[] inBuffer, bool useFooter = true, int customLength = 0) {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        int uncompressedSize = 0, res2 = inBuffer.Length;

        //if the hacked in LZ4 footer will be used to extract the uncompressed size of the buffer. If the buffer does not have a footer 
        //provide the known uncompressed size through the customLength integer.
        if (useFooter)
        {
            res2 -= 4;
            uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
        }
        else
        {
            uncompressedSize = customLength;
        }

        byte[] outBuffer = new byte[uncompressedSize];

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

        //res should be the compressed size
        int res = LZ4DecompressBuffer(cbuf.AddrOfPinnedObject(), obuf.AddrOfPinnedObject(), uncompressedSize);

        cbuf.Free();
        obuf.Free();

        if (res != res2) { /*Debug.Log("ERROR: " + res.ToString());*/ return null; }

        return outBuffer;
    }

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
    // In any case, if you don't need the created in-Memory file, you should use the LZ4.LZ4releaseBuffer function to free the memory!
    //
    // Parameters:
    //
    // url:             The url of the file you want to download to a native memory buffer.
    // downloadDone:    Informs a bool that the download of the file to memory is done.
    // pointer:         An IntPtr for a native memory buffer
    // fileSize:        The size of the downloaded file will be returned here.
    public static IEnumerator downloadLZ4FileNative(string url, Action<bool> downloadDone, Action<IntPtr> pointer = null, Action<int> fileSize = null) {
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

                    nativeBuffer = LZ4Create_Buffer(zipSize);
                    nativeBufferIsBeingUsed = true;

                    // buffer for the download
                    byte[] bytes = new byte[2048];
                    nativeOffset = 0;

                    using (UnityWebRequest wwwSK = UnityWebRequest.Get(url)) {

                        // Here we call our custom webrequest function to download our archive to a native memory buffer.
                        wwwSK.downloadHandler = new CustomWebRequest4(bytes);
                        
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
    public class CustomWebRequest4 : DownloadHandlerScript {

        public CustomWebRequest4()
            : base()
        {
        }

        public CustomWebRequest4(byte[] buffer)
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
            LZ4AddTo_Buffer(nativeBuffer, nativeOffset, pbuf.AddrOfPinnedObject(), dataLength );
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
}
