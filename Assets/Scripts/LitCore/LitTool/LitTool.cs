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
* Copyright @ Derek Liu 2018. All rights reserved.
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
    public class LitTool : SingletonMono<LitTool>
    {
        //驱动器
        private static GameDriver _driver;

        private static MonoBehaviour _mono;
        /// <summary>
        /// 协程全局使用mono
        /// </summary>
        public static MonoBehaviour MonoBehaviour
        {
            get
            {
                if ( _mono == null )
                {
                    GameObject go = new GameObject( "Monobehavior" );
                    MonoBehaviour = go.AddComponent<MonoForCorouting>();
                    go.hideFlags = HideFlags.HideAndDontSave; //可见性，以及不可消除性
                }
                return _mono;
            }
            private set { _mono = value; }
        }

        #region 延迟调用
        private static float _delayFuncTimeCouting = 0f;  //延迟方法当前计时
        private static float _delayFuncWaitTimeMax = FrameworkConfig.Instance.DelayFuncDetectInterver;
        private static bool _usePreciseMode = FrameworkConfig.Instance.UseDelayFuncPreciseDetect;
        /// <summary>
        /// 延迟是否使用精准计时。True - update每帧执行。False - 每间隔_delayFuncWaitTimeInterval时间执行一次延迟方法遍历
        /// </summary>
        public static bool UsePreciseModeForDelayFunc
        {
            get { return _usePreciseMode; }
            set
            {
                _usePreciseMode = value;
                if ( !value )
                {
                    _delayFuncWaitTimeMax = FrameworkConfig.Instance.DelayFuncDetectInterver;
                    _delayFuncTimeCouting = 0f;
                }
            }
        }

        //受限时影响的委托
        internal static event Action<float> DelayFuncRealEvent; //忽略TimeScale方式
        internal static event Action<float> DelayFuncEvent;       //受TimeScale影响方式

        /// <summary>
        /// LToolUpdate
        /// </summary>
        private static void BindingUpdate()
        {
            if ( !UsePreciseModeForDelayFunc )
            {
                //每X秒触发一次分发
                _delayFuncTimeCouting += Time.unscaledDeltaTime;
                if ( _delayFuncTimeCouting >= _delayFuncWaitTimeMax )
                {
                    _delayFuncTimeCouting = 0f;
                    //LDebug.Log( DelayFuncRealEvent?.GetInvocationList().Length );

                    DelayFuncEvent?.Invoke( Time.time );
                    DelayFuncRealEvent?.Invoke( Time.unscaledTime );
                }
            }
            //逐帧遍历
            else
            {
                DelayFuncEvent?.Invoke( Time.time );
                DelayFuncRealEvent?.Invoke( Time.unscaledTime );
            }
        }

        /// <summary>
        /// 延迟调用方法
        /// </summary>
        /// <param name="time">延迟时间</param>
        /// <param name="func">延迟时间结束后回调函数。暂不支持主动取消，使用过程中需要注意</param>
        /// <param name="useIgnoreTimeScale">是否忽略TimeScale，默认为true</param>
        /// <param name="useUpdate">使用update方式或者协程方式，默认是update方式</param>
        public static void DelayPlayFunction( float time, Action func, bool useIgnoreTimeScale = true, bool useUpdate = true )
        {
            if ( _driver == null ) { _driver = GameDriver.Instance; _driver.UpdateEventHandler += BindingUpdate; }

            if ( useUpdate ) DelayPlayFuncUpdate( time, func, useIgnoreTimeScale );
            else DelayPlayFuncMono( time, func, useIgnoreTimeScale );
        }

        #region 协程方案

        /// <summary>
        /// 延迟调用方法（协程方案）
        /// </summary>
        /// <param name="time">等待时间，秒</param>
        /// <param name="func">时间到了回调函数</param>
        /// <param name="realTime">是否是真实时间（忽略TimeScale）</param>
        static void DelayPlayFuncMono( float time, Action func, bool realTime )
        {
            if ( realTime )
            {
                MonoBehaviour.StartCoroutine( DelayFunctionReal( time, func ) );
            }
            else
            {
                MonoBehaviour.StartCoroutine( DelayFunction( time, func ) );
            }

        }
        static IEnumerator DelayFunction( float time, Action func )
        {
            yield return new WaitForSeconds( time );
            func?.Invoke();
        }
        static IEnumerator DelayFunctionReal( float time, Action func )
        {
            yield return new WaitForSecondsRealtime( time );
            func?.Invoke();
        }

        /// <summary>
        /// WaitUtil方法
        /// </summary>
        /// <param name="conditionFunc">通过需要的判定条件</param>
        /// <param name="func">通过达成后的回调函数</param>
        public static void WaitUntilFunction( Func<bool> conditionFunc, Action func )
        {
            MonoBehaviour.StartCoroutine( IWaitUntilFunction( conditionFunc, func ) );
        }
        static IEnumerator IWaitUntilFunction( Func<bool> conditionFunc, Action func )
        {
            yield return new WaitUntil( conditionFunc );
            func?.Invoke();
        }

        #endregion

        #region Update方案

        /// <summary>
        /// 延迟调用方法（Update计时方案）
        /// </summary>
        /// <param name="time">等待时间，秒</param>
        /// <param name="func">时间到了回调函数</param>
        /// <param name="realTime">是否是真实时间（忽略TimeScale）</param>
        static void DelayPlayFuncUpdate( float time, Action func, bool realTime )
        {
            new DelayFuncDecoration( realTime ? Time.unscaledTime + time : Time.time + time, func, realTime );
        }

        #endregion

        #endregion

        #region 时间转换工具

        private static DateTime _dateStartUTC = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
        private static DateTime _dateStart = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Local );
        private static TimeSpan _timtSpan = new TimeSpan();

        /// <summary>
        /// 获取指定显示显示格式的时间跨度表达
        /// 
        /// {0:00}:{1:00} 分/秒   累加到分钟数
        /// {0:00}:{1:00}:{2:00} 时/分/秒   累加到小时数
        /// </summary>
        /// <param name="startTime">起点日期</param>
        /// <param name="endTime">结束日期</param>
        /// <param name="format">返回的日期格式</param>
        /// <returns></returns>
        public static string GetTimeSpanWithFormat( DateTime startTime, DateTime endTime, string format = "{0:00}:{1:00}" )
        {
            _timtSpan = endTime - startTime;
            //todo 尚待扩展
            if ( format.Equals( "{0:00}:{1:00}" ) )
            {
                return string.Format( "{0:00}:{1:00}", _timtSpan.Minutes + ( _timtSpan.Days * 24 * 60 ), _timtSpan.Seconds );
            }
            else if ( format.Equals( "{0:00}:{1:00}:{2:00}" ) )
            {
                return string.Format( "{0:00}:{1:00}:{2:00}", ( _timtSpan.Hours + ( _timtSpan.Days * 24 ) ), _timtSpan.Minutes, _timtSpan.Seconds );
            }
            return string.Format( "{0:00}:{1:00}", _timtSpan.Minutes, _timtSpan.Seconds );
        }

        /// <summary>
        /// 获取指定显示显示格式的时间跨度表达
        /// </summary>
        /// <param name="span"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetTimeSpanWithFormat( TimeSpan span, string format = "{0:00}:{1:00}" )
        {
            //todo 尚待扩展
            if ( format.Equals( "{0:00}:{1:00}" ) )
            {
                return string.Format( "{0:00}:{1:00}", span.Minutes + span.Days * 24 * 60 + span.Hours * 60, span.Seconds );
            }
            else if ( format.Equals( "{0:00}:{1:00}:{2:00}" ) )
            {
                return string.Format( "{0:00}:{1:00}:{2:00}", span.Hours + span.Days * 24, span.Minutes, span.Seconds );
            }
            return string.Format( "{0:00}:{1:00}", span.Minutes + span.Days * 24 * 60 + span.Hours * 60, span.Seconds );
        }


        /// <summary>
        /// 获取当前UTC时间戳Timestamp
        /// </summary>
        /// <returns></returns>
        public static long GetUTCTimeStamp()
        {
            long timeStamp = Convert.ToInt64( ( DateTime.UtcNow - _dateStartUTC ).TotalSeconds );
            return timeStamp;
        }

        /// <summary>
        /// 获取UTC时间戳Timestamp
        /// </summary>
        /// <param name="dt">UTC日期</param>
        /// <returns></returns>
        public static long GetUTCTimeStamp( DateTime dt )
        {
            long timeStamp = Convert.ToInt64( ( dt - _dateStartUTC ).TotalSeconds );
            return timeStamp;
        }

        /// <summary>
        /// UTC 时间戳Timestamp转换成UTC日期
        /// </summary>
        /// <param name="timeStamp">需要转换的时间戳秒</param>
        /// <returns>返回的UTC日期</returns>
        public static DateTime GetUTCDateTime( long timeStamp )
        {
            long lTime = timeStamp * 10000000;
            TimeSpan toNow = new TimeSpan( lTime );
            DateTime targetDt = _dateStartUTC.Add( toNow );
            return targetDt;
        }


        /// <summary>
        /// 获取当前时间戳Timestamp
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            long timeStamp = Convert.ToInt64( ( DateTime.Now - _dateStart ).TotalSeconds );
            return timeStamp;
        }

        /// <summary>
        /// 获取时间戳Timestamp
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns></returns>
        public static long GetTimeStamp( DateTime dt )
        {
            long timeStamp = Convert.ToInt64( ( dt - _dateStart ).TotalSeconds );
            return timeStamp;
        }

        /// <summary>
        /// 时间戳Timestamp转换成日期
        /// </summary>
        /// <param name="timeStamp">需要转换的时间戳秒</param>
        /// <returns>返回的日期</returns>
        public static DateTime GetDateTime( long timeStamp )
        {
            long lTime = timeStamp * 10000000;
            TimeSpan toNow = new TimeSpan( lTime );
            DateTime targetDt = _dateStart.Add( toNow );
            return targetDt;
        }


        #endregion

        #region UI格式扩展
        /// <summary>
        /// 世界坐标转UI坐标
        /// </summary>
        /// <param name="targetWorldPos">对象世界坐标</param>
        /// <param name="mainCam">主摄像机</param>
        /// <param name="uiCam">UI相机</param>
        /// <param name="uiCanvas">被放置的UICANVAS节点</param>
        /// <returns>被放置的UI世界坐标，设置其 transform.position即可。（没有返回UI坐标的Vector2是为了避免容器父节点坐标影响)</returns>
        public static Vector3 World2UIPos( Vector3 targetWorldPos, Camera mainCam, Camera uiCam, RectTransform uiCanvas )
        {
            Vector3 result = Vector3.zero;
            //屏幕转UI  ui(当前的canvas)  _camera_UiCamera(UI的摄像机)
            var vec3 = mainCam.WorldToScreenPoint( targetWorldPos );
            RectTransformUtility.ScreenPointToWorldPointInRectangle( uiCanvas, vec3, uiCam, out result );
            return result;
        }

        /// <summary>
        /// UI坐标转世界坐标
        /// </summary>
        /// <param name="uiTarget">目标UI对象</param>
        /// <returns></returns>
        public static Vector3 UI2WorldPos( RectTransform uiTarget )
        {
            CanvasScaler canvasScaler = null;
            if ( FrameworkConfig.Instance.UseHotFixMode )
                canvasScaler = LitFramework.HotFix.UIManager.Instance.CanvasScaler;
            else
                canvasScaler = LitFramework.Mono.UIManager.Instance.CanvasScaler;
            
            var reference = canvasScaler.referenceResolution;

            Vector2 targetAnchored = uiTarget.anchoredPosition;
            Vector2 screenPos;
            switch ( canvasScaler.screenMatchMode )
            {
                case CanvasScaler.ScreenMatchMode.MatchWidthOrHeight:
                    var scale = canvasScaler.matchWidthOrHeight == 0 ? Screen.width / reference.x : Screen.height / reference.y;
                    targetAnchored *= scale;
                    break;
                case CanvasScaler.ScreenMatchMode.Expand:
                case CanvasScaler.ScreenMatchMode.Shrink:
                    var scaleX = Screen.width / reference.x;
                    var scaleY = Screen.height / reference.y;
                    targetAnchored.y *= scaleY;
                    targetAnchored.x *= scaleX;
                    break;
            }
            screenPos = targetAnchored + 0.5f * new Vector2( Screen.width, Screen.height );

            var worldPos = new Vector3();

            if ( FrameworkConfig.Instance.UseHotFixMode )
                RectTransformUtility.ScreenPointToWorldPointInRectangle( LitFramework.HotFix.UIManager.Instance.RectransRoot, screenPos, LitFramework.HotFix.UIManager.Instance.UICam, out worldPos );
            else
                RectTransformUtility.ScreenPointToWorldPointInRectangle( LitFramework.Mono.UIManager.Instance.RectransRoot, screenPos, LitFramework.Mono.UIManager.Instance.UICam, out worldPos );

            return worldPos;
        }

        /// <summary>
        /// 屏幕坐标转3D坐标系的本地坐标
        /// </summary>
        /// <param name="screenPos">屏幕坐标</param>
        /// <param name="parent">3D位置在世界中的父对象</param>
        /// <param name="mainCam">所参照的世界主相机</param>
        /// <returns></returns>
        public static Vector3 ScreenToLocalPos( Vector3 screenPos, Transform parent, Camera mainCam )
        {
            Vector3 point = new Vector3( screenPos.x, screenPos.y, -mainCam.transform.position.z );
            Vector3 worldPos = mainCam.ScreenToWorldPoint( point );
            return parent.worldToLocalMatrix.MultiplyPoint( worldPos );
        }

        /// <summary>
        /// 世界坐标向特定画布坐标转换
        /// </summary>
        /// <param name="canvas">画布</param>
        /// <param name="screen">屏幕坐标</param>
        /// <param name="uiCam">UICam</param>
        /// <returns>返回画布上的二维坐标</returns>
        public static Vector2 ScreenToCanvasPos( Canvas canvas, Vector3 screen, Camera uiCam )
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle( canvas.transform as RectTransform,
                screen, uiCam, out Vector2 position );
            return position;
        }

        /// <summary>
        /// 动态对文本创建下划线。Text组件
        /// </summary>
        /// <param name="target">Text组件</param>
        /// <param name="contents">显示文本内容</param>
        /// <param name="style">下滑线类型，默认为'_'</param>
        /// <param name="alignByGeometry">是否使用几何对齐</param>
        /// <param name="richText">是否支持富文本</param>
        public static void CreateLinkStyle( Text target, string contents, string style = "_", bool alignByGeometry = false, bool richText = false )
        {
            if ( target == null )
                return;
            //克隆Text，获得相同的属性  
            Text underline = Instantiate( target ) as Text;

            underline.name = "lhw";
            underline.transform.SetParent( target.transform );
            underline.alignByGeometry = alignByGeometry;
            target.text = contents;
            target.supportRichText = richText;
            RectTransform rt = underline.rectTransform;
            //设置下划线坐标和位置  
            rt.anchoredPosition3D = Vector3.zero;
            rt.offsetMax = Vector2.zero;
            rt.offsetMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.anchorMin = Vector2.zero;
            underline.text = style;

            float perlineWidth = underline.preferredWidth;      //单个下划线宽度  
            float width = target.preferredWidth;
            int lineCount = ( int )Mathf.Round( width / perlineWidth );

            StringBuilder sb = new StringBuilder();
            for ( int k = 0; k < lineCount; k++ )
            {
                sb.Append( style );
            }
            underline.text += sb.ToString();
            underline.transform.localScale = Vector3.one;
        }
        #endregion

        #region 图集加载

        private Dictionary<string, Sprite[]> _atlasDict = new Dictionary<string, Sprite[]>();

        /// <summary>
        /// Resource - 加载图集中的子对象
        /// </summary>
        /// <param name="spriteName"></param>
        /// <param name="atlasPath"></param>
        /// <returns></returns>
        public Sprite LoadSpriteAtlas( string spriteName, string atlasPath = null )
        {
            //常驻内存
            Sprite sprite = Resources.Load<Sprite>( spriteName );

            if ( sprite != null || string.IsNullOrEmpty( atlasPath ) )
            {
                return GameObject.Instantiate<Sprite>( sprite );
            }
            if ( !_atlasDict.ContainsKey( atlasPath ) )
            {
                Sprite[] atlasSprites = Resources.LoadAll<Sprite>( atlasPath );
                _atlasDict.Add( atlasPath, atlasSprites );
            }

            var sprites = _atlasDict[ atlasPath ];
            var length = _atlasDict[ atlasPath ].Length;
            for ( int i = 0; i < length; i++ )
            {
                if ( sprites[ i ].name.Equals( string.Concat( new string[] { atlasPath, "_", spriteName } ) ) )

                    return sprite = sprites[ i ];
            }
            return GameObject.Instantiate<Sprite>( sprite );
        }
        #endregion
    }

    /// <summary>
    /// 全局使用的monobehaivor
    /// </summary>
    public class MonoForCorouting : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad( this );
        }
    }

    /// <summary>
    /// 延迟调用函数包装类
    /// </summary>
    class DelayFuncDecoration : IDisposable
    {
        private Action _callBackFunc;
        private float _targetTime;
        private bool _isReal;

        public DelayFuncDecoration( float targetTime, Action func, bool useReal )
        {
            _targetTime = targetTime;
            _callBackFunc = func;
            _isReal = useReal;

            if ( _isReal ) LitTool.DelayFuncRealEvent += DelayFuncEventHandler;
            else LitTool.DelayFuncEvent += DelayFuncEventHandler;
        }


        /// <summary>
        /// 时间判定回调
        /// </summary>
        /// <param name="nowTime"></param>
        private void DelayFuncEventHandler( float nowTime )
        {
            if ( nowTime >= _targetTime )
            {
                _callBackFunc?.Invoke();

                if ( _isReal ) LitTool.DelayFuncRealEvent -= DelayFuncEventHandler;
                else LitTool.DelayFuncEvent -= DelayFuncEventHandler;

                Dispose();
            }
        }


        public void Dispose()
        {
            _callBackFunc = null;
        }
    }
}
