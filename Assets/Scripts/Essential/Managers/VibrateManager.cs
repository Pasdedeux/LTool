using Assets.Scripts;
using LitFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.ComponentModel;
using LitFramework.Singleton;

namespace LitFramework
{

    public enum VibrateState
    {
        /// <summary>
        /// 轻轻的震动
        /// </summary>
        Softly,
        /// <summary>
        /// 连续短促的小震
        /// </summary>
        Interval,
        /// <summary>
        /// 强烈的震动
        /// </summary>
        Acute,
    }

    public class VibrateManager : Singleton<VibrateManager>, IManager
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport( "__Internal" )]
        private static extern void InstantiateFeedbackGenerators();
        [DllImport( "__Internal" )]
        private static extern void ReleaseFeedbackGenerators();
        [DllImport( "__Internal" )]
        private static extern void SelectionHaptic();
        [DllImport( "__Internal" )]
        private static extern void SuccessHaptic();
        [DllImport( "__Internal" )]
        private static extern void WarningHaptic();
        [DllImport( "__Internal" )]
        private static extern void FailureHaptic();
        [DllImport( "__Internal" )]
        private static extern void LightImpactHaptic();
        [DllImport( "__Internal" )]
        private static extern void MediumImpactHaptic();
        [DllImport( "__Internal" )]
        private static extern void HeavyImpactHaptic();
#elif UNITY_ANDROID && !UNITY_EDITOR
        private AndroidJavaObject _javaObject;
        private long[] _softly = new long[] { 0, 50, 10, 50 }, _interval = new long[] { 0, 100 }, _acute = new long[] { 0, 300 };
#endif

        public void Install()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass jd = new AndroidJavaClass( "com.taotao.newshake.MainShake" );
            _javaObject = jd.CallStatic<AndroidJavaObject>( "GetInstans" );
#elif UNITY_IOS && !UNITY_EDITOR
            InstantiateFeedbackGenerators();
#endif
        }

        public void Uninstall() { }



        public void Shake( VibrateState vibrateState )
        {
            LDebug.Log( "[设置]->震动 "+ vibrateState );
            switch ( vibrateState )
            {
                case VibrateState.Softly:
#if UNITY_ANDROID && !UNITY_EDITOR
                    Shake( _softly );
#elif UNITY_IOS && !UNITY_EDITOR
                    SelectionHaptic();
#endif
                    break;
                case VibrateState.Interval:
#if UNITY_ANDROID && !UNITY_EDITOR
                    Shake( _interval );
#elif UNITY_IOS && !UNITY_EDITOR
                    SuccessHaptic();
#endif
                    break;
                case VibrateState.Acute:
#if UNITY_ANDROID && !UNITY_EDITOR
                    Shake( _acute );
#elif UNITY_IOS && !UNITY_EDITOR
                    FailureHaptic();
#endif
                    break;
            }
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        /// <summary>
        /// 延迟毫秒,震动时间,延迟毫秒,震动时间
        /// </summary>0,100,100,100
        /// <param name="pattern">延迟（毫秒）,震动时间（毫秒）,延迟（毫秒）,震动时间（毫秒）</param>
        /// <param name="repeat">-1不循环 2=无限循环</param>
        public void Shake( long[] pattern, int repeat = -1 )
        {
            LDebug.Log( "[设置]->震动 " );

            _javaObject.Call( "UnityCallShake", pattern, repeat );
        }
#endif
    }

}

