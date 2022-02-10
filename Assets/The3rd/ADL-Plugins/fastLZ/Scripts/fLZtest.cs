using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO;


public class fLZtest : MonoBehaviour {
#if (!UNITY_TVOS && !UNITY_WEBGL)  || UNITY_EDITOR

    // some variables to get status returns from the functions
    private int  lz1, lz2, lz3, lz4, fbuftest, nFbuftest;

	// a single item ulong array to get the progress of the compression
    private ulong[] progress = new ulong[1];

	// a single item ulong array to get the progress of the decompression
	private ulong[] progress2 = new ulong[1];
	
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
            if (GUI.Button(new Rect(50, 150, 250, 50), "start fastLZ test"))
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
            GUI.Label(new Rect(50, 220, 250, 40), "fLZ Compress:    " + lz1.ToString()  + " bytes");
            GUI.Label(new Rect(300, 220, 120, 40), progress[0].ToString() + "%");

            GUI.Label(new Rect(50, 260, 250, 40), "fLZ Decompress: " + lz2.ToString());
            GUI.Label(new Rect(300, 260, 250, 40), progress2[0].ToString() + "%");

            GUI.Label(new Rect(50, 300, 250, 40), "Buffer Compress:    " + lz3.ToString());
            GUI.Label(new Rect(50, 340, 250, 40), "Buffer Decompress: " + lz4.ToString());

			#if (UNITY_EDITOR || UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX) && !UNITY_EDITOR_WIN
				GUI.Label(new Rect(50, 380, 250, 40), "FileBuffer test: " + fbuftest.ToString());
                GUI.Label(new Rect(50, 420, 280, 40), "Native FileBuffer test: " + nFbuftest.ToString());
			#endif
        }

     }


    void DoTests() {
        // File tests
        // compress a file to flz with highest level of compression (2).
        lz1 = fLZ.compressFile(ppath+ "/" + myFile, ppath + "/" + myFile + ".flz", 2, true, progress);

        // decompress the previously compressed archive
        lz2 = fLZ.decompressFile(ppath + "/" + myFile + ".flz", ppath + "/" + myFile + "B.tif", true,  progress2);

        // Buffer tests
        if (File.Exists(ppath + "/" + myFile)) {
            byte[] bt = File.ReadAllBytes(ppath + "/" + myFile);

            // compress a byte buffer (we write the output buffer to a file for debug purposes.)
            if (fLZ.compressBuffer(bt, ref buff, 2, true)){
                lz3 = 1;
                File.WriteAllBytes(ppath + "/buffer1.flzbuf", buff);
            }

            byte[] bt2 = File.ReadAllBytes(ppath + "/buffer1.flzbuf");

            // decompress a byte buffer (we write the output buffer to a file for debug purposes.)
            if (fLZ.decompressBuffer(bt2, ref buff, true)){
                lz4 = 1;
                File.WriteAllBytes(ppath + "/buffer1.tif", buff);
            }

			// FIXED BUFFER FUNCTION:
			int decommpressedSize = fLZ.decompressBufferFixed(bt2, ref fixedOutBuffer);
			if(decommpressedSize > 0) Debug.Log(" # Decompress Fixed size Buffer: " + decommpressedSize);


			bt2= null; bt = null;
        }

		// make FileBuffer tests on supported platfoms.
		#if (UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_STANDALONE_LINUX || UNITY_EDITOR) && !UNITY_EDITOR_WIN
			// make a temp buffer to read an flz file in.
			if (File.Exists(ppath + "/" + myFile + ".flz")){
				byte[] FileBuffer = File.ReadAllBytes(ppath + "/" + myFile + ".flz");
				fbuftest = fLZ.decompressFile(null, ppath + "/" + myFile + ".flzFILE_BUFF.tif", true,  progress2, FileBuffer);
			}


		#endif


    }


   IEnumerator DownloadTestFile() {
        //make sure a previous flz file having the same name with the one we want to download does not exist in the ppath folder
        if (File.Exists(ppath + "/" + myFile)) File.Delete(ppath + "/" + myFile);

        Debug.Log("starting download");

        //replace the link to the flz file with your own (although this will work also)
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

                //write the downloaded flz file to the ppath directory so we can have access to it
                //depending on the Install Location you have set for your app, set the Write Access accordingly!
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
            if(fLZ.nativeBufferIsBeingUsed) { Debug.Log("Native buffer download is in use"); yield break; }

            // A bool for download checking
            bool downloadDoneN = false;

            // A native memory pointer
            IntPtr nativePointer = IntPtr.Zero;

            // int to get the downloaded file size
            int zsize = 0;

            Debug.Log("Downloading flz file to native memory buffer");

            // Here we are calling the coroutine for a pointer. We also get the downloaded file size.
            StartCoroutine(fLZ.downloadFlzFileNative("http://telias.free.fr/temp/testLZ4.tif.flz", r => downloadDoneN = r, pointerResult => nativePointer = pointerResult, size => zsize = size));       

            while (!downloadDoneN) yield return true;

            nFbuftest = fLZ.decompressFile(zsize.ToString(), ppath + "/nativeBufferToFile.tif", true, null, nativePointer);
            
            // free the native memory buffer!
            fLZ.fLZreleaseBuffer(nativePointer);
        }
    #endif

#else
        void OnGUI() {
            GUI.Label(new Rect(10, 10, 500, 40), "Please switch to the WebGL-tvOS example.");
        }
#endif
}
