/*======================================
* 项目名称 ：LitFramework
* 项目描述 ：
* 类 名 称 ：SceneLoadManager
* 
* 类 描 述 ：用于场景加载的流程控制，其中主要是SceneManager的方法再封装
*                   
* 命名空间 ：LitFramework.GameFlow.Manager
* 机器名称 ：SKY-20170413SEJ 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/8/24 20:50:24
* 更新时间 ：2018/8/24 20:50:24
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ DerekLiu 2018. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2018/8/24 20:50:24
*修改人： LHW
*版本号： V1.0.0.0
*描述：初版只是原生加载方法的简单封装，后面根据需求逐步扩展
*
======================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LitFramework.GameFlow.Manager
{
    public class SceneLoadManager : Singleton<SceneLoadManager>
    {
        private AsyncOperation _asyncLoadTask = null;

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneID"></param>
        /// <param name="isAsync"></param>
        /// <returns></returns>
        public AsyncOperation LoadSceneAsync( int sceneID, bool isAdditive )
        {
            _asyncLoadTask = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync( sceneID, !isAdditive ? UnityEngine.SceneManagement.LoadSceneMode.Single : UnityEngine.SceneManagement.LoadSceneMode.Additive );
            return _asyncLoadTask;
        }

        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="sceneID"></param>
        public void LoadScene( int sceneID )
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene( sceneID );
        }


        public override void DoDestroy()
        {
            base.DoDestroy();
            _asyncLoadTask = null;
        }

    }
}
