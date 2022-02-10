using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;



public class testLzmaWebGLtvOS : MonoBehaviour
{
#if UNITY_WEBGL || UNITY_TVOS

	//an output Buffer for the decompressed lzma buffer
	private byte[] outbuffer = null, outbuffer2 = null;
	private Texture2D tex = null, tex2 = null;

	byte[] wwb = null;
	#if UNITY_TVOS
	byte[] sevenZ = null;
	#endif

	private bool downloadDone2;

	private string log = "";

    //log for output of results
    void plog(string t = "") {
        log += t + "\n"; ;
    }

	void Start(){
    	tex = new Texture2D(1,1,TextureFormat.RGBA32, false);
		tex2 = new Texture2D(1,1,TextureFormat.RGBA32, false);

		//get an lzma file from the net
		StartCoroutine( getFromSite() );
        #if UNITY_TVOS
        StartCoroutine(Download7ZFile());
        #endif

        outbuffer2 = new byte[1];

        #if UNITY_TVOS && !UNITY_EDITOR
        StartCoroutine(tvos());
        #endif
    }

    #if UNITY_TVOS && !UNITY_EDITOR
    IEnumerator tvos() {
        yield return new WaitForSeconds(6);
        log = "";
        StartCoroutine(performOperations());
        yield return new WaitForSeconds(2);
        log = "";
        StartCoroutine(perform7zOperations());
        yield return true;
    }
	#endif

	void OnGUI(){
		
		if (downloadDone2 == true) {
			GUI.Label(new Rect(10, 0, 250, 30), "got package, ready to test");

			if (GUI.Button(new Rect(10, 40, 230, 50), "start Lzma test")) {
				log = "";
				StartCoroutine(performOperations());
			}
            #if UNITY_TVOS
            if (GUI.Button(new Rect(10, 100, 230, 50), "7z decode2Buffer test")) {
                log = "";
                StartCoroutine(perform7zOperations());
            }
            #endif
        }

        if (tex != null) GUI.DrawTexture(new Rect(300, 10, 375, 300), tex);
		if(tex2 != null) GUI.DrawTexture(new Rect(680, 10, 375, 300), tex2);

		GUI.TextArea(new Rect(10, 370, Screen.width - 20, Screen.height - 400), log);
				
	}



	// ============================================================================================================================================================= 


    IEnumerator getFromSite() {
		plog("getting file from Site ...");

         using (UnityWebRequest www = UnityWebRequest.Get("https://dl.dropbox.com/s/r8upgppb4kqz1sw/testLZ4b.png.lzma")) {
            #if UNITY_5 || UNITY_4
                yield return www.Send();
            #else
                yield return www.SendWebRequest();
            #endif

            if (www.error != null)  {
                Debug.Log(www.error);
            } else {
                wwb = new byte[www.downloadHandler.data.Length];
                Array.Copy(www.downloadHandler.data, 0, wwb, 0, www.downloadHandler.data.Length);
                 plog("Got file");
            }
        }

		downloadDone2 = true;
    }

    #if UNITY_TVOS
    IEnumerator Download7ZFile() {

        //replace the link to the 7zip file with your own (although this will work also)
        using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get("https://dl.dropbox.com/s/16v2ng25fnagiwg/test.7z")) {
        #if UNITY_5 || UNITY_4
                yield return www.Send();
        #else
            yield return www.SendWebRequest();
        #endif

            if (www.error != null) {
                plog(www.error);
            } else {

                log = "";
                sevenZ = new byte[www.downloadHandler.data.Length];
                Array.Copy(www.downloadHandler.data, sevenZ, www.downloadHandler.data.Length);

                plog("download 7z done");
            }
        }

    }
    IEnumerator perform7zOperations() {
        if(sevenZ == null || (sevenZ != null && sevenZ.Length < 10)) { plog("No 7z found."); yield break;}

        uint headersSize = lzma.getHeadersSize(null, sevenZ);
        plog("Headers size of 7z: " + headersSize.ToString());

        var entry = lzma.decode2Buffer(null, "1.txt", sevenZ);
        if(entry != null) plog("buffer decoded. size: " + entry.Length.ToString() + "  progress: " + lzma.getBytesWritten().ToString());

        plog("Get entry size: " + lzma.get7zSize(null, "1.txt", sevenZ).ToString() + "\n");

        plog("Get info on 7z: " + lzma.get7zInfo(null, sevenZ).ToString() );
        

        if(lzma.ninfo != null && lzma.ninfo.Count > 0) {
            for(int i = 0; i < lzma.ninfo.Count; i++){
                plog(i.ToString() + ". " + lzma.ninfo[i].ToString() + " : " + lzma.sinfo[i].ToString());
            }
        }
    }
    #endif

    IEnumerator performOperations() {
		plog("Decompress buffer: "+lzma.decompressBuffer(wwb, ref outbuffer).ToString());
		if(outbuffer != null) tex.LoadImage(outbuffer);
		plog();
		yield return true;
		byte[] bb = new byte[1];
        lzma.setProps(9, 1 << 16);
		plog("Compress buffer: " +lzma.compressBuffer(outbuffer, ref bb).ToString());
		plog();
		yield return true;
		plog("Decompress buffer 2: "+lzma.decompressBuffer(bb, ref outbuffer2).ToString());
		if(outbuffer2 != null) tex2.LoadImage(outbuffer2);
	}

#else
    void OnGUI(){
        GUI.Label(new Rect(10,10,500,40),"Only for WebGL or tvOS.");
    }
#endif

}

