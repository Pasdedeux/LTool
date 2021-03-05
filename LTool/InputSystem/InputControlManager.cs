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

namespace LitFramework.Input
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using UnityEngine.EventSystems;
    using LitFramework.Base;
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

    public class InputControlManager : SingletonMono<InputControlManager>, IManager
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

        //触碰阶段外发函数
        public event Action<Vector2> TouchBeganCallback, TouchEndCallback, TouchStationaryCallback, TouchMoveCallback;

        private Vector2 _touchBeginPos;
        private Vector2 _touchEndPos;
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
                else
                    throw new Exception( "UIControlManager is not installed" );
            }
            set
            {
                if ( _isInit )
                    _isEnable = value;
                else
                    throw new Exception( "UIControlManager is not installed" );
            }
        }

        private RuntimePlatform _currentPlatform = Application.platform;
        //持续点击灵敏度-持续指定时间视为按压
        private float _curPressTime = 0;
        private float PRESS_DOWN_SENSITIVITY = 0.5f;

        //是否模块已安装
        private bool _isInit = false;
        private bool _isEnable = false;

        //输出的滑动结果
        private TouchDirection _touchResult = TouchDirection.None;

        public void Install()
        {
            _isInit = true;
            _isEnable = true;

            var eventsys = GameObject.Find( "EventSystem" );
            if ( eventsys == null )
            {
                eventsys = new GameObject( "EventSystem" );
                eventsys.AddComponent<StandaloneInputModule>();
                eventsys.AddComponent<EventSystem>();
            }
            _currentEventSys = eventsys.GetComponent<EventSystem>();

            //给内部方法绑定一个计算当前是否是持续性按压状态
            TouchEndCallback += CalculateTimeByPressOver;
            TouchStationaryCallback += CalculateTimeByPressStart;
        }

        public void Uninstall()
        {
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


        public void InputUpdateHandler()
        {
            //点击返回
            if ( Input.GetKeyDown( KeyCode.Escape ) )
                EscapeBtnClick();

            if ( _isInit && IsEnable )
            {
                if ( _currentPlatform == PLATFORM_ANDROID || _currentPlatform == PLATFORM_IOS )
                {
                    //是否点击到UI界面
                    if ( Input.touchCount > 0 )
                    {
                        _touchResult = GetTouchMoveDirection( _touchResult );
                        if ( CurrentIsOnUI )
                            IsTouchedOnUICallBack?.Invoke( true );
                    }
                    else _touchResult = TouchDirection.None;
                }
                else if ( _currentPlatform == PLATFORM_WINDOWEDITOR || _currentPlatform == PLATFORM_IOSEDITOR )
                {
                    if ( Input.GetMouseButtonDown( 0 ) )
                    {
                        //点击UI检测
                        if ( EventSystem.current.IsPointerOverGameObject() )
                        {
                            _touchResult |= TouchDirection.OnUI;
                            IsTouchedOnUICallBack?.Invoke( true );
                        }
                        else
                            IsTouchedOnUICallBack?.Invoke( false );
                        //按压计时
                        CalculateTimeByPressStart( Input.mousePosition );
                        //输出开始点击事件
                        TouchBeganCallback?.Invoke( Input.mousePosition );
                    }

                    if ( Input.GetButton( "Fire1" ) )
                    {
                        TouchMoveCallback?.Invoke( Input.mousePosition );
                    }

                    if ( Input.GetMouseButtonUp( 0 ) )
                    {
                        TouchEndCallback?.Invoke( Input.mousePosition );
                        CalculateTimeByPressOver( Input.mousePosition );

                        _touchResult = TouchDirection.None;
                    }
                }
            }
        }

        /// <summary>
        /// 按住屏幕开始计时
        /// </summary>
        private void CalculateTimeByPressStart( Vector2 touchPos )
        {
            _curPressTime += Time.deltaTime;
            if ( _curPressTime > PRESS_DOWN_SENSITIVITY && _curPressTime < PRESS_DOWN_SENSITIVITY + Time.deltaTime )
                IsTouchedContinuePressCallBack?.Invoke( true );
        }
        /// <summary>
        /// 离开屏幕计时
        /// </summary>
        private void CalculateTimeByPressOver( Vector2 inputPos )
        {
            if ( _curPressTime > 0 )
                IsTouchedContinuePressCallBack?.Invoke( false );
            _curPressTime = 0;
        }

        /// <summary>
        /// 默认-按顺序关闭UI
        /// </summary>
        /// <param name="extendedFunc">采用自定义函数执行返回键功能</param>
        private void EscapeBtnClick()
        {
            EscapeCallBack?.Invoke();
        }

        /// <summary>
        /// 设置触控方向的【顺时针】旋转角度
        /// </summary>
        /// <param name="degree"></param>
        public void SetRotateClockwise( float degree )
        {
            _clockWiseDegree = degree;
        }


        /// <summary>
        /// 获取一次滑动行为
        /// </summary>
        /// <returns></returns>
        public TouchDirection GetTouchMoveDirection( TouchDirection dirResult )
        {
            if ( Input.touchCount > 0 )
            {
                //实时检测触碰到UI
                if ( _currentEventSys.IsPointerOverGameObject( Input.touches[ 0 ].fingerId ) )
                    dirResult |= TouchDirection.OnUI;

                if ( Input.touches[ 0 ].phase != TouchPhase.Canceled )
                {
                    Vector2 inputPos = Input.touches[ 0 ].position;
                    switch ( Input.touches[ 0 ].phase )
                    {
                        case TouchPhase.Began:
                            _touchBeginPos = inputPos;
                            TouchBeganCallback?.Invoke( inputPos );
                            break;
                        case TouchPhase.Ended:
                            _touchEndPos = inputPos;
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
                            TouchMoveCallback?.Invoke( inputPos );
                            break;
                        case TouchPhase.Stationary:
                            TouchStationaryCallback?.Invoke( inputPos );
                            break;
                    }
                }
                return dirResult;
            }
            else
                return dirResult = TouchDirection.None;
        }

    }
}
