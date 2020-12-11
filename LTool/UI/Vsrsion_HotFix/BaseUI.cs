using System;
using LitFramework.UI.Base;
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
        /// <summary>
        /// 是否执行过Start
        /// </summary>
        private bool IsStarted { get; set; }
        /// <summary>
        /// 当前窗口类型
        /// </summary>
        private UIType _uiType = new UIType();
        public UIType CurrentUIType
        { get { return _uiType; } set { _uiType = value; } }
        /// <summary>
        /// 创建完毕标记
        /// </summary>
        internal bool IsInitOver = false;

        private Canvas _rootCanvas;
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
        /// <param name="replay">会传bool到 OnEnable/OnDisable</param>
        public void Show( bool replay = false )
        {
            IsShowing = true;

            CheckMask();
            
            OnEnabled( replay );
            if ( !IsStarted ) DoStart();
            OnShow();

            _rootCanvas.enabled = true;

            if ( !replay )
                GameObjectInstance.SetActive( IsShowing );
        }

        public void CheckMask()
        {
            //设置模态窗体调用(弹出窗体)
            if ( CurrentUIType.uiNodeType == UINodeTypeEnum.PopUp )
            {
                var modelType = UIModelBehavior.Instance.GetBehavior( AssetsName );
                UIType targetUIType = modelType != null ? modelType : CurrentUIType;

                UIMaskManager.Instance.SetMaskWindow( gameObject, targetUIType.uiTransparent );
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
                OnDisabled( false );
                GameObjectInstance.SetActive(false);

                if ( CurrentUIType.uiNodeType == UINodeTypeEnum.PopUp && IsShowing )
                    UIMaskManager.Instance.CancelMaskWindow();
            }
            else
            {
                _rootCanvas.enabled = false;
                //对于处于冻结的UI，可能需要断开该窗口的网络通信或者操作、刷新响应等操作
                OnDisabled( true );
                //GameObjectInstance.SetActive(false);
            }

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
            if(_rootRectTransform==null)
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
            Debug.Log("关闭ui:" + AssetsName);
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
            GameObject.Destroy(GameObjectInstance);
            Resources.UnloadUnusedAssets();
        }
        #endregion

    }
}