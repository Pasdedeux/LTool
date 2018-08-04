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
    using LitFramework.UI.Base;
    using System;

    public class InputControlManager : SingletonMono<InputControlManager>, IManager
    {
        #region 事件委托
        /// <summary>
        /// 点击返回键
        /// </summary>
        public event Action EscapeCallBack;
        /// <summary>
        /// 持续点击触发
        /// </summary>
        public event Action<bool> TouchedContinuePressCallBack;
        /// <summary>
        /// 是否点击到UI反馈
        /// </summary>
        public event Action<bool> TouchedOnUICallBack;
        /// <summary>
        /// 开始触屏
        /// </summary>
        public event Action<Vector2> TouchedBeganCallBack;
        /// <summary>
        /// 滑动事件反馈
        /// </summary>
        public event Action<Vector2> TouchedMoveScreenPosCallBack;
        #endregion

        #region 常量
        private const RuntimePlatform PLATFORM_ANDROID = RuntimePlatform.Android;
        private const RuntimePlatform PLATFORM_IOS = RuntimePlatform.IPhonePlayer;
        private const RuntimePlatform PLATFORM_WINDOWEDITOR = RuntimePlatform.WindowsEditor;
        private const RuntimePlatform PLATFORM_IOSEDITOR = RuntimePlatform.OSXEditor;
        #endregion

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
        //滑动方向捕捉器
        private TouchDirectionControl _touchDirectionCalculator;
        //持续点击灵敏度-持续指定时间视为按压
        private float _curPressTime = 0;
        private const float PRESS_DOWN_SENSITIVITY = 0.5f;

        //是否模块已安装
        private bool _isInit = false;
        private bool _isEnable = false;

        //输出的滑动结果
        private TouchDirection _touchResult = TouchDirection.None;

        public void Install()
        {
            _isInit = true;
            _isEnable = true;

            _touchDirectionCalculator = TouchDirectionControl.Instance;
            _touchDirectionCalculator.TouchBeganCallback = TouchedBeganCallBack;
            _touchDirectionCalculator.TouchEndCallback = CalculateTimeByPressOver;
            _touchDirectionCalculator.TouchStationaryCallback = CalculateTimeByPressStart;
            _touchDirectionCalculator.TouchMoveCallback = TouchedMoveScreenPosCallBack;

            if(GameObject.Find( "EventSystem" )==null)
            {
                GameObject go = new GameObject( "EventSystem" );
                go.AddComponent<EventSystem>();
                go.AddComponent<StandaloneInputModule>();
            }
        }

        public void Uninstall()
        {
            _isInit = false;
            _isEnable = false;

            _touchDirectionCalculator.TouchEndCallback = null;
            _touchDirectionCalculator.TouchMoveCallback = null;
            _touchDirectionCalculator.TouchBeganCallback = null;
            _touchDirectionCalculator.TouchStationaryCallback = null;
            _touchDirectionCalculator.DoDestroy();
            _touchDirectionCalculator = null;

            EscapeCallBack = null;
            TouchedOnUICallBack = null;
            TouchedBeganCallBack = null;
            TouchedContinuePressCallBack = null;
            TouchedMoveScreenPosCallBack = null;

            DoDestroy();
        }


        void Update()
        {
            //点击返回
            if (Input.GetKeyDown( KeyCode.Escape ))
                CloseUIByOrder();

            if ( _isInit && IsEnable )
            {
                if ( _currentPlatform == PLATFORM_ANDROID || _currentPlatform == PLATFORM_IOS )
                {
                    //是否点击到UI界面
                    if ( Input.touchCount > 0 )
                    {
                        _touchResult =_touchDirectionCalculator.GetTouchMoveDirection( _touchResult );
                        if( CurrentIsOnUI )
                            TouchedOnUICallBack?.Invoke( true );
                    }
                    else _touchResult = TouchDirection.None;
                }
                else if( _currentPlatform == PLATFORM_WINDOWEDITOR || _currentPlatform == PLATFORM_IOSEDITOR )
                {
                    if ( Input.GetMouseButtonDown(0) )
                    {
                        //点击UI检测
                        if ( EventSystem.current.IsPointerOverGameObject() )
                        {
                            _touchResult |= TouchDirection.OnUI;
                            TouchedOnUICallBack?.Invoke( true );
                        }
                        else
                            TouchedOnUICallBack?.Invoke( false );
                        //按压计时
                        CalculateTimeByPressStart( Input.mousePosition );
                        //输出开始点击事件
                        TouchedBeganCallBack?.Invoke( Input.mousePosition );
                    }

                    if ( Input.GetMouseButtonUp(0))
                    {
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
                TouchedContinuePressCallBack?.Invoke( true );
        }
        /// <summary>
        /// 离开屏幕计时
        /// </summary>
        private void CalculateTimeByPressOver( Vector2 inputPos )
        {
            if ( _curPressTime > 0 )
                TouchedContinuePressCallBack?.Invoke( false );
            _curPressTime = 0;
        }

        /// <summary>
        /// 默认-按顺序关闭UI
        /// </summary>
        /// <param name="extendedFunc">采用自定义函数执行返回键功能</param>
        private void CloseUIByOrder()
        {
            EscapeCallBack?.Invoke();
        }
    }
}
