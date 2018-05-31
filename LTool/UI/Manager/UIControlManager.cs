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

namespace LitFramework.UI.Extended
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using UnityEngine.EventSystems;
    using LitFramework.UI.Base;
    using System;

    public class UIControlManager : SingletonMono<UIControlManager>, IManager
    {
        /// <summary>
        /// 点击返回键
        /// </summary>
        public event Action EscapeCallBack;
        /// <summary>
        /// 点击到UI上时事件绑定
        /// </summary>
        public event Action<bool> IsTouchedOnUICallBack;
        /// <summary>
        /// 持续点击触发
        /// </summary>
        public event Action<bool> TouchedContinuePressCallBack;
        /// <summary>
        /// 滑动事件反馈
        /// </summary>
        public event Action<Vector2> TouchedMoveScreenPosCallBack;


        /// <summary>
        /// 当前是否在点击UI
        /// </summary>
        public bool CurrentIsOnUI { get; private set; }
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

        //持续点击灵敏度-持续指定时间视为按压
        private float _curPressTime = 0;
        private const float PRESS_DOWN_SENSITIVITY = 0.5f;

        private const TouchPhase began = TouchPhase.Began;
        private const TouchPhase moved = TouchPhase.Moved;
        private const TouchPhase ended = TouchPhase.Ended;
        private const TouchPhase canceled = TouchPhase.Canceled;
        private const TouchPhase stationary = TouchPhase.Stationary;

        //是否模块已安装
        private bool _isInit = false;
        private bool _isEnable = false;

        public void Install()
        {
            _isInit = true;

            IsEnable = true;
        }

        public void Uninstall()
        {
            _isInit = false;

            IsEnable = false;
            DoDestroy();
        }





        void Update()
        {
            //点击返回
            if (Input.GetKeyDown( KeyCode.Escape ))
                CloseUIByOrder();

            if ( _isInit && IsEnable )
            {
                if ( Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer )
                {
                    //是否点击到UI界面
                    if ( Input.touchCount > 0 )
                    {
                        var touch0 = Input.GetTouch( 0 );
                        if ( touch0.phase == began )
                        {
                            if ( EventSystem.current.IsPointerOverGameObject( Input.GetTouch( 0 ).fingerId ) )
                            {
                                CurrentIsOnUI = true;
                                IsTouchedOnUICallBack?.Invoke( true );
                            }
                            else
                            {
                                CurrentIsOnUI = false;
                                IsTouchedOnUICallBack?.Invoke( false );
                            }
                        }
                        else if ( touch0.phase == moved )
                        {
                            TouchedMoveScreenPosCallBack?.Invoke( touch0.position );
                        }
                        else if ( touch0.phase == stationary )
                            CalculateTimeByPressStart();
                        else if ( touch0.phase == ended || touch0.phase == canceled )
                            CalculateTimeByPressOver();

                    }

                }
                else if( Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor )
                {
                    if ( Input.GetButton( "Fire1" ) )
                    {
                        CalculateTimeByPressStart();

                        //点击UI检测
                        if ( EventSystem.current.IsPointerOverGameObject() )
                        {
                            CurrentIsOnUI = true;
                            IsTouchedOnUICallBack?.Invoke( true );
                        }
                        else
                        {
                            CurrentIsOnUI = false;
                            IsTouchedOnUICallBack?.Invoke( false );
                        }
                    }

                    if ( Input.GetButtonUp( "Fire1" ) )
                        CalculateTimeByPressOver();
                }
            }
        }

        /// <summary>
        /// 按住屏幕开始计时
        /// </summary>
        private void CalculateTimeByPressStart()
        {
            _curPressTime += Time.deltaTime;
            if ( _curPressTime > PRESS_DOWN_SENSITIVITY && _curPressTime < PRESS_DOWN_SENSITIVITY + Time.deltaTime )
                TouchedContinuePressCallBack?.Invoke( true );
        }
        /// <summary>
        /// 离开屏幕计时
        /// </summary>
        private void CalculateTimeByPressOver()
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
