/*======================================
* 项目名称 ：LitFramework.UI.Manager
* 项目描述 ：
* 类 名 称 ：UIControlManager
* 类 描 述 ：本类包含在UI界面上的各种操作信息及检测，例如是否点击的UI层，持续点击（按压），
*                 返回键操作、抛出对应接口
*                   
* 命名空间 ：LitFramework.UI.Manager
* 机器名称 ：SKY-20170413SEJ 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/5/17 11:50:24
* 更新时间 ：2018/5/17 11:50:24
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ DerekLiu 2018. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2018/5/17 11:50:24
*修改人： LHW
*版本号： V1.0.0.0
*描述：
*
======================================*/

namespace LitFramework.InputSystem
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using UnityEngine.EventSystems;
    using LitFramework.Base;
    using LitFramework.Singleton;
    using System;

    [Flags]
    public enum TouchDirection
    {
        None = 1 << 0,
        Left = 1 << 1,
        Right = 1 << 2,
        Up = 1 << 3,
        Down = 1 << 4,
        OnUI = 1 << 5
    }

    public partial class InputControlManager : SingletonMono<InputControlManager>, IManager
    {
        #region 事件委托
        /// <summary>
        /// 点击返回键
        /// </summary>
        public event Action EscapeCallBack;
        /// <summary>
        /// 是否持续点击触发
        /// </summary>
        public Action<bool> IsTouchedContinuePressCallBack;
        /// <summary>
        /// 是否点击到UI反馈
        /// </summary>
        public Action<bool> IsTouchedOnUICallBack;
        #endregion

        #region 常量
        private const RuntimePlatform PLATFORM_ANDROID = RuntimePlatform.Android;
        private const RuntimePlatform PLATFORM_IOS = RuntimePlatform.IPhonePlayer;
        private const RuntimePlatform PLATFORM_WINDOWEDITOR = RuntimePlatform.WindowsEditor;
        private const RuntimePlatform PLATFORM_IOSEDITOR = RuntimePlatform.OSXEditor;
        private const float MOVE_MIX_DISTANCE = 1f; //滑动距离限制，超过这个距离视为滑动有效
        #endregion

        #region 单指 || PC操作

        //触碰阶段外发函数
        public event Action<Vector2>
            TouchBeganCallback,
            TouchEndCallback,
            TouchStationaryCallback,
            TouchMoveCallback;

        private Vector2 _touchBeginPos;
        private Vector2 _touchEndPos;

        #endregion

        #region 双指 ||  滚轮事件

        /// <summary>
        /// 双指|鼠标滚轮放大程度
        /// </summary>
        public event Action<float> FingerZoomCallback;
        /// <summary>
        /// 双指滑动相对上次记录偏移角度。顺时针计算
        /// </summary>
        public event Action<float> FingerRotateCallBack;

        private Vector2 _oldPosition1;
        private Vector2 _oldPosition2;

        #endregion


        private EventSystem _currentEventSys;
        private float _clockWiseDegree = 0;

        /// <summary>
        /// 当前是否在点击UI
        /// </summary>
        public bool CurrentIsOnUI
        {
            get { return ( _touchResult & TouchDirection.OnUI ) == TouchDirection.OnUI; }
        }
        /// <summary>
        /// 鼠标点击操作功能是否开启
        /// </summary>
        public bool IsEnable
        {
            get
            {
                if ( _isInit )
                    return _isEnable;
                //else
                //    throw new Exception( "UIControlManager is not installed" );
                return true; ;
            }
            set
            {
                if ( _isInit )
                {
                    _isEnable = value;
                    _currentEventSys.enabled = value;
                }
                //else
                //    throw new Exception( "UIControlManager is not installed" );
            }
        }

        private RuntimePlatform _currentPlatform = Application.platform;
        //持续点击灵敏度-持续指定时间视为按压
        private float _curPressTime = 0;
        //private float PRESS_DOWN_SENSITIVITY = 0.5f;

        //是否模块已安装
        private bool _isInit = false;
        private bool _isEnable = false;

        //PC模式下记录的点击坐标
        private Vector3 _recordedPCPos;
        //PC模式下记录静态按压时间
        private float _pressTimeCount = 0f;

        //输出的滑动结果
        private TouchDirection _touchResult = TouchDirection.None;
        private int _uiLayer;

        private FrameworkConfig _config;

        public void Install()
        {
            _isInit = true;
            _isEnable = true;
            _config = FrameworkConfig.Instance;

            var eventsys = GameObject.Find( "EventSystem" );
            if ( eventsys == null )
            {
                eventsys = new GameObject( "EventSystem" );
                eventsys.AddComponent<StandaloneInputModule>();
                eventsys.AddComponent<EventSystem>();
            }
            _currentEventSys = eventsys.GetComponent<EventSystem>();
            _currentEventSys.enabled = true;
            _uiLayer = LayerMask.NameToLayer("UI");

            //给内部方法绑定一个计算当前是否是持续性按压状态
            TouchEndCallback += CalculateTimeByPressOver;
            TouchStationaryCallback += CalculateTimeByPressStart;

            GameDriver.Instance.UpdateEventHandler += OnInputUpdateHandler;
        }

        public void Uninstall()
        {
            GameDriver.Instance.UpdateEventHandler -= OnInputUpdateHandler;

            EscapeCallBack = null;
            TouchBeganCallback = null;
            TouchMoveCallback = null;
            TouchStationaryCallback = null;
            TouchEndCallback = null;
            IsTouchedOnUICallBack = null;
            IsTouchedContinuePressCallBack = null;

            _isInit = false;
            _isEnable = false;
            _currentEventSys = null;

            DoDestroy();
        }

        public int TouchCount { get { return Input.touchCount; } }

        private void OnInputUpdateHandler()
        {
            //点击返回
            if ( Input.GetKeyDown( KeyCode.Escape ) )
                EscapeBtnClick();

            if ( _isInit && IsEnable )
            {
                if ( _currentPlatform == PLATFORM_ANDROID || _currentPlatform == PLATFORM_IOS )
                {
                    //是否点击到UI界面
                    if ( TouchCount > 0 )
                    {
                        _touchResult = GetTouchMoveDirection( _touchResult );
                    }
                    else _touchResult = TouchDirection.None;
                }
                else if ( _currentPlatform == PLATFORM_WINDOWEDITOR || _currentPlatform == PLATFORM_IOSEDITOR )
                {
                    var wheel = Input.GetAxis( "Mouse ScrollWheel" );
                    FingerZoomCallback?.Invoke( wheel );

                    if ( Input.GetMouseButtonDown( 0 ) )
                    {
                        //点击UI检测
                        if ( EventSystem.current.IsPointerOverGameObject() && EventSystem.current.gameObject.layer == _uiLayer )
                        {
                             _touchResult |= TouchDirection.OnUI;
                        }
                        IsTouchedOnUICallBack?.Invoke( CurrentIsOnUI );

                        if ( _config.TouchDetectUI && CurrentIsOnUI ) return;

                        //按压计时
                        CalculateTimeByPressStart( Input.mousePosition );
                        //输出开始点击事件
                        TouchBeganCallback?.Invoke( Input.mousePosition );
                    }

                    if ( Input.GetButton( "Fire1" ) )
                    {
                        if ( _config.TouchDetectUI && CurrentIsOnUI ) return;

                        if ( _recordedPCPos == Vector3.zero )
                        {
                            _pressTimeCount = 0f;
                            _recordedPCPos = Input.mousePosition;
                            return;
                        }
                        if ( _recordedPCPos != Input.mousePosition )
                        {
                            _pressTimeCount = 0f;
                            _recordedPCPos = Input.mousePosition;
                            
                            TouchMoveCallback?.Invoke( Input.mousePosition );
                        }
                        else
                        {
                            _pressTimeCount += Time.deltaTime;
                            if ( _pressTimeCount >= _config.TouchStationaryTime )
                                TouchStationaryCallback?.Invoke( Input.mousePosition );
                        }
                    }

                    if ( Input.GetMouseButtonUp( 0 ) )
                    {
                        _recordedPCPos = Vector2.zero;

                        if ( _config.TouchDetectUI && CurrentIsOnUI ) { _touchResult = TouchDirection.None; return; }
                        TouchEndCallback?.Invoke( Input.mousePosition );
                        CalculateTimeByPressOver( Input.mousePosition );

                        _touchResult = TouchDirection.None;
                    }
                }
            }
        }

        //按住屏幕开始计时
        private void CalculateTimeByPressStart( Vector2 touchPos )
        {
            _curPressTime += Time.deltaTime;
            if ( _curPressTime > _config.TouchStationaryTime && _curPressTime < _config.TouchStationaryTime + Time.deltaTime )
            {
                IsTouchedContinuePressCallBack?.Invoke( true );
            }
        }
        //离开屏幕计时
        private void CalculateTimeByPressOver( Vector2 inputPos )
        {
            if ( _curPressTime > 0 )
                IsTouchedContinuePressCallBack?.Invoke( false );
            _curPressTime = 0;
        }

        //默认-按顺序关闭UI
        private void EscapeBtnClick()
        {
            EscapeCallBack?.Invoke();
        }

        //设置触控方向的【顺时针】旋转角度
        public void SetRotateClockwise( float degree )
        {
            _clockWiseDegree = degree;
        }

        //获取一次单点滑动
        public TouchDirection GetTouchMoveDirection( TouchDirection dirResult )
        {
            if ( TouchCount == 1 )
            {
                //实时检测触碰到UI
                if ( _currentEventSys.IsPointerOverGameObject( Input.touches[ 0 ].fingerId ) && _currentEventSys.gameObject.layer == _uiLayer )
                {
                    dirResult |= TouchDirection.OnUI;

                    IsTouchedOnUICallBack?.Invoke( CurrentIsOnUI );
                }

                if ( Input.touches[ 0 ].phase != TouchPhase.Canceled )
                {
                    Vector2 inputPos = Input.touches[ 0 ].position;
                    switch ( Input.touches[ 0 ].phase )
                    {
                        case TouchPhase.Began:
                            if ( _config.TouchDetectUI && CurrentIsOnUI ) return dirResult;

                            _touchBeginPos = inputPos;
                            TouchBeganCallback?.Invoke( inputPos );
                            break;
                        case TouchPhase.Ended:
                            _touchEndPos = inputPos;
                            if ( _config.TouchDetectUI && CurrentIsOnUI ) return dirResult = TouchDirection.None;

                            TouchEndCallback?.Invoke( inputPos );
                            if ( Vector2.Distance( _touchBeginPos, _touchEndPos ) < MOVE_MIX_DISTANCE )
                                return dirResult;
                            float offSetX = _touchEndPos.x - _touchBeginPos.x;
                            float offSetY = _touchEndPos.y - _touchBeginPos.y;
                            float angle = Mathf.Atan2( offSetY, offSetX ) * Mathf.Rad2Deg;
                            //顺时针偏移
                            angle += _clockWiseDegree;
                            if ( angle > 180 )
                                angle = 360 - angle;
                            else if ( angle < -180 )
                                angle += 360;
                            if ( angle <= 45f && angle > -45f )
                                dirResult |= TouchDirection.Right;
                            else if ( angle > 45f && angle <= 135f )
                                dirResult |= TouchDirection.Up;
                            else if ( ( angle > 135f && angle < 180f ) || ( angle > -180f && angle <= -135f ) )
                                dirResult |= TouchDirection.Left;
                            else if ( angle > -135f && angle <= -45f )
                                dirResult |= TouchDirection.Down;
                            break;
                        case TouchPhase.Moved:
                            if ( _config.TouchDetectUI && CurrentIsOnUI ) return dirResult;
                            TouchMoveCallback?.Invoke( inputPos );
                            break;
                        case TouchPhase.Stationary:
                            if ( _config.TouchDetectUI && CurrentIsOnUI ) return dirResult;
                            TouchStationaryCallback?.Invoke( inputPos );
                            break;
                    }
                }
                else
                    dirResult = TouchDirection.None;
                return dirResult;
            }
            else if( TouchCount > 1 )
            {
                if ( Input.GetTouch( 0 ).phase == TouchPhase.Began ) _oldPosition1 = Input.GetTouch( 0 ).position;
                if ( Input.GetTouch( 1 ).phase == TouchPhase.Began ) _oldPosition2 = Input.GetTouch( 1 ).position;

                if ( Input.GetTouch( 0 ).phase == TouchPhase.Moved || Input.GetTouch( 1 ).phase == TouchPhase.Moved )
                {
                    //计算出当前两点触摸点的位置
                    var tempPosition1 = Input.GetTouch( 0 ).position;
                    var tempPosition2 = Input.GetTouch( 1 ).position;

                    FingerRotateCallBack?.Invoke( LMath.CalcIncludedAngle2D( _oldPosition2 - _oldPosition1, tempPosition2 - tempPosition1 ) );

                    //缩放数据
                    float currentTouchDistance = Vector3.Distance( tempPosition1, tempPosition2 );
                    float lastTouchDistance = Vector3.Distance( _oldPosition1, _oldPosition2 );

                    FingerZoomCallback?.Invoke( ( lastTouchDistance - currentTouchDistance ) * Time.deltaTime );

                    //备份上一次触摸点的位置，用于对比
                    _oldPosition1 = tempPosition1;
                    _oldPosition2 = tempPosition2;
                }
            }
            return dirResult = TouchDirection.None;
        }
    }
}
