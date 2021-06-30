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
    public class VibrateManager : Singleton<VibrateManager>, IManager
    {
        private AndroidJavaObject _javaObject;
        private RuntimePlatform _platForm;

        public void Install()
        {
            _platForm = Application.platform;
            if ( _platForm == RuntimePlatform.Android )
            {
                AndroidJavaClass jd = new AndroidJavaClass( "com.taotao.newshake.MainShake" );
                _javaObject = jd.CallStatic<AndroidJavaObject>( "GetInstans" );
            }
        }

        public void Uninstall() { }

        /// <summary>
        /// 延迟毫秒,震动时间,延迟毫秒,震动时间
        /// </summary>0,100,100,100
        /// <param name="pattern">延迟（毫秒）,震动时间（毫秒）,延迟（毫秒）,震动时间（毫秒）</param>
        /// <param name="repeat">-1不循环 2=无限循环</param>
        public void Shake( long[] pattern, int repeat = -1 )
        {
            LDebug.Log( "震动 " + _platForm );
            if ( _platForm == RuntimePlatform.Android )
                _javaObject.Call("UnityCallShake",pattern,repeat);
        }
    }

}

