/*======================================
* 项目名称 ：LitFramework
* 项目描述 ：
* 类 名 称 ：GameFlowController
* 类 描 述 ：
*                   
* 命名空间 ：LitFramework.GameFlow
* 机器名称 ：
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/8/24 11:11:24
* 更新时间 ：2018/8/24 11:11:24
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ DerekLiu 2018. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2018/8/24 11:11:24
*修改人： LHW
*版本号： V1.0.0.0
*描述：先增加UILoading与场景加载方法的合并
*
======================================*/

using LitFramework.GameFlow.Manager;
using LitFramework.UI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LitFramework.GameFlow
{
    /// <summary>
    /// 游戏主流程控制器，用于场景加载、跳转等命令合集
    /// </summary>
    public class GameFlowController : Singleton<GameFlowController>
    {
        private SceneLoadManager _sceneLoadMng;
        private AsyncOperation _asyncOperation = null;
        private IUIManager _iUIManger = null;

        public GameFlowController()
        {
            _sceneLoadMng = SceneLoadManager.Instance;
        }

        /// <summary>
        /// 场景切换，此方法用于Async加载方式
        /// </summary>
        /// <param name="sceneID">要切换到的目标ID</param>
        /// <param name="isAdditive">是否叠加场景，默认为false</param>
        /// <param name="needLoadingUI">是否需要转场UI</param>
        /// <param name="needFading">是否需要黑屏渐变</param>
        /// <param name="isHot">是否是热更版本</param>
        public void ChangeScene( int sceneID, string loadingUIPath = "", Action callBackAfterChanging = null, bool isAdditive = false, bool isHot = false, bool needFading = true, float fadingTime = 0.5f )
        {
            _iUIManger = isHot ? ( ( IUIManager )HotFix.UIManager.Instance ) : ( ( IUIManager )Mono.UIManager.Instance );
            
            if ( needFading )            
                _iUIManger.HideFade( fadingTime );

            _asyncOperation = SceneLoadManager.Instance.LoadSceneAsync( sceneID, isAdditive );
            _asyncOperation.allowSceneActivation = false;
            
            // 如果需要配合显示进度条，则将场景加载至于进度为0的时候执行
            if ( loadingUIPath != string.Empty )
            {
                //牺牲了场景加载的进度性
                LoadingTaskModel.Instance.AddTask( 0, () =>
                {
                    bool over = _asyncOperation.progress >= 0.9f;
                    if ( over ) _asyncOperation.allowSceneActivation = true;
                    return over;
                }, true );

                //场景加载完成后的回调
                LoadingTaskModel.Instance.AddTask( 100, () =>
                {
                    _asyncOperation.allowSceneActivation = true;
                    
                    if ( needFading )
                        _iUIManger.ShowFade( fadingTime, () =>
                        {
                            if ( isHot )
                                HotFix.UIManager.Instance.Close( loadingUIPath );
                            else
                                Mono.UIManager.Instance.Close( loadingUIPath );

                            if ( callBackAfterChanging != null ) callBackAfterChanging.Invoke();
                            _iUIManger.HideFade( fadingTime );
                        } );
                    else
                    {
                        if ( isHot )
                            HotFix.UIManager.Instance.Close( loadingUIPath );
                        else
                            Mono.UIManager.Instance.Close( loadingUIPath );

                        if ( callBackAfterChanging != null ) callBackAfterChanging.Invoke();
                    }

                    LoadingTaskModel.Instance.ClearTask();
                    return true;
                }, true );

                if ( isHot )
                    HotFix.UIManager.Instance.Show( loadingUIPath );
                else
                    Mono.UIManager.Instance.Show( loadingUIPath );
            }
            //如果不需要UI的切换场景，则直接异步加载即可
            else
            {
                while ( _asyncOperation.progress < 0.9f ) { }
                _asyncOperation.allowSceneActivation = true;

                if ( needFading )
                    _iUIManger.ShowFade( fadingTime, () =>
                    {
                        if ( callBackAfterChanging != null ) callBackAfterChanging.Invoke();
                        _iUIManger.HideFade( fadingTime );
                    } );
                else
                     if ( callBackAfterChanging != null ) callBackAfterChanging.Invoke();
            }
            
        }


        public override void DoDestroy()
        {
            base.DoDestroy();
            _sceneLoadMng = null;
        }
    }
}
