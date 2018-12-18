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
        /// 加载场景，这里使用的async加载方式
        /// </summary>
        /// <param name="sceneID"></param>
        /// <param name="callBackBeforeChanging"></param>
        /// <param name="callBackAfterChanging"></param>
        /// <param name="loadingUIPath"></param>
        /// <param name="needFading"></param>
        /// <param name="fadingTime"></param>
        /// <param name="isHot"></param>
        /// <param name="isAdditive"></param>
        public void ChangeScene( int sceneID, Action callBackBeforeChanging = null, Action callBackAfterChanging = null, string loadingUIPath = null, bool needFading = true, float fadingTime = 0.5f, bool isHot = false, bool isAdditive = false )
        {
            _iUIManger = isHot ? ( ( IUIManager )HotFix.UIManager.Instance ) : ( ( IUIManager )Mono.UIManager.Instance );
            _iUIManger.UseFading = needFading;

            //No UIloading && No Fading
            if ( string.IsNullOrEmpty( loadingUIPath ) && !needFading )
            {
                callBackBeforeChanging?.Invoke();

                _asyncOperation = SceneLoadManager.Instance.LoadSceneAsync( sceneID, isAdditive );
                _asyncOperation.allowSceneActivation = false;
                while ( _asyncOperation.progress < 0.9f ) { }
                _asyncOperation.allowSceneActivation = true;

                GameUtility.LitTool.WaitUntilFunction( () => { return _asyncOperation.isDone; }, () => { callBackAfterChanging?.Invoke(); } );
            }

            //No UIloading && Fading
            else if ( string.IsNullOrEmpty( loadingUIPath ) && needFading )
            {
                _iUIManger.ShowFade( fadingTime, () =>
                {
                    callBackBeforeChanging?.Invoke();

                    _asyncOperation = SceneLoadManager.Instance.LoadSceneAsync( sceneID, isAdditive );
                    _asyncOperation.allowSceneActivation = false;
                    while ( _asyncOperation.progress < 0.9f ) { }
                    _asyncOperation.allowSceneActivation = true;

                    GameUtility.LitTool.WaitUntilFunction( () => { return _asyncOperation.isDone; }, () => { callBackAfterChanging?.Invoke(); } );

                    _iUIManger.HideFade( fadingTime );
                } );
            }

            //UIloading && No Fading
            else if( !string.IsNullOrEmpty( loadingUIPath ) && !needFading )
            {
                // 默认占用0帧
                LoadingTaskModel.Instance.AddTask( 0, () =>
                {
                    callBackBeforeChanging?.Invoke();

                    _asyncOperation = SceneLoadManager.Instance.LoadSceneAsync( sceneID, isAdditive );
                    _asyncOperation.allowSceneActivation = false;
                    return true;
                }, true );

                // 默认占用1帧 牺牲了场景加载的进度性
                LoadingTaskModel.Instance.AddTask( 1, () =>
                {
                    bool over = _asyncOperation.progress >= 0.9f;
                    if ( over ) _asyncOperation.allowSceneActivation = true;
                    return over;
                }, true );

                //场景加载完成后的回调
                LoadingTaskModel.Instance.AddTask( 100, () =>
                {
                    LoadingTaskModel.Instance.ClearTask();
                    callBackAfterChanging?.Invoke();

                    _iUIManger.Close( loadingUIPath );
                    return true;
                }, true );

                _iUIManger.Show( loadingUIPath );
            }

            //UIloading && Fading
            else
            {
                _iUIManger.ShowFade( fadingTime, () =>
                {
                    //显示UILoading
                    _iUIManger.HideFade( fadingTime, () =>
                    {
                        _iUIManger.Show( loadingUIPath );
                    } );

                    // 默认占用0帧
                    LoadingTaskModel.Instance.AddTask( 0, () =>
                    {
                        callBackBeforeChanging?.Invoke();

                        _asyncOperation = SceneLoadManager.Instance.LoadSceneAsync( sceneID, isAdditive );
                        _asyncOperation.allowSceneActivation = false;
                        return true;
                    }, true );

                    // 默认占用1帧 牺牲了场景加载的进度性
                    LoadingTaskModel.Instance.AddTask( 1, () =>
                    {
                        bool over = _asyncOperation.progress >= 0.9f;
                        if ( over ) _asyncOperation.allowSceneActivation = true;
                        return over;
                    }, true );

                    //场景加载完成后的回调
                    LoadingTaskModel.Instance.AddTask( 100, () =>
                    {
                        _iUIManger.ShowFade( fadingTime, () =>
                        {
                            LoadingTaskModel.Instance.ClearTask();
                            callBackAfterChanging?.Invoke();

                            _iUIManger.Close( loadingUIPath );
                            _iUIManger.HideFade( fadingTime );
                        });
                        return true;
                    }, true );
                } );
            }

        }


        public override void DoDestroy()
        {
            base.DoDestroy();
            _sceneLoadMng = null;
        }
    }
}
