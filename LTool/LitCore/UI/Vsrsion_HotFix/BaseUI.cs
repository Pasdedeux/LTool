﻿/**************************************************************** 
 * 作    者：Derek Liu 
 * CLR 版本：4.0.30319.42000 
 * 创建时间：2018/1/31 15:48:18 
 * 当前版本：1.0.0.1 
 *  
 * 描述说明： 
 * 
 * 修改历史： 
 * 
***************************************************************** 
 * Copyright @ Derek Liu 2018 All rights reserved 
*****************************************************************/

using LitFramework.UI.Base;
using System;
using System.Collections;
using UnityEngine;

namespace LitFramework.HotFix
{
    public abstract class BaseUI : UI.Base.IBaseUI
    {
        private RectTransform _rootRectTransform;
        /// <summary>
        /// 该窗口是否开启中
        /// </summary>
        public bool IsShowing { get; set; }
        private UIType _uiType = new UIType();
        /// <summary>
        /// 当前窗口类型
        /// </summary>
        public UIType CurrentUIType
        { get { return _uiType; } set { _uiType = value; } }
        /// <summary>
        /// 是否执行过Start
        /// </summary>
        private bool IsStarted { get; set; }
        /// <summary>
        /// 资源名
        /// </summary>
        public string AssetsName { get; set; }
        /// <summary>
        /// 创建完毕标记，用于控制UI预制件在第一次创建出来时，不要自动触发OnEnable
        /// </summary>
        public bool IsInitOver = false;
        /// <summary>
        /// 这个标记的作用是，一个隐藏的UI被重新激活，会自动触发OnEnable，会与框架中Show方法自动触发OnEnabled（如果IsShow为False）
        /// </summary>
        private bool _hasEnabled = false;

        private Canvas _rootCanvas;
        /// <summary>
        /// 关联的UI实例
        /// </summary>
        public GameObject GameObjectInstance { get; set; }

        /// <summary>
        /// 显示窗体
        /// </summary>
        /// <param name="replay">会传bool到 OnEnable/OnDisable</param>
        public void Show( bool replay = false )
        {
            IsShowing = true;

            CheckMask();

            if ( !replay )
                //gameObject.SetActive( IsShowing );
                _rootCanvas.enabled = IsShowing;
            else
                OnEnabled( replay );

            if ( !IsStarted ) DoStart();
            OnShow();
            _rootCanvas.enabled = true;
        }

        /// <summary>
        /// 检测并显示模态窗体背景
        /// </summary>
        public void CheckMask()
        {
            //设置模态窗体调用(弹出窗体)
            if ( CurrentUIType.uiNodeType == UINodeTypeEnum.PopUp )
            {
                var modelType = UIModelBehavior.Instance.GetBehavior( AssetsName );
                UIType targetUIType = modelType ?? CurrentUIType;

                UIMaskManager.Instance.SetMaskWindow( GameObjectInstance, targetUIType.uiTransparent );
            }
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        /// <param name="isDestroy">是否摧毁并彻底释放</param>
        /// <param name="freeze">是否暂时冻结，会传bool到 OnEnable/OnDisable</param>
        public void Close( bool isDestroy = false, bool freeze = false )
        {
            if ( !freeze )
            {
                _rootCanvas.enabled = false;

                if ( CurrentUIType.uiNodeType == UINodeTypeEnum.PopUp && IsShowing )
                    UIMaskManager.Instance.CancelMaskWindow();
            }
            else
            {
                _rootCanvas.enabled = false;
                //对于处于冻结的UI，可能需要断开该窗口的网络通信或者操作、刷新响应等操作
            }
            OnDisabled( freeze );

            IsShowing = false;

            OnClose();

            if ( isDestroy )
                DoDestroy();
        }

        public virtual void OnClose() { }

        ///<inheritdoc/>
        /// <remarks>
        /// 刷新窗体
        /// </remarks>
        public abstract void OnShow();

        public virtual void Dispose() { }

        public virtual void OnAdapter()
        {
            if ( _rootRectTransform == null )
                _rootRectTransform = GameObjectInstance.GetComponent<RectTransform>();
            _rootRectTransform.anchorMax = Vector2.one;
            _rootRectTransform.anchorMin = Vector2.zero;
            _rootRectTransform.offsetMax = Vector2.zero;
            _rootRectTransform.offsetMin = Vector2.zero;
            _rootCanvas = GameObjectInstance.GetComponent<Canvas>();
            _rootCanvas.enabled = false;
        }

        /// <summary>
        /// 点击返回事件
        /// </summary>
        public virtual void OnBackPushed()
        {
            LDebug.Log( "关闭ui:" + AssetsName );
            UIManager.Instance.Close( AssetsName );
        }

        #region Alternative Function

        public abstract void OnAwake();

        public virtual void OnEnabled( bool replay ) { }

        public virtual void OnDisabled( bool freeze ) { }

        public virtual void OnStart() { }

        public virtual void OnUpdate() { }

        private void DoStart()
        {
            IsStarted = true;
            OnStart();
        }

        private void DoDestroy()
        {
            Dispose();
            IsStarted = false;
            IsInitOver = false;
            GameObject.Destroy( GameObjectInstance );
            Resources.UnloadUnusedAssets();
        }
        #endregion

    }
}