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

namespace LitFramework
{

    public enum VibrateState
    {
        /// <summary>
        /// 轻轻的震动
        /// </summary>
        Softly,
        /// <summary>
        /// 连续短促的小震  3
        /// </summary>
        Interval,
        /// <summary>
        /// 强烈的震动
        /// </summary>
        Acute,
    }

    public class VibrateManager : Singleton<VibrateManager>, IManager
    {
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


        private RuntimePlatform _platForm;
        private AndroidJavaObject _javaObject;
        private long[] _softly = new long[] { 0, 50, 10, 50 }, _interval = new long[] { 0, 100, 100, 100 }, _acute = new long[] { 0, 300, 100, 300 };


        public void Install()
        {
            _platForm = Application.platform;
            if ( _platForm == RuntimePlatform.Android )
            {
                AndroidJavaClass jd = new AndroidJavaClass( "com.taotao.newshake.MainShake" );
                _javaObject = jd.CallStatic<AndroidJavaObject>( "GetInstans" );
            }
            else if ( _platForm == RuntimePlatform.IPhonePlayer || UnityEngine.iOS.Device.generation.ToString().Contains( "iPad" ) )
            {
                InstantiateFeedbackGenerators();
            }

        }

        public void Uninstall() { }



        public void Shake( VibrateState vibrateState )
        {
            LDebug.Log( "[设置]->震动 " + _platForm );
            switch ( vibrateState )
            {
                case VibrateState.Softly:
                    if ( _platForm == RuntimePlatform.Android )
                    {
                        Shake( _softly );
                    }
                    else if ( _platForm == RuntimePlatform.IPhonePlayer || UnityEngine.iOS.Device.generation.ToString().Contains( "iPad" ) )
                    {
                        SelectionHaptic();
                    }

                    break;
                case VibrateState.Interval:
                    if ( _platForm == RuntimePlatform.Android )
                    {
                        Shake( _interval );
                    }
                    else if ( _platForm == RuntimePlatform.IPhonePlayer || UnityEngine.iOS.Device.generation.ToString().Contains( "iPad" ) )
                    {
                        SuccessHaptic();
                    }

                    break;
                case VibrateState.Acute:
                    if ( _platForm == RuntimePlatform.Android )
                    {
                        Shake( _acute );
                    }
                    else if ( _platForm == RuntimePlatform.IPhonePlayer || UnityEngine.iOS.Device.generation.ToString().Contains( "iPad" ) )
                    {
                        FailureHaptic();
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 延迟毫秒,震动时间,延迟毫秒,震动时间
        /// </summary>0,100,100,100
        /// <param name="pattern">延迟（毫秒）,震动时间（毫秒）,延迟（毫秒）,震动时间（毫秒）</param>
        /// <param name="repeat">-1不循环 2=无限循环</param>
        public void Shake( long[] pattern, int repeat = -1 )
        {
            LDebug.Log( "[设置]->震动 " + _platForm );
            if ( _platForm == RuntimePlatform.Android )
                _javaObject.Call( "UnityCallShake", pattern, repeat );
        }
    }

}

