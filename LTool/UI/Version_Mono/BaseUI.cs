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
 * Copyright @ Derek 2018 All rights reserved 
*****************************************************************/

using System.Collections;
using UnityEngine;

namespace LitFramework.Mono
{

    public abstract class BaseUI : MonoBehaviour , UI.Base.IBaseUI
    {
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
        /// 是否执行过Start
        /// </summary>
        private bool IsStarted { get; set; }
        private Coroutine _waitForStartFunc;
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

            //默认执行OnEnable()
            gameObject.SetActive( IsShowing );

            //设置模态窗体调用(弹出窗体)
            if( CurrentUIType.uiNodeType == UINodeTypeEnum.PopUp )
                UIMaskManager.Instance.SetMaskWindow( gameObject , CurrentUIType.uiTransparent );

            if( IsStarted )
                OnShow();
            else
                _waitForStartFunc = StartCoroutine( IWaitToOnShow() );
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        /// <param name="isDestroy">是否摧毁并彻底释放</param>
        /// <param name="freeze">是否暂时冻结（功能未想好）</param>
        public void Close( bool isDestroy = false , bool freeze = false )
        {
            //默认执行OnDisable()
            if( !freeze )
            {
                gameObject.SetActive( false );

                if( CurrentUIType.uiNodeType == UINodeTypeEnum.PopUp && IsShowing )
                    UIMaskManager.Instance.CancelMaskWindow();
            }
            else
            {
                //TODO 对于处于冻结的UI，可能需要断开该窗口的网络通信或者操作、刷新响应等操作
                gameObject.SetActive( false );
            }

            IsShowing = false;

            if( _waitForStartFunc != null )
            {
                StopCoroutine( _waitForStartFunc );
                _waitForStartFunc = null;
            }

            if( isDestroy )
                DoDestroy();
        }


        ///<inheritdoc/>
        /// <remarks>
        /// 刷新窗体
        /// </remarks>
        public abstract void OnShow();

        public virtual void Dispose() { }

        public virtual void OnAdapter() { }

        #region Alternative Function

        public abstract void OnAwake();

        public virtual void OnEnabled() { }

        public virtual void OnDisabled() { }

        public virtual void OnStart() { }

        public virtual void OnUpdate() { }

        private void DoDestroy()
        {
            Dispose();
            IsStarted = false;
            GameObject.Destroy( gameObject );
            Resources.UnloadUnusedAssets();
        }

        private void Awake()
        {
            OnAwake();
        }

        private void Start()
        {
            OnStart();
            IsStarted = true;
        }

        private void OnEnable()
        {
            OnEnabled();
        }

        private void OnDisable()
        {
            OnDisabled();
        }

        private void Update()
        {
            OnUpdate();
        }
        #endregion

        private IEnumerator IWaitToOnShow()
        {
            yield return new WaitUntil( () => { return IsStarted; } );
            OnShow();
        }

        /// <summary>
        /// 点击返回事件
        /// </summary>
        public virtual void OnBackPushed()
        {
            Debug.Log( "关闭ui:" + AssetsName );
            UIManager.Instance.Close( AssetsName );
        }
    }
}