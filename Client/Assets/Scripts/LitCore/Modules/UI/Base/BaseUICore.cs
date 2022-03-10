/*======================================
* 项目名称 ：Assets.Scripts.LitCore.Modules.UI.Base
* 项目描述 ：
* 类 名 称 ：BaseUICore
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.LitCore.Modules.UI.Base
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2022/1/12 14:49:34
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2022. All rights reserved.
*******************************************************************
======================================*/

using DG.Tweening;
using LitFramework.HotFix;
using LitFramework.UI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LitFramework.UI.Base
{
    /// <summary>
    /// 以装饰者模式封装UI窗口逻辑业务
    /// <para>无论是否使用monobehavior，该层均定义了UI各项行为的调用关系及顺序</para>
    /// <para>同类型接口交由UIManager使用，关联上层装饰者</para>
    /// <para></para>
    /// 
    /// 备注：
    /// 1、将顶层UI需要减少访问的非必掉接口隐藏，避免强行实现太多无关方法
    /// 2、提供存储状态的位置
    /// 3、维持一定的自管理逻辑，减轻UIManager需要维护的逻辑量，使其专注于流程
    /// </summary>
    public class BaseUICore : IBaseUI
    {
        private IBaseUI _operateUI;
        public BaseUICore(IBaseUI targetUI)
        {
            _operateUI = targetUI;
        }

        public bool IsShowing { get => _operateUI.IsShowing; set => _operateUI.IsShowing = value; }
        public UIType CurrentUIType { get => _operateUI.CurrentUIType; set => _operateUI.CurrentUIType = value; }
        public bool IsStarted { get => _operateUI.IsStarted; set => _operateUI.IsStarted = value; }
        public UIFlag Flag { get => _operateUI.Flag; set => _operateUI.Flag = value; }
        public bool UseLowFrame { get => _operateUI.UseLowFrame; set => _operateUI.UseLowFrame = value; }
        public string AssetsName { get => _operateUI.AssetsName; set => _operateUI.AssetsName = value; }
        public bool IsInitOver { get => _operateUI.IsInitOver; set => _operateUI.IsInitOver = value; }
        public GameObject GameObjectInstance { get => _operateUI.GameObjectInstance; set => _operateUI.GameObjectInstance = value; }
        public DOTweenAnimation[] DotAnims { get => _operateUI.DotAnims; set => _operateUI.DotAnims = value; }
        public Canvas RootCanvas { get => _operateUI.RootCanvas; set => _operateUI.RootCanvas = value; }
        public Transform RootTrans { get => _operateUI.RootTrans; set => _operateUI.RootTrans = value; }
        public Transform RootAniTrans { get => _operateUI.RootAniTrans; set => _operateUI.RootAniTrans = value; }

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
            if (IsShowing)
                OnShow(args: args);
            else
            {
                if (IsInitOver)
                    OnEnabled(false);

                IsShowing = true;

                CheckMask();

                if (!replay)
                    RootCanvas.enabled = IsShowing;
                else
                    OnEnabled(replay);

                if (!IsStarted) DoStart();

                OnShow(args);

                RootCanvas.enabled = true;
            }
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        /// <param name="isDestroy">是否摧毁并彻底释放</param>
        /// <param name="freeze">是否暂时冻结，会传bool到 OnEnable/OnDisable</param>
        public void Close(bool isDestroy = false, bool freeze = false)
        {
            if (!freeze)
            {
                RootCanvas.enabled = false;

                if (CurrentUIType.uiNodeType == UINodeTypeEnum.PopUp && IsShowing)
                    UIMaskManager.Instance.CancelMaskWindow();
            }
            else
            {
                RootCanvas.enabled = false;
                //对于处于冻结的UI，可能需要断开该窗口的网络通信或者操作、刷新响应等操作
            }

            DoReset();
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

        public void Initialize()
        {
            var rootRectTransform = GameObjectInstance.GetComponent<RectTransform>();
            rootRectTransform.anchorMax = Vector2.one;
            rootRectTransform.anchorMin = Vector2.zero;
            rootRectTransform.offsetMax = Vector2.zero;
            rootRectTransform.offsetMin = Vector2.zero;

            RootTrans = this.GameObjectInstance.transform;
            RootCanvas = GameObjectInstance.GetComponent<Canvas>();
            RootCanvas.enabled = false;
            RootAniTrans = RootTrans.Find("Container_Anim");
            DotAnims = AnimationManager.GetAllAnim(RootTrans);

            _initPos = RootAniTrans.localPosition;
            _initQuat = RootAniTrans.localRotation;
            _initScale = RootAniTrans.localScale;

            FindMember();
        }




        public virtual void OnClose() { _operateUI.OnClose(); }

        ///<inheritdoc/>
        /// <remarks>
        /// 刷新窗体
        /// </remarks>
        //public virtual void OnShow() { }

        /// <summary>
        /// 刷新窗体，带参数
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnShow(params object[] args) { _operateUI.OnShow(args); }

        public virtual void Dispose() { _operateUI.Dispose(); }

        public virtual void OnAdapter() { _operateUI.OnAdapter(); }

        /// <summary>
        /// 点击返回事件
        /// </summary>
        public virtual void OnBackPushed()
        {
            LDebug.Log("关闭ui:" + AssetsName);
            UIManager.Instance.Close(AssetsName);
        }

        #region Alternative Function
        public virtual void FindMember() { _operateUI.FindMember(); }

        public virtual void OnAwake() { _operateUI.OnAwake(); }

        public virtual void OnEnabled(bool replay) { _operateUI.OnEnabled(replay); }

        public virtual void OnDisabled(bool freeze) { _operateUI.OnDisabled(freeze); }

        public virtual void OnStart() { _operateUI.OnStart(); }

        public virtual void OnUpdate() { _operateUI.OnUpdate(); }

        //不要实现
        public void CallCtor() { }

        #region local func

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

        private void DoReset()
        {
            RootAniTrans.localPosition = _initPos;
            RootAniTrans.localRotation = _initQuat;
            RootAniTrans.localScale = _initScale;
        }

        #endregion

        #endregion

    }
}
