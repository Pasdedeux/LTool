﻿/*======================================
* 项目名称 ：Assets.Scripts.Controller
* 项目描述 ：
* 类 名 称 ：FrameworkFacade
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Controller
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/7/21 18:19:07
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using Assets.Scripts.Essential.ConfigParsing;
using Assets.Scripts.Essential.SDK;
using Assets.Scripts.Module.HotFix;
using LitFramework;
using LitFramework.GameFlow;
using LitFramework.HotFix;
using LitFramework.MsgSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class FrameworkController : Singleton<FrameworkController>
    {
        //定义需要迁移或热更文件
        private Queue<IHotFix> _hotFixFileQueue = new Queue<IHotFix>
        (
            new IHotFix[] { 
            //1、CsvList配置档
            new HotFixCSVList(), 
            //2、ABVersion配置档
            new HotFixAB(),
            //3、DLL热更代码档更新
            new HotfixLogic()
        });

        private UnityEngine.AsyncOperation _asyncOperation;
        private IScriptCall sc = null;
        /// <summary>
        /// 框架启动器
        /// 
        /// 这一部分请勿随意删减
        /// 
        /// 启动时先弹出LoadingUI
        /// </summary>
        public void InitFramework()
        {
            GameObject.DontDestroyOnLoad(GameObject.Find("Canvas_Root"));

            LDebug.Enable = FrameworkConfig.Instance.showLog;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            AuthorizedManager.Instance.Install();

            //文件迁移和热更结束后，才进行后续加载步骤
            //若未开启对应功能，则直接继续后续步骤
            MsgManager.Instance.Register(InternalEvent.END_LOAD_REMOTE_CONFIG, LoadAllConfigs);
            //由于UI的使用，需要提前执行方法确定
            RsLoadManager.Instance.Install();
            //UI模块中Loading界面先于热更逻辑执行，故需要提前完成初始化
            if (FrameworkConfig.Instance.UseHotFixMode)
            {
                //保留命名空间
                LitFramework.HotFix.UIManager.Instance.LoadResourceFunc = (e) => RsLoadManager.Instance.Load<GameObject>(e);
                LitFramework.HotFix.UIManager.Instance.Install();
            }
            else
            {
                //保留命名空间
                LitFramework.Mono.UIManager.Instance.LoadResourceFunc = (e) => RsLoadManager.Instance.Load<GameObject>(e);
                LitFramework.Mono.UIManager.Instance.Install();
            }

            //框架基础启动完毕后，需要进行的自定义加载事件
            //配置表的实际加载放到这里单独执行而没有包含到框架内自动执行，是因为配置表本身可能数量多、数据量大，会有较长时间消耗
            //同时不排除业务场景中需要把这个等待过程单独表现在进度条上。
            //而UI等模块的启动依赖于框架启动，所以为了保持框架本身的快速启动，以保证UI界面能尽早完成显示（如Loading界面），故把数据加载这种可能会占用大量时间的操作，放到外面择机调用
            //切换运行环境
            if (FrameworkConfig.Instance.scriptEnvironment == RunEnvironment.DotNet) sc = new DotNetScriptCall();
            else if (FrameworkConfig.Instance.scriptEnvironment == RunEnvironment.ILRuntime) sc = new ILRScriptCall();

            //执行热更流程
            LoadingTaskModel.Instance.AddTask(5, () =>
            {
                //暂停接入任务
                UI.UILoading.LOADING_CONTINUE = false;
                HotFixController.Instance.Excecute(_hotFixFileQueue);
                return true;
            });

            //==================具体项目的代码从这里开始==================//
            LoadingTaskModel.Instance.AddTask(20, () =>
            {
                _asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);
                _asyncOperation.allowSceneActivation = true;
                return true;
            });
            //==================具体项目的代码从这里结束==================//


            LoadingTaskModel.Instance.AddTask(100, () =>
            {
                while (!_asyncOperation.isDone) { };
                sc.StartRun();
                UIManager.Instance.Close(ResPath.UI.UILOADING);
                return true;
            });

            //启动Loading界面，准备进度条预读取事件
            StartLoadingLogo();
        }

        /// <summary>
        /// UI Loading
        /// </summary>
        public void StartLoadingLogo()
        {
            //默认启动游戏先开始显示UI界面，并增加一次渐显效果
            UIManager.Instance.Show(ResPath.UI.UILOADING);
            UIManager.Instance.FadeImage.CrossFadeAlpha(0, 0.4f, false);
        }


        /// <summary>
        /// 执行文件迁移+文件热更结束
        /// </summary>
        /// <param name="e"></param>
        private void LoadAllConfigs(MsgArgs e = null)
        {
            MsgManager.Instance.UnRegister(InternalEvent.END_LOAD_REMOTE_CONFIG, LoadAllConfigs);
            //在热更模式下，用于初始化
            sc.PreStartRun();
            //除UI外，其它模块需要确保在资源完整更新后，执行项目启动
            LitFrameworkFacade.Instance.StartUp();
            //执行ScriptableObject的数据注入
            ScriptableLoader.Instance.InjectScriptableData("CommonData");
            //继续加载任务
            UI.UILoading.LOADING_CONTINUE = true;
        }




    }
}
