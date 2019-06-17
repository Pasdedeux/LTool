#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.LitTool
* 项目描述 ：
* 类 名 称 ：LitTool
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFramework.LitTool
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/8/26 17:04:10
* 更新时间 ：2018/8/26 17:04:10
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Liu Hanwen 2018. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace LitFramework.LitTool
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class LitTool:SingletonMono<LitTool>
    {
        private static MonoBehaviour _mono;
        public static MonoBehaviour monoBehaviour
        {
            get
            {
                if ( _mono == null )
                {
                    GameObject go = new GameObject( "Monobehavior" );
                    monoBehaviour = go.AddComponent<MonoForCorouting>();
                    go.hideFlags = HideFlags.HideAndDontSave; //可见性，以及不可消除性
                }
                return _mono;
            }
            private set { _mono = value; }
        }
        private static WaitForSeconds _waitSeconds;
        private static WaitUntil _waitUntil;


        #region 延迟调用方法
        public static void DelayPlayFunction( float time, System.Action func )
        {
            _waitSeconds = new WaitForSeconds( time );
            monoBehaviour.StartCoroutine( DelayFunction( time, func ) );
        }
        static IEnumerator DelayFunction( float time, System.Action func )
        {
            yield return _waitSeconds;
            func?.Invoke();
        }

        public static void StopDelayPlayFunction()
        {
            monoBehaviour.StopCoroutine( "DelayFunction" );
        }


        public static void WaitUntilFunction( Func<bool> conditionFunc, System.Action func )
        {
            _waitUntil = new WaitUntil( conditionFunc );
            monoBehaviour.StartCoroutine( WaitUntilFunction( func ) );
        }

        static IEnumerator WaitUntilFunction( Action func )
        {
            yield return _waitUntil;
            func?.Invoke();
        }
        #endregion

        #region 时间转换工具

        private static DateTime _dateStart = new DateTime( 1970, 1, 1, 8, 0, 0 );
        private static TimeSpan _timtSpan = new TimeSpan(); 
        /// <summary>
        /// 获取指定显示显示格式的时间跨度表达
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetTimeSpanWithFormat(DateTime startTime, DateTime endTime, string format = "{0:00}:{1:00}")
        {
            _timtSpan = endTime - startTime;
            //todo 尚待扩展
            if ( format.Equals( "{0:00}:{1:00}" ) )
            {
                return string.Format( "{0:00}:{1:00}", _timtSpan.Minutes + (  _timtSpan.Days * 24 * 60  ) , _timtSpan.Seconds );
            }
            else if ( format.Equals( "{0:00}:{1:00}:{2:00}" ) ) 
            {
                return string.Format( "{0:00}:{1:00}:{2:00}", ( _timtSpan.Hours + ( _timtSpan.Days * 24 ) ), _timtSpan.Minutes, _timtSpan.Seconds );
            }
            return string.Format( "{0:00}:{1:00}", _timtSpan.Minutes, _timtSpan.Seconds );
        }

        
        /// <summary>
        /// 获取时间戳Timestamp  
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int GetTimeStamp( DateTime dt )
        {
            int timeStamp = Convert.ToInt32( ( dt - _dateStart ).TotalSeconds );
            return timeStamp;
        }


        /// <summary>
        /// 时间戳Timestamp转换成日期
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime GetDateTime( int timeStamp )
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime( _dateStart );
            long lTime = ( ( long )timeStamp * 10000000 );
            TimeSpan toNow = new TimeSpan( lTime );
            DateTime targetDt = dtStart.Add( toNow );
            return targetDt;
        }

        #endregion

        #region UI格式扩展
        private static List<Component> _textComponets = new List<Component>( 8 );
        public static void CreateLinkStyle( Text target, string contents, string style = "_", bool alignByGeometry = false /*, bool removeAllComponents = true*/ )
        {
            if ( target == null )
                return;
            //克隆Text，获得相同的属性  
            Text underline = Instantiate( target ) as Text;

            //if ( removeAllComponents )
            //{
            //    var components = underline.GetComponents<Component>();
            //    for ( int i = 0; i < components.Length; i++ )
            //    {
            //        var component = components[ i ];
            //        if ( component is RectTransform || component is Text || component is Shadow ) continue;
            //        _textComponets.Add( component );
            //    }
            //    for ( int i = _textComponets.Count - 1; i > -1; i-- )
            //    {
            //        DestroyImmediate( _textComponets[ i ] );
            //    }
            //    _textComponets.Clear();
            //}

            underline.name = "lhw";
            underline.transform.SetParent( target.transform );
            underline.alignByGeometry = alignByGeometry;
            target.text = contents;
            RectTransform rt = underline.rectTransform;
            //设置下划线坐标和位置  
            rt.anchoredPosition3D = Vector3.zero;
            rt.offsetMax = Vector2.zero;
            rt.offsetMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.anchorMin = Vector2.zero;
            underline.text = style;
            float perlineWidth = underline.preferredWidth;      //单个下划线宽度  
            LDebug.Log( perlineWidth.ToString() );
            float width = target.preferredWidth;
            LDebug.Log( width.ToString() );
            int lineCount = ( int )Mathf.Round( width / perlineWidth );
            LDebug.Log( lineCount.ToString() );

            StringBuilder sb = new StringBuilder();
            for ( int k = 0; k < lineCount; k++ )
            {
                sb.Append( style );
            }
            underline.text += sb.ToString();
            underline.transform.localScale = Vector3.one;
        }
        #endregion
    }


    public class MonoForCorouting : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad( this );
        }
    }
}
