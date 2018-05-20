/*======================================
* 项目名称 ：LitFramework.UI.Manager
* 项目描述 ：
* 类 名 称 ：UIControlManager
* 类 描 述 ：本类包含在UI界面上的各种操作信息及检测，例如是否点击的UI层，持续点击（按压），抛出对应接口
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

namespace LitFramework.UI.Manager
{
#if UNITY_ANDROID && !UNITY_EDITOR
#define ANDROID
#endif


#if UNITY_IPHONE && !UNITY_EDITOR
#define IPHONE
#endif

    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using UnityEngine.EventSystems;
    using LitFramework.UI.Base;
    using System;

    public class UIControlManager : SingletonMono<UIControlManager>, IManager
    {
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
                if( _isInit )
                    return _isEnable;
                else
                {
                    throw new Exception( "UIControlManager is not installed" );
                }
            }
            set
            {
                if( _isInit )
                    _isEnable = value;
                else
                {
                    throw new Exception( "UIControlManager is not installed" );
                }
            }
        }
        private bool _isEnable = false;

        /// <summary>
        /// 是否模块已安装
        /// </summary>
        private bool _isInit = false;

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
            if( _isInit && IsEnable )
            {
                if( Input.GetMouseButtonUp( 0 ) || ( Input.touchCount > 0 && Input.GetTouch( 0 ).phase == TouchPhase.Began ) )
                {
#if IPHONE || ANDROID
                    if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) )
#else
                    if( EventSystem.current.IsPointerOverGameObject() )
#endif
                    {
                        Debug.Log( "====> 点击到UI" );
                        CurrentIsOnUI = true;
                    }
                    else
                    {
                        Debug.Log( "====> 没有点击UI" );
                        CurrentIsOnUI = false;
                    }
                }

            }
        }
    }
}
