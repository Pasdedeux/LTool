﻿using System.Collections;
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
        /// <summary>
        /// 当前窗口类型
        /// </summary>
        private UIType _uiType = new UIType();
        public UIType CurrentUIType
        { get { return _uiType; } set { _uiType = value; } }

        /// <summary>
        /// 关联的UI实例
        /// </summary>
        public GameObject GameObjectInstance { get; set; }

        /// <summary>
        /// 资源名
        /// </summary>
        public string AssetsName { get; set; }

        /// <summary>
        /// 显示窗体
        /// </summary>
        /// <param name="replay">【暂时不管这个参数】</param>
        public void Show( bool replay = false )
        {
            IsShowing = true;

            //设置模态窗体调用(弹出窗体)
            if ( CurrentUIType.uiNodeType == UINodeTypeEnum.PopUp )
                UIMaskManager.Instance.SetMaskWindow(GameObjectInstance, CurrentUIType.uiTransparent);

            OnEnabled();
            OnStart();
            OnShow();

            //默认执行OnEnable()
            GameObjectInstance.SetActive( IsShowing );
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        /// <param name="isDestroy">是否摧毁并彻底释放</param>
        /// <param name="freeze">是否暂时冻结（功能未想好）</param>
        public void Close( bool isDestroy = false, bool freeze = false )
        {
            //默认执行OnDisable()
            OnDisabled();

            if ( !freeze )
            {
                GameObjectInstance.SetActive(false);

                if ( CurrentUIType.uiNodeType == UINodeTypeEnum.PopUp && IsShowing )
                    UIMaskManager.Instance.CancelMaskWindow();
            }
            else
            {
                //TODO 对于处于冻结的UI，可能需要断开该窗口的网络通信或者操作、刷新响应等操作
                GameObjectInstance.SetActive(false);
            }

            IsShowing = false;

            if ( isDestroy )
                DoDestroy();
        }


        ///<inheritdoc/>
        /// <remarks>
        /// 刷新窗体
        /// </remarks>
        public abstract void OnShow();

        public virtual void Dispose() { }

        public virtual void OnAdapter()
        {
            if(_rootRectTransform==null)
                _rootRectTransform = GameObjectInstance.GetComponent<RectTransform>();
            _rootRectTransform.anchorMax = Vector2.one;
            _rootRectTransform.anchorMin = Vector2.zero;
            _rootRectTransform.offsetMax = Vector2.zero;
            _rootRectTransform.offsetMin = Vector2.zero;

        }

        /// <summary>
        /// 点击返回事件
        /// </summary>
        public virtual void OnBackPushed()
        {
            Debug.Log("关闭ui:" + AssetsName);
            UIManager.Instance.Close( AssetsName );
        }

        #region Alternative Function

        public abstract void OnAwake();

        public virtual void OnEnabled() { }

        public virtual void OnDisabled() { }

        public virtual void OnStart() { }

        public virtual void OnUpdate() { }

        private void DoDestroy()
        {
            Dispose();
            GameObject.Destroy(GameObjectInstance);
            Resources.UnloadUnusedAssets();
        }
        #endregion

    }
}