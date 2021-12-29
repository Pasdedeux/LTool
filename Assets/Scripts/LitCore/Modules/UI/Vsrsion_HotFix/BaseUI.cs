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

using DG.Tweening;
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
        protected bool IsStarted { get; set; }
        /// <summary>
        /// 附加特性
        /// </summary>
        public UIFlag Flag = UIFlag.Normal;
        /// <summary>
        /// 是否使用低帧率
        /// </summary>
        public bool UseLowFrame = false;
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
        /// 动画列表
        /// </summary>
        public DOTweenAnimation[] ui_anims;
        protected Transform root,m_AniTrans;

        //基础信息的初始化状态
        private Vector3 _initPos = Vector3.zero, _initScale = Vector3.zero;
        private Quaternion _initQuat = Quaternion.identity;

        /// <summary>
        /// 显示窗体
        /// </summary>
        /// <param name="replay">会传bool到 OnEnable/OnDisable</param>
        /// <param name="args">通过Show的传参</param>
        public void Show(bool replay = false, params object[] args)
        {
            IsShowing = true;

            CheckMask();

            if (!replay)
                //gameObject.SetActive( IsShowing );
                _rootCanvas.enabled = IsShowing;
            else
                OnEnabled(replay);

            if (!IsStarted) DoStart();

            OnShow(args);

            _rootCanvas.enabled = true;
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        /// <param name="isDestroy">是否摧毁并彻底释放</param>
        /// <param name="freeze">是否暂时冻结，会传bool到 OnEnable/OnDisable</param>
        public void Close(bool isDestroy = false, bool freeze = false)
        {
            OnDisabled(freeze);

            if (!freeze)
            {
                _rootCanvas.enabled = false;

                if (CurrentUIType.uiNodeType == UINodeTypeEnum.PopUp && IsShowing)
                    UIMaskManager.Instance.CancelMaskWindow();
            }
            else
            {
                _rootCanvas.enabled = false;
                //对于处于冻结的UI，可能需要断开该窗口的网络通信或者操作、刷新响应等操作
            }

            OnReset();
            OnClose();

            if (isDestroy) DoDestroy();

            //确保最后执行，保证cancelmaskwindow正常执行
            IsShowing = false;
        }


        /// <summary>
        /// 检测并显示模态窗体背景
        /// </summary>
        public void CheckMask()
        {
            //设置模态窗体调用(弹出窗体)
            if (CurrentUIType.uiNodeType == UINodeTypeEnum.PopUp)
            {
                var modelType = UIModelBehavior.Instance.GetBehavior(AssetsName);
                UIType targetUIType = modelType ?? CurrentUIType;

                UIMaskManager.Instance.SetMaskWindow(GameObjectInstance, targetUIType.uiTransparent);
            }
        }

        public virtual void OnClose() { }

        ///<inheritdoc/>
        /// <remarks>
        /// 刷新窗体
        /// </remarks>
        //public virtual void OnShow() { }

        /// <summary>
        /// 刷新窗体，带参数
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnShow( params object[] args ) { }

        public virtual void Dispose() { }

        public virtual void OnAdapter()
        {
            if (_rootRectTransform == null)
                _rootRectTransform = GameObjectInstance.GetComponent<RectTransform>();
            _rootRectTransform.anchorMax = Vector2.one;
            _rootRectTransform.anchorMin = Vector2.zero;
            _rootRectTransform.offsetMax = Vector2.zero;
            _rootRectTransform.offsetMin = Vector2.zero;
            _rootCanvas = GameObjectInstance.GetComponent<Canvas>();
            _rootCanvas.enabled = false;
            root = _rootCanvas.transform;
        }

        /// <summary>
        /// 点击返回事件
        /// </summary>
        public virtual void OnBackPushed()
        {
            LDebug.Log("关闭ui:" + AssetsName);
            UIManager.Instance.Close(AssetsName);
        }

        #region Alternative Function
        protected virtual void FindMember() { }
        public abstract void OnAwake();

        public virtual void OnEnabled(bool replay) { }

        public virtual void OnDisabled(bool freeze) { }

        public virtual void OnStart() { }

        public virtual void OnUpdate() { }

        internal void Initialize()
        {
            root = this.GameObjectInstance.transform;
            m_AniTrans = root.Find("Container_Anim");
            ui_anims = AnimationManager.GetAllAnim(root);

            _initPos = m_AniTrans.localPosition;
            _initQuat = m_AniTrans.localRotation;
            _initScale = m_AniTrans.localScale;
            FindMember();
        }

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
            GameObject.Destroy(GameObjectInstance);
            Resources.UnloadUnusedAssets();
        }

        private void OnReset()
        {
            m_AniTrans.localPosition = _initPos;
            m_AniTrans.localRotation = _initQuat;
            m_AniTrans.localScale = _initScale;
        }
        #endregion

    }
}