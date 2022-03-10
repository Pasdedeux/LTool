using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class fLZ{

#if !UNITY_WEBPLAYER || UNITY_EDITOR

    internal static bool isle = BitConverter.IsLittleEndian;

#if UNITY_5_4_OR_NEWER
	#if (UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_WEBGL) && !UNITY_EDITOR || UNITY_EDITOR_LINUX
		private const string libname = "fastlz";
	#elif UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		private const string libname = "libfastlz";
	#endif
#else
	#if (UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_WEBGL) && !UNITY_EDITOR
		private const string libname = "fastlz";
	#endif
	#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		private const string libname = "libfastlz";
	#endif
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX

		#if (!UNITY_WEBGL || UNITY_EDITOR)
			#if (UNITY_STANDALONE_OSX  || UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)&& !UNITY_EDITOR_WIN
				[DllImport(libname, EntryPoint = "fsetPermissions")]
				internal static extern int fsetPermissions(string filePath, string _user, string _group, string _other);
			#endif
			[DllImport(libname, EntryPoint = "fLZcompressFile"
                #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		        , CallingConvention = CallingConvention.Cdecl
                #endif
            )]
			internal static extern int fLZcompressFile(int level, string inFile, string outFile, bool overwrite, IntPtr percent);

			[DllImport(libname, EntryPoint = "fLZdecompressFile"
                #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		        , CallingConvention = CallingConvention.Cdecl
                #endif
            )]
			internal static extern int fLZdecompressFile(string inFile, string outFile, bool overwrite, IntPtr percent, IntPtr FileBuffer, int fileBufferLength);
		#endif

        [DllImport(libname, EntryPoint = "fLZreleaseBuffer"
            #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		    , CallingConvention = CallingConvention.Cdecl
            #endif
        )]
        public static extern void fLZreleaseBuffer(IntPtr buffer);

        [DllImport(libname, EntryPoint = "create_Buffer"
        #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		    , CallingConvention = CallingConvention.Cdecl
        #endif
        )]
        public static extern IntPtr create_Buffer(int size);

        [DllImport(libname, EntryPoint = "addTo_Buffer"
        #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		    , CallingConvention = CallingConvention.Cdecl
        #endif
        )]
        private static extern void addTo_Buffer(IntPtr destination, int offset, IntPtr buffer, int len);


        [DllImport(libname, EntryPoint = "fLZcompressBuffer"
            #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		    , CallingConvention = CallingConvention.Cdecl
            #endif
        )]
        internal static extern IntPtr fLZcompressBuffer(IntPtr buffer, int bufferLength, int level, ref int v);

        [DllImport(libname, EntryPoint = "fLZdecompressBuffer"
            #if (UNITY_STANDALONE_WIN && ENABLE_IL2CPP) || UNITY_ANDROID
		    , CallingConvention = CallingConvention.Cdecl
            #endif
        )]
        internal static extern int fLZdecompressBuffer(IntPtr buffer, int bufferLength, IntPtr outbuffer);
#endif

#if (UNITY_IOS || UNITY_TVOS || UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		#if (UNITY_IOS || UNITY_IPHONE) && !UNITY_WEBGL
			[DllImport("__Internal")]
			internal static extern int fsetPermissions(string filePath, string _user, string _group, string _other);
			[DllImport("__Internal")]
			internal static extern int fLZcompressFile(int level, string inFile, string outFile, bool overwrite, IntPtr percent);
			[DllImport("__Internal")]
			internal static extern int fLZdecompressFile(string inFile, string outFile, bool overwrite, IntPtr percent, IntPtr FileBuffer, int fileBufferLength);
            [DllImport("__Internal")]
            public static extern IntPtr create_Buffer(int size);
            [DllImport("__Internal")]
            internal static extern void addTo_Buffer(IntPtr destination, int offset, IntPtr buffer, int len);
		#endif
		#if (UNITY_IPHONE || UNITY_IOS || UNITY_TVOS || UNITY_WEBGL)
			[DllImport("__Internal")]
			public static extern void fLZreleaseBuffer(IntPtr buffer);
			[DllImport("__Internal")]
			internal static extern IntPtr fLZcompressBuffer(IntPtr buffer, int bufferLength, int level, ref int v);
			[DllImport("__Internal")]
			internal static extern int fLZdecompressBuffer(IntPtr buffer, int bufferLength, IntPtr outbuffer);
		#endif
#endif

	#if (!UNITY_WEBGL && !UNITY_TVOS) || UNITY_EDITOR
	// set permissions of a file in user, group, other.
	// Each string should contain any or all chars of "rwx".
	// returns 0 on success
	public static int setFilePermissions(string filePath, string _user, string _group, string _other){
		#if (UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX || UNITY_IOS || UNITY_TVOS || UNITY_IPHONE) && !UNITY_EDITOR_WIN
			return fsetPermissions(filePath, _user, _group, _other);
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

    
    //Compress a file to fLZ.
    //
    //Full paths to the files should be provided.
    //level:    level of compression (1 = faster/bigger, 2 = slower/smaller).
    //returns:  size of resulting archive in bytes
    //progress: provide a single item ulong array to get the progress of the compression in real time. (only when called from a thread/task)
    //

    public static int compressFile(string inFile, string outFile, int level, bool overwrite, ulong[] progress)
    {
        if (level < 1) level = 1;
        if (level > 2) level = 2;
        if(progress == null) progress = new ulong[1];

		GCHandle ibuf = GCHandle.Alloc(progress, GCHandleType.Pinned);
        int res = fLZcompressFile(level, @inFile, @outFile,  overwrite,  ibuf.AddrOfPinnedObject());
        ibuf.Free();
        return res;
    }

    // Decompress an fLZ file. An overloaded function to receive managed and unmanaged file Buffers.
    //
    // Full paths to the files should be provided.
    // returns: 1 on success. ( -5 when not passing the size of a native file buffer through the inFile string parameter. )
    // progress: provide a single item ulong array to get the progress of the decompression in real time. (only when called from a thread/task)
	// fileBuffer		: A buffer that holds an FLZ file. When assigned the function will decompress from this buffer and will ignore the filePath. (iOS, Android, MacOSX, Linux)
    //                  : It can be a byte[] buffer or a native IntPtr buffer (downloaded using the helper function: downloadFlzFileNative)
    //                  : When an IntPtr is used as the input buffer, the size of it must be passed to the function as a string with the inFile parameter!
    //
    public static int decompressFile(string inFile, string outFile, bool overwrite, ulong[] progress, object fileBuffer = null) {
		int res = 0;
        if(progress == null) progress = new ulong[1];
		GCHandle ibuf = GCHandle.Alloc(progress, GCHandleType.Pinned);
        
		#if (UNITY_IPHONE || UNITY_IOS  || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
            if(fileBuffer != null) {
                int fileBufferLength = 0;
                IntPtr fileBufferPointer = IntPtr.Zero;
                GCHandle fbuf;
                bool managed = checkObject(fileBuffer, inFile, ref fbuf, ref fileBufferPointer, ref fileBufferLength);
                
                if (!managed && fileBufferLength == 0) { Debug.Log("Please provide a valid native buffer size as a string in filePath"); return -5; }

				res = fLZdecompressFile(null, @outFile, overwrite, ibuf.AddrOfPinnedObject(), fileBufferPointer, fileBufferLength);

				if (managed) fbuf.Free();
                ibuf.Free();
				return res;
			}
		#endif
        res = fLZdecompressFile(inFile, @outFile, overwrite, ibuf.AddrOfPinnedObject(), IntPtr.Zero, 0);
        ibuf.Free();
        return res;
    }




	#endif

    //Compress a byte buffer in fLZ format.
    //
    //inBuffer:     the uncompressed buffer.
    //outBuffer:    a referenced buffer that will be resized to fit the fLZ compressed data.
    //includeSize:  include the uncompressed size of the buffer in the resulted compressed one because fLZ does not include this.
    //level:        level of compression (1 = faster/bigger, 2 = slower/smaller).
    //returns true on success
	//
	// !!  If the input is not compressible, the returned buffer might be larger than the input buffer and not valid !!
	//
	// The  minimum input buffer size is 16.
	// The output buffer can not be smaller than 66 bytes
	//
    public static bool compressBuffer(byte[] inBuffer, ref byte[] outBuffer, int level, bool includeSize = true)
    {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int res = 0, size = 0;
		byte[] bsiz = null;

        //if the uncompressed size of the buffer should be included. This is a hack since fLZ lib does not support this.
        if (includeSize) {
			bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!isle) Array.Reverse(bsiz);
        }

        if (level < 1) level = 1;
        if (level > 2) level = 2;

        ptr = fLZcompressBuffer(cbuf.AddrOfPinnedObject(), inBuffer.Length,  level, ref res);

        cbuf.Free();

        if (res == 0 || ptr == IntPtr.Zero) { fLZreleaseBuffer(ptr); return false; }

        System.Array.Resize(ref outBuffer, res + size);

        //add the uncompressed size to the buffer
        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[i+res] = bsiz[i];  /*Debug.Log(BitConverter.ToInt32(bsiz, 0));*/ }

        Marshal.Copy(ptr, outBuffer, 0, res  );

        fLZreleaseBuffer(ptr);
		bsiz = null;

        return true;
    }


    //Compress a byte buffer in fLZ format.
    //
    //inBuffer:     the uncompressed buffer.
    //outBuffer:    a referenced buffer that will be resized to fit the fLZ compressed data.
    //includeSize:  include the uncompressed size of the buffer in the resulted compressed one because fLZ does not include this.
    //level:        level of compression (1 = faster/bigger, 2 = slower/smaller).
	//returns: a new buffer with the compressed data.
    //
	// !!  If the input is not compressible, the returned buffer might be larger than the input buffer and not valid !!
	//
	// The  minimum input buffer size is 16.
	// The output buffer can not be smaller than 66 bytes
	//
    public static byte[] compressBuffer(byte[] inBuffer, int level, bool includeSize = true)
    {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        IntPtr ptr;

        int res = 0, size = 0;
		byte[] bsiz = null;

        //if the uncompressed size of the buffer should be included. This is a hack since fLZ lib does not support this.
        if (includeSize) {
			bsiz = new byte[4];
            size = 4;
            bsiz = BitConverter.GetBytes(inBuffer.Length);
            if (!isle) Array.Reverse(bsiz);
        }

        if (level < 1) level = 1;
        if (level > 2) level = 2;

        ptr = fLZcompressBuffer(cbuf.AddrOfPinnedObject(), inBuffer.Length,  level, ref res);
        cbuf.Free();

        if (res == 0 || ptr == IntPtr.Zero) { fLZreleaseBuffer(ptr); return null; }

        byte[] outBuffer = new byte[res + size];

        //add the uncompressed size to the buffer
        if (includeSize) { for (int i = 0; i < 4; i++) outBuffer[i + res] = bsiz[i];  /*Debug.Log(BitConverter.ToInt32(bsiz, 0));*/ }

        Marshal.Copy(ptr, outBuffer, 0, res);

        fLZreleaseBuffer(ptr);
		bsiz = null;

        return outBuffer;
    }


    //Decompress an fLZ compressed buffer to a referenced buffer.
    //
    //inBuffer: the fLZ compressed buffer
    //outBuffer: a referenced buffer that will be resized to store the uncompressed data.
    //useFooter: if the input Buffer has the uncompressed size info.
    //customLength: provide the uncompressed size of the compressed buffer. Not needed if the usefooter is used!
    //returns true on success
    //
    public static bool decompressBuffer(byte[] inBuffer, ref byte[] outBuffer, bool useFooter = true, int customLength = 0)
    {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        int uncompressedSize = 0, res2 = inBuffer.Length ,ex = 0;

        //if the hacked in fLZ footer will be used to extract the uncompressed size of the buffer. If the buffer does not have a footer 
        //provide the known uncompressed size through the customLength integer.
        if (useFooter) {
            res2 -= 4;
            uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
            if(inBuffer.Length > uncompressedSize) ex = inBuffer.Length - uncompressedSize;
        }
        else {
            uncompressedSize = customLength; 
            if(inBuffer.Length > outBuffer.Length) ex = inBuffer.Length - outBuffer.Length;
        }

        System.Array.Resize(ref outBuffer, uncompressedSize);

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);


        int res = fLZdecompressBuffer(cbuf.AddrOfPinnedObject(), uncompressedSize + ex, obuf.AddrOfPinnedObject());

        cbuf.Free();
        obuf.Free();

        if (res == 0) return true;

        return false;
    }

	//Decompress an flz compressed buffer to a referenced fixed size buffer.
    //
    //inBuffer: the flz compressed buffer
    //outBuffer: a referenced fixed size buffer where the data will get decompressed
    //useFooter: if the input Buffer has the uncompressed size info.
    //customLength: provide the uncompressed size of the compressed buffer. Not needed if the useFooter is used!
    //returns uncompressedSize
    //
	public static int decompressBufferFixed(byte[] inBuffer, ref byte[] outBuffer, bool safe = true, bool useFooter = true, int customLength = 0) {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        int uncompressedSize = 0, res2 = inBuffer.Length, ex = 0;

        //if the hacked in LZ4 footer will be used to extract the uncompressed size of the buffer. If the buffer does not have a footer 
        //provide the known uncompressed size through the customLength integer.
        if (useFooter){
            res2 -= 4;
            uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
            if(inBuffer.Length > uncompressedSize) ex = inBuffer.Length - uncompressedSize;
        }
        else{
            uncompressedSize = customLength;
            if(inBuffer.Length > outBuffer.Length) ex = inBuffer.Length - outBuffer.Length;
        }

		//Check if the uncompressed size is bigger then the size of the fixed buffer. Then:
		//1. write only the data that fits in it.
		//2. or return a negative number. 
		//It depends on if we set the safe flag to true or not.
		if(uncompressedSize > outBuffer.Length) {
			if(safe) return -101;  else  uncompressedSize = outBuffer.Length;
		 }

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

        int res = fLZdecompressBuffer(cbuf.AddrOfPinnedObject(), uncompressedSize + ex, obuf.AddrOfPinnedObject());

        cbuf.Free();
        obuf.Free();

		if(safe) { if (res != 0) return -101; }

        return uncompressedSize;
    }


    //Decompress an fLZ compressed buffer to a new buffer.
    //
    //inBuffer: the fLZ compressed buffer
    //useFooter: if the input Buffer has the uncompressed size info.
    //customLength: provide the uncompressed size of the compressed buffer. Not needed if the usefooter is used!
	//returns: a new buffer with the uncompressed data.
    //
    public static byte[] decompressBuffer(byte[] inBuffer, bool useFooter = true, int customLength = 0)
    {
        GCHandle cbuf = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
        int uncompressedSize = 0, res2 = inBuffer.Length, ex = 0;

        //if the hacked in fLZ footer will be used to extract the uncompressed size of the buffer. If the buffer does not have a footer 
        //provide the known uncompressed size through the customLength integer.
        if (useFooter)
        {
            res2 -= 4;
            uncompressedSize = (int)BitConverter.ToInt32(inBuffer, res2);
            if(inBuffer.Length > uncompressedSize) ex = inBuffer.Length - uncompressedSize;
        }
        else
        {
            uncompressedSize = customLength;
            if(inBuffer.Length > customLength) ex = inBuffer.Length - customLength;
        }

        byte[] outBuffer = new byte[uncompressedSize];

        GCHandle obuf = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);


        int res = fLZdecompressBuffer(cbuf.AddrOfPinnedObject(), uncompressedSize + ex, obuf.AddrOfPinnedObject());

        cbuf.Free();
        obuf.Free();
        
        if (res != 0) return null;

        return outBuffer;
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
    // In any case, if you don't need the created in-Memory file, you should use the fLZ.fLZreleaseBuffer function to free the memory!
    //
    // Parameters:
    //
    // url:             The url of the file you want to download to a native memory buffer.
    // downloadDone:    Informs a bool that the download of the file to memory is done.
    // pointer:         An IntPtr for a native memory buffer
    // fileSize:        The size of the downloaded file will be returned here.
    public static IEnumerator downloadFlzFileNative(string url, Action<bool> downloadDone, Action<IntPtr> pointer = null, Action<int> fileSize = null) {
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

                    nativeBuffer = create_Buffer(zipSize);
                    nativeBufferIsBeingUsed = true;

                    // buffer for the download
                    byte[] bytes = new byte[2048];
                    nativeOffset = 0;

                    using (UnityWebRequest wwwSK = UnityWebRequest.Get(url)) {

                        // Here we call our custom webrequest function to download our archive to a native memory buffer.
                        wwwSK.downloadHandler = new CustomWebRequest3(bytes);
                        
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
    public class CustomWebRequest3 : DownloadHandlerScript {

        public CustomWebRequest3()
            : base()
        {
        }

        public CustomWebRequest3(byte[] buffer)
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
            addTo_Buffer(nativeBuffer, nativeOffset, pbuf.AddrOfPinnedObject(), dataLength );
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

