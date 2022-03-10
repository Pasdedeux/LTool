
using UnityEngine;
using System;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class benchmark : MonoBehaviour{
#if (!UNITY_WEBPLAYER && !UNITY_WEBGL && !UNITY_TVOS)  || UNITY_EDITOR


    private int lzres = 0, zipres = 0, flzres = 0;
	private int brres = 0, lz4res = 0, gzres = 0;

    private bool pass1, pass2;

    //for counting the time taken to decompress the 7z file.
    private float t1, tim;

    //the test file to download.
    private string myFile = "testimg2.7z", myFile2 = "testimg.zip", uncFile = "testimg.tif";

    //the adress from where we download our test file
    private string uri = "https://dl.dropbox.com/s/5r7tlkvff9ba04b/";

    private string ppath;

    private string log="";
	
	private bool downloadDone, benchmarkStarted;
	
	private long tsize;

	GUIStyle style;

    //A 1 item integer array to get the current extracted file of the 7z archive. Compare this to the total number of the files to get the progress %.
    private int[] progress = new int[1];
	private ulong[] progress1 = new ulong[1];
	private ulong[] progress2 = new ulong[1];
	private float[] progress3 = new float[1];
	private ulong[] progress4 = new ulong[1];
	private ulong[] bytes = new ulong[1];
	private ulong[] gzProgress = new ulong[1];

    void  Start(){

		ppath = Application.persistentDataPath;

		//we are setting the lzma.persitentDataPath so the get7zinfo, get7zSize, decode2Buffer functions can work on separate threads!
		lzma.persitentDataPath = Application.persistentDataPath;

		#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			ppath=".";
		#endif

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

		if (!File.Exists(ppath + "/" + myFile)) StartCoroutine(Download7ZFile()); else downloadDone = true;

		benchmarkStarted = false;

		style = new GUIStyle ();
		style.richText = true;
		GUI.color = Color.black;
    }
	
	

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
    }
	

    void OnGUI(){
		
        if (downloadDone){

			if(!benchmarkStarted) {
				if (GUI.Button(new Rect(10, 10, 170, 50), "start Benchmark (48 mb)")){
					benchmarkStarted = true;
					log = "";
					lzres = 0; zipres = 0; flzres = 0;	lz4res = 0;

			
					StartCoroutine(decompressFunc());
				}
			}

        } 

		GUI.TextArea(new Rect(10, 70, Screen.width - 20, Screen.height - 190), log,style);
    }


	
    //call from separate thread. here you can get the progress of the extracted files through a referenced integer.
	IEnumerator decompressFunc(){
        System.IO.FileInfo fio;

        fio = new FileInfo(ppath + "/testimg2.7z");
        //decompress 7zip
        log += "<color=lime>decompressing 7zip ... <color=yellow>(" + ((float)fio.Length/1024).ToString("F")+ " kb)</color></color>";
		t1 = Time.realtimeSinceStartup;
		lzres = lzma.doDecompress7zip(ppath + "/"+myFile , ppath + "/",  true,true);
		log += "  <color=white>(" + lzma.getBytesWritten().ToString() + ")\n</color>";
		tim = Time.realtimeSinceStartup - t1;
		log += "<color=white>status: "+ lzres + " |  7z time: </color>" + "<color=cyan>"+ tim + "   sec</color>\n\n";
		log += "<color=orange>compressing lzma ... </color>";
		yield return true;

		//compress lzma alone
		t1 = Time.realtimeSinceStartup;
		lzma.setProps(9, 1 << 16);
		if(File.Exists(ppath +"/"+ uncFile+".lzma")) File.Delete(ppath +"/"+ uncFile+".lzma");
		lzres = lzma.LzmaUtilEncode( ppath +"/"+ uncFile, ppath +"/"+ uncFile+".lzma");
		log += "<color=white>(" + lzma.getBytesRead().ToString() + ")\n</color>";
		tim = Time.realtimeSinceStartup - t1;
        fio = new FileInfo(ppath + "/" + uncFile + ".lzma");
        log += "<color=white>status: "+ lzres +" |  lzma time: </color>" + "<color=teal>" + tim + "   sec</color>   <color=yellow>(" + ((float)fio.Length / 1024).ToString("F") + " kb)</color>\n\n";
		log += "<color=lime>decompressing lzma alone ...  </color>";
		yield return true;

        //decompress lzma alone
        t1 = Time.realtimeSinceStartup;
        lzres = lzma.LzmaUtilDecode(ppath + "/" + uncFile + ".lzma", ppath + "/" + uncFile );
		log += "<color=white>(" + lzma.getBytesWritten().ToString() + ")\n</color>";
        tim = Time.realtimeSinceStartup - t1;
        log += "<color=white>status: " + lzres + " |  lzma time: </color>" + "<color=cyan>" + tim + "   sec</color>\n\n";
        log += "<color=orange>compressing zip ...  </color>";
        yield return true;


        //compress zip
        t1 = Time.realtimeSinceStartup;
		if(File.Exists(ppath + "/"+myFile2)) File.Delete(ppath + "/"+myFile2);
        progress1[0] = 0;
		zipres = lzip.compress_File(9, ppath + "/"+myFile2, ppath + "/"+uncFile, false, null, null, null, false, 0, progress1);
		log += "<color=white>(" + progress1[0].ToString() + ")\n</color>";
		tim = Time.realtimeSinceStartup - t1;
        fio = new FileInfo(ppath + "/" + myFile2);
        log += "<color=white>status: "+ zipres + " |  zip time: </color>" + "<color=teal>" + tim + "   sec</color>   <color=yellow>(" + ((float)fio.Length / 1024).ToString("F") + " kb)</color>\n\n";
		log += "<color=lime>decompressing zip ... </color>";
		yield return true;
		
		//decompress zip
		t1 = Time.realtimeSinceStartup;
		zipres = lzip.decompress_File(ppath + "/"+myFile2, ppath+"/", progress, null, progress1);
		log += "<color=white>(" + progress1[0].ToString() + ")\n</color>";
		tim = Time.realtimeSinceStartup - t1;
		log += "<color=white>status: "+ zipres + " |  zip time: </color>" + "<color=cyan>" + tim + "   sec</color>\n\n";
    #if (!UNITY_EDITOR_OSX && !UNITY_STANDALONE_OSX && !UNITY_IOS && !UNITY_TVOS) || (UNITY_EDITOR && !UNITY_EDITOR_OSX)
		log += "<color=orange>Compressing to zip-bz2 ...  </color>";
		yield return true;

        //compress zip-bz2
        t1 = Time.realtimeSinceStartup;
		if(File.Exists(ppath + "/"+myFile2+"bz2.zip")) File.Delete(ppath + "/"+myFile2+"bz2.zip");
        progress1[0] = 0;
		zipres = lzip.compress_File(9, ppath + "/"+myFile2+"bz2.zip", ppath + "/"+uncFile, false, null, null, null, true, 0, progress1);
		log += "<color=white>(" + progress1[0].ToString() + ")\n</color>";
		tim = Time.realtimeSinceStartup - t1;
        fio = new FileInfo(ppath + "/" + myFile2+"bz2.zip");
        log += "<color=white>status: "+ zipres + " |  zip-bz2 time: </color>" + "<color=teal>" + tim + "   sec</color>   <color=yellow>(" + ((float)fio.Length / 1024).ToString("F") + " kb)</color>\n\n";
		log += "<color=lime>decompressing zip-bz2 ...  </color>";
		yield return true;
		
		//decompress zip-bz2
		t1 = Time.realtimeSinceStartup;
		zipres = lzip.decompress_File(ppath + "/"+myFile2+"bz2.zip", ppath+"/", progress, null, progress1);
		log += "<color=white>(" + progress1[0].ToString() + ")\n</color>";
		tim = Time.realtimeSinceStartup - t1;
		log += "<color=white>status: "+ zipres + " |  zip-bz2 time: </color>" + "<color=cyan>" + tim + "   sec</color>\n\n";
    #endif
		log += "<color=orange>Compressing to flz ...  </color>";
		yield return true;

		//compress flz
		t1 = Time.realtimeSinceStartup;
		flzres = fLZ.compressFile(ppath+ "/" + uncFile, ppath + "/" + uncFile + ".flz", 2, true, progress2);
		log += "<color=white>(" + progress2[0].ToString() + ")\n</color>";
		tim = Time.realtimeSinceStartup - t1;
        fio = new FileInfo(ppath + "/" + uncFile + ".flz");
        log += "<color=white>status: "+ flzres + " |  flz time: </color>" + "<color=teal>" + tim + "   sec</color>   <color=yellow>(" + ((float)fio.Length / 1024).ToString("F") + " kb)</color>\n\n";
		log += "<color=lime>Decompressing flz ...  </color>";
		yield return true;

		//decompress flz
		t1 = Time.realtimeSinceStartup;
		flzres  = fLZ.decompressFile(ppath + "/" + uncFile + ".flz", ppath + "/" + uncFile , true,  progress2);
		log += "<color=white>(" + progress2[0].ToString() + ")\n</color>";
		tim = Time.realtimeSinceStartup - t1;
		log += "<color=white>status: "+ flzres + " |  flz time: </color>" + "<color=cyan>" + tim + "   sec</color>\n\n";
		log += "<color=orange>Compressing to LZ4 ...  </color>";
		yield return true;

		//compress lz4
		t1 = Time.realtimeSinceStartup;
		lz4res = (int) LZ4.compress(ppath+ "/" + uncFile, ppath + "/" + uncFile + ".lz4", 9,  progress3);
		log += "<color=white>(" + progress3[0].ToString() + ")\n</color>";
		tim = Time.realtimeSinceStartup - t1;
        fio = new FileInfo(ppath + "/" + uncFile + ".lz4");
        log += "<color=white>status: "+ lz4res + " |  LZ4 time: </color>" + "<color=teal>" + tim + "   sec</color>     <color=yellow>(" + ((float)fio.Length / 1024).ToString("F") + " kb)</color>\n\n";
		log += "<color=lime>Decompressing LZ4 ...  </color>";
		yield return true;

		//decompress lz4
		t1 = Time.realtimeSinceStartup;
		lz4res = LZ4.decompress(ppath + "/" + uncFile + ".lz4", ppath + "/" + uncFile ,  bytes);
		log += "<color=white>(" + bytes[0].ToString() + ")\n</color>";
		tim = Time.realtimeSinceStartup - t1;
		log += "<color=white>status: "+ lz4res + " |  LZ4 time: </color>" + "<color=cyan>" + tim + "   sec</color>  \n\n";
		log += "<color=orange>Compressing to brotli ...  </color>";
		yield return true;

		//compress brotli
		t1 = Time.realtimeSinceStartup;
		brres = (int) brotli.compressFile(ppath+ "/" + uncFile, ppath + "/" + uncFile + ".br",   progress4);
		log += "<color=white>(" + progress4[0].ToString() + ")\n</color>";
		tim = Time.realtimeSinceStartup - t1;
        fio = new FileInfo(ppath + "/" + uncFile + ".br");
        log += "<color=white>status: "+ brres + " |  brotli time: </color>" + "<color=teal>" + tim + "   sec</color>     <color=yellow>(" + ((float)fio.Length / 1024).ToString("F") + " kb)</color>\n\n";
		log += "<color=lime>Decompressing brotli ...  </color>";
		yield return true;

		//decompress brotli
		t1 = Time.realtimeSinceStartup;
        progress4[0] = 0;
		brres = brotli.decompressFile(ppath + "/" + uncFile + ".br", ppath + "/" + uncFile ,   progress4);
		log += "<color=white>(" + progress4[0].ToString() + ")\n</color>";
		tim = Time.realtimeSinceStartup - t1;
		log += "<color=white>status: "+ brres + " |  brotli time: </color>" + "<color=cyan>" + tim + "   sec</color>  \n\n";
		log += "<color=lime>Compressing gzip ...  </color>";
		yield return true;

		//compress gzip
		t1 = Time.realtimeSinceStartup;
		gzres =  lzip.gzipFile(ppath+ "/" + uncFile, ppath + "/" + uncFile + ".gz", 10, gzProgress);
		log += "<color=white>(" + gzProgress[0].ToString() + ")\n</color>";
		tim = Time.realtimeSinceStartup - t1;
        fio = new FileInfo(ppath + "/" + uncFile + ".gz");
        log += "<color=white>status: "+ gzres + " |  gzip time: </color>" + "<color=teal>" + tim + "   sec</color>     <color=yellow>(" + ((float)fio.Length / 1024).ToString("F") + " kb)</color>\n\n";
		log += "<color=lime>Decompressing gzip ...  </color>";
		yield return true;

		//decompress gzip
		t1 = Time.realtimeSinceStartup;
        gzProgress[0] = 0;
		gzres = lzip.ungzipFile(ppath + "/" + uncFile + ".gz", ppath + "/" + uncFile , gzProgress);
		log += "<color=white>(" + gzProgress[0].ToString() + ")\n</color>";
		tim = Time.realtimeSinceStartup - t1;
		log += "<color=white>status: "+ brres + " |  gzip time: </color>" + "<color=cyan>" + tim + "   sec</color>  \n\n";
		yield return true;

		//test setting file permissions
		#if !UNITY_EDITOR && !UNITY_STANDALONE_WINDOWS
		Debug.Log(lzma.setFilePermissions(ppath+ "/" + uncFile, "rw","r","r"));
		Debug.Log(lzip.setFilePermissions(ppath+ "/" + uncFile, "rw","r","r"));
		Debug.Log(fLZ.setFilePermissions(ppath+ "/" + uncFile, "rw","r","r"));
		Debug.Log(LZ4.setFilePermissions(ppath+ "/" + uncFile, "rw","r","r"));
		Debug.Log(brotli.setFilePermissions(ppath+ "/" + uncFile, "rw","r","r"));
		#endif
		benchmarkStarted = false;
    }



    IEnumerator Download7ZFile() {

		//make sure a previous 7z file having the same name with the one we want to download does not exist in the ppath folder
		if (File.Exists(ppath + "/" + myFile)) File.Delete(ppath + "/" + myFile);

        Debug.Log("downloading 7zip file");
        log += "<color=white>downloading 7zip file ...\n</color>";

        //replace the link to the brotli file with your own (although this will work also)
        using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(uri + myFile)) {
            #if UNITY_5 || UNITY_5
            yield return www.Send();
            #else
            yield return www.SendWebRequest();
            #endif

            if (www.error != null)
            {
                Debug.Log(www.error);
            } else {
                downloadDone = true;
                log = "";
                //write the downloaded brotli file to the ppath directory so we can have access to it
                //depending on the Install Location you have set for your app, set the Write Access accordingly!
                File.WriteAllBytes(ppath + "/" + myFile, www.downloadHandler.data);

                Debug.Log("download done");
            }
        }
    }


#else
        void OnGUI(){
            GUI.Label(new Rect(10, 10, 500, 40), "Does not work on WebGL or tvOS.");
        }
#endif
}

