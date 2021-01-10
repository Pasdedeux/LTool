using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace LitFramework
{
    public class VibrateManager : SingletonMono<VibrateManager>
    {
        public AndroidJavaObject javaObject;
        public void Awake()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            javaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
#endif
        }
        public void ClickShake()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
               javaObject.Call("UnityCallShake");
#endif
        }
    }

}

