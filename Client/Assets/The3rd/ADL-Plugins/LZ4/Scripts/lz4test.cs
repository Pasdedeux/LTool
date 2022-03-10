using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO;


public class lz4test : MonoBehaviour {
#if (!UNITY_TVOS && !UNITY_WEBGL) || UNITY_EDITOR

    // some variables to get status returns from the functions
    private float lz1 = 0;
    private int  lz2 = -1, lz3, lz4, fbuftest, nFbuftest;

	// A single item ulong array to get the bytes being decompressed
    private ulong[] bytes = new ulong[1];

	// A single item float array to get progress of compression.
    private float[] progress = new float[1];

    // a test file that will be downloaded to run the tests
    private string myFile = "testLZ4.tif";

    // the adress from where we download our test file
    private string uri = "https://dl.dropbox.com/s/r1ccmnreyd460vr/";

    // our path where we do the tests
    private string ppath;

    private bool compressionStarted;
    private bool downloadDone;

    // a reusable buffer
    private byte[] buff;

	// fixed size buffer, that don't gets resized, to perform decompression of buffers in them and avoid memory allocations.
	private byte[] fixedOutBuffer = new byte[1024*768];


    // Use this for initialization
    void Start () {
		ppath = Application.persistentDataPath;

        #if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			ppath=".";
        #endif

        buff = new byte[0];

        Debug.Log(ppath);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if(!File.Exists(ppath + "/" + myFile)) StartCoroutine(DownloadTestFile()); else downloadDone = true;
    }

	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
    }


    void OnGUI()
    {
        if (downloadDone == true)
        {
            GUI.Label(new Rect(50, 0, 350, 30), "package downloaded, ready to extract");
            GUI.Label(new Rect(50, 30, 450, 90), ppath);
        }

        if (downloadDone)
        {
            if (GUI.Button(new Rect(50, 150, 250, 50), "start LZ4 test"))
            {
                compressionStarted = true;
                // call the decompresion demo functions.
                // DoTests();
                // we call the test function on a thread to able to see progress. WebGL does not support threads.
				Thread th = new Thread(DoTests); th.Start();

                // native FileBuffer test
                #if (UNITY_IPHONE || UNITY_IOS  || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
                    StartCoroutine(nativeFileBufferTest());
                #endif
            }
        }

        if (compressionStarted){
            // if the return code is 1 then the decompression was succesful.
            GUI.Label(new Rect(50, 220, 250, 40), "LZ4 Compress:    " + (lz1).ToString() + "%");
            GUI.Label(new Rect(300, 220, 120, 40), progress[0].ToString() + "%");

            GUI.Label(new Rect(50, 260, 250, 40), "LZ4 Decompress: " + (lz2+1).ToString());
            GUI.Label(new Rect(300, 260, 250, 40), bytes[0].ToString());

            GUI.Label(new Rect(50, 300, 250, 40), "Buffer Compress:    " + lz3.ToString());
            GUI.Label(new Rect(50, 340, 250, 40), "Buffer Decompress: " + lz4.ToString());

			#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
				GUI.Label(new Rect(50, 380, 250, 40), "FileBuffer test: " + (fbuftest+1).ToString());
                GUI.Label(new Rect(50, 420, 280, 40), "Native FileBuffer test: " + nFbuftest.ToString());
			#endif
        }

     }


    void DoTests() {
    
        // File tests
        // compress a file to lz4 with highest level of compression (9).
        lz1 = LZ4.compress(ppath+ "/" + myFile, ppath + "/" + myFile + ".lz4", 9,  progress);

        // decompress the previously compressed archive
        lz2 = LZ4.decompress(ppath + "/" + myFile + ".lz4", ppath + "/" + myFile + "B.tif",  bytes);


        // Buffer tests
        if (File.Exists(ppath + "/" + myFile)){
            byte[] bt = File.ReadAllBytes(ppath + "/" + myFile);

            // compress a byte buffer (we write the output buffer to a file for debug purposes.)
            if (LZ4.compressBuffer(bt, ref buff, 9, true)){
                lz3 = 1;
                File.WriteAllBytes(ppath + "/buffer1.lz4buf", buff);
            }

            byte[] bt2 = File.ReadAllBytes(ppath + "/buffer1.lz4buf");

            // decompress a byte buffer (we write the output buffer to a file for debug purposes.)
            if (LZ4.decompressBuffer(bt2, ref buff, true)){
                lz4 = 1;
                File.WriteAllBytes(ppath + "/buffer1D.tif", buff);
            }

				//FIXED BUFFER FUNCTION:
				int decommpressedSize = LZ4.decompressBufferFixed(bt2, ref fixedOutBuffer);
				if(decommpressedSize > 0) Debug.Log(" # Decompress Fixed size Buffer: " + decommpressedSize);

			bt2= null; bt = null;
        }

		// make FileBuffer test on supported platfoms.
		#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
			// make a temp buffer to read an lz4 file in.
			if (File.Exists(ppath + "/" + myFile + ".lz4")){
				byte[] FileBuffer = File.ReadAllBytes(ppath + "/" + myFile + ".lz4");
				fbuftest = LZ4.decompress(null, ppath + "/" + myFile + ".FBUFF.tif",  bytes, FileBuffer);
			}
		#endif

    }


   IEnumerator DownloadTestFile()
    {
        // make sure a previous lz4 file having the same name with the one we want to download does not exist in the ppath folder
        if (File.Exists(ppath + "/" + myFile)) File.Delete(ppath + "/" + myFile);

        Debug.Log("starting download");

        // replace the link to the lz4 file with your own (although this will work also)
        using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(uri + myFile)) {
			#if UNITY_5 || UNITY_4
                yield return www.Send();
            #else
                yield return www.SendWebRequest();
            #endif

            if (www.error != null)
            {
                Debug.Log(www.error);
            } else {
                downloadDone = true;

                // write the downloaded lz4 file to the ppath directory so we can have access to it
                // depending on the Install Location you have set for your app, set the Write Access accordingly!
                File.WriteAllBytes(ppath + "/" + myFile, www.downloadHandler.data);

                Debug.Log("download done");
            }
        }
    }

    #if (UNITY_IPHONE || UNITY_IOS  || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
        // native file buffer test
        // For iOS, Android, Linux and MacOSX the plugin can handle a byte buffer as a file. (in this case a native file buffer)
        // This way you can extract the file or parts of it without writing it to disk.
        IEnumerator nativeFileBufferTest() {

            //make a check that the intermediate native buffer is not being used!
            if(LZ4.nativeBufferIsBeingUsed) { Debug.Log("Native buffer download is in use"); yield break; }

            // A bool for download checking
            bool downloadDoneN = false;

            // A native memory pointer
            IntPtr nativePointer = IntPtr.Zero;

            // int to get the downloaded file size
            int zsize = 0;

            Debug.Log("Downloading LZ4 file to native memory buffer");

            // Here we are calling the coroutine for a pointer. We also get the downloaded file size.
            StartCoroutine(LZ4.downloadLZ4FileNative("http://telias.free.fr/temp/testLZ4.tif.lz4", r => downloadDoneN = r, pointerResult => nativePointer = pointerResult, size => zsize = size));       

            while (!downloadDoneN) yield return true;

            nFbuftest = LZ4.decompress(zsize.ToString(), ppath + "/nativeBufferToFileLZ4.tif", null, nativePointer);
            
            if(nFbuftest ==0) nFbuftest = 1;

            // free the native memory buffer!
            LZ4.LZ4releaseBuffer(nativePointer);
        }
    #endif

#else
        void OnGUI(){
            GUI.Label(new Rect(10,10,500,40),"Please run the WebGL/tvOS demo.");
        }
#endif
}
