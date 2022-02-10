using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;



public class fLZtestWebGLtvOS : MonoBehaviour
{
#if UNITY_WEBGL || UNITY_TVOS


	//an output Buffer for the decompressed flz buffer
	private byte[] outbuffer = null;
	private Texture2D tex = null;

    byte[] compressedBuffer = null;
	byte[] wwb = null;

	private bool downloadDone2;

	private string log = "";

    //log for output of results
    void plog(string t)
    {
        log += t + "\n"; ;
    }

	void Start(){
		tex = new Texture2D(1600,1280,TextureFormat.RGBA32, false);

		//get an image from the internet to perform compress/decompress operations on it
		StartCoroutine( getFromSite() );
    }


	
	void OnGUI(){
		
		if (downloadDone2 == true) {
			GUI.Label(new Rect(10, 0, 250, 30), "got image, ready to compress/decompress");


			if (GUI.Button(new Rect(10, 90, 230, 50), "start FLZ test")) {
                StartCoroutine(performOperations());

			}
		}

		if(tex != null) GUI.DrawTexture(new Rect(360, 10, 375, 300), tex);

		GUI.TextArea(new Rect(10, 370, Screen.width - 20, Screen.height - 400), log);
	}



	// ============================================================================================================================================================= 

	IEnumerator performOperations() {
        log ="";
        yield return true;

        if(wwb == null) yield break; else plog("Image size: " + wwb.Length.ToString());

        if(compressedBuffer != null) compressedBuffer = null;

        compressedBuffer = fLZ.compressBuffer(wwb, 2);

        plog("Buffer compressed. Size: " + compressedBuffer.Length.ToString());
        yield return true;

		plog("flz decompress: " + fLZ.decompressBuffer(compressedBuffer, ref outbuffer).ToString() );
		if(outbuffer != null) { tex.LoadImage(outbuffer); plog("Decompressed size: " + outbuffer.Length.ToString()); }
        yield return true;
    }

	IEnumerator getFromSite() {
		plog("getting image from Site ...");

         using (UnityWebRequest www = UnityWebRequest.Get("https://dl.dropbox.com/s/sp79pnbnw6xhn43/testLZ4b.png")) {
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
                 plog("Got image");
            }
        }

		downloadDone2 = true;

        #if UNITY_TVOS && !UNITY_EDITOR
            yield return true;
            StartCoroutine(performOperations());
        #endif
    }

#else
    void OnGUI(){
       GUI.Label(new Rect(10, 10, 500, 40),"Only for WebGL or tvOS");
    }
#endif

}

