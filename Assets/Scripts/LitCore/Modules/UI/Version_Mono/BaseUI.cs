/**************************************************************** 
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
using UnityEngine.UI;

namespace LitFramework.Mono
{
    /// <summary>
    /// 与Hotfix相同
    /// 
    /// 留出位置留待项目内进行其它扩展
    /// </summary>
    public abstract class BaseUI : MonoBehaviour, IBaseUI
    {
        #region 自持有状态

        public bool IsShowing { get; set; } = false;
        public bool IsStarted { get; set; } = false;
        public bool IsInitOver { get; set; } = false;
        public bool UseLowFrame { get; set; } = false;
        public string AssetsName { get; set; }
        public UIFlag Flag { get; set; } = UIFlag.Normal;
        public UIType CurrentUIType { get; set; } = new UIType();
        public GameObject GameObjectInstance { get; set; }
        public DOTweenAnimation[] DotAnims { get; set; }
        public Transform RootTrans { get; set; }
        public Transform RootAniTrans { get; set; }
        public Canvas RootCanvas { get; set; }

        #endregion

        #region 核心业务类

        private IBaseUI _uiCore;

        public void CallCtor()
        {
            if (_uiCore == null) _uiCore = new BaseUICore(this);
        }

        public void CheckMask()
        {
            _uiCore.CheckMask();
        }

        public void Show(bool replay = false, params object[] args)
        {
            _uiCore.Show(replay, args);
        }

        public void Close(bool isDestroy = false, bool freeze = false)
        {
            _uiCore.Close(isDestroy, freeze);
        }

        public void Initialize()
        {
            _uiCore.Initialize();
        }

        #endregion

        #region 子类覆写

        public virtual void Dispose() { }

        public virtual void FindMember() { }

        public virtual void OnAdapter() { }

        public abstract void OnAwake();

        public virtual void OnBackPushed() { }

        public virtual void OnClose() { }

        public virtual void OnDisabled(bool freeze) { }

        public virtual void OnEnabled(bool replay) { }

        public virtual void OnShow(params object[] args) { }

        public virtual void OnStart() { }

        public virtual void OnUpdate() { }

        #endregion
    }
}