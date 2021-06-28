using Assets.Scripts;
using LitFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace LitFramework
{
    public class VibrateManager : Singleton<VibrateManager>,IManager
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        public AndroidJavaObject javaObject;
#endif
       
        public void Install()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            javaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
#endif
        }

        public void Uninstall() { }

        /// <summary>
        /// 延迟毫秒,震动时间,延迟毫秒,震动时间
        /// </summary>0,100,100,100
        /// <param name="pattern">延迟（毫秒）,震动时间（毫秒）,延迟（毫秒）,震动时间（毫秒）</param>
        /// <param name="repeat">-1不循环 2=无限循环</param>
        public void Shake( long[] pattern, int repeat = -1 )
        {
#if UNITY_ANDROID && !UNITY_EDITOR
               javaObject.Call("UnityCallShake",pattern,repeat);
#endif
        }
    }

}

