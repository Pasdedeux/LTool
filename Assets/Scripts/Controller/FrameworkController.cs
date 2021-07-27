/*======================================
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

using LitFramework;
using LitFramework.GameFlow;
using LitFramework.GameFlow.Model.DataLoadInterface;
using LitFramework.Mono;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class FrameworkController: Singleton<FrameworkController>
    {
        /// <summary>
        /// 框架启动器
        /// 
        /// 启动时先弹出LoadingUI
        /// </summary>
        public void InitFramework()
        {
            //框架启动：直接启动，或者使用LoadingTaskModel登记启动时机
            LitFrameworkFacade.Instance.StartUp( beforeExecuteFunc: () =>
            {
                //配置档加载流程预绑定
                LocalDataManager.Instance.InstallEventHandler += e =>
                {
                    //顺次加载本地配置表、JSON数据
                    new CSVConfigData( e );
                    new JsonConfigData( e );
                };
            },
            afterExecuteFunc: () =>
            {
                //框架基础启动完毕后，需要进行的自定义加载事件
                //配置表的实际加载放到这里单独执行而没有包含到框架内自动执行，是因为配置表本身可能数量多、数据量大，会有较长时间消耗
                //同时不排除业务场景中需要把这个等待过程单独表现在进度条上。
                //而UI等模块的启动依赖于框架启动，所以为了保持框架本身的快速启动，以保证UI界面能尽早完成显示（如Loading界面），故把数据加载这种可能会占用大量时间的操作，放到外面择机调用
                LoadingTaskModel.Instance.AddTask( 5, () => { LocalDataManager.Instance.Install(); return true; } );
                
                //启动Loading界面，准备进度条预读取事件
                InitLoadingLogo();
            }
            );
        }

        /// <summary>
        /// UI Loading
        /// </summary>
        public void InitLoadingLogo()
        {
            //默认启动游戏先开始显示UI界面
            UIManager.Instance.Show( ResPath.UI.UILOADING );

            UIManager.Instance.FadeImage.CrossFadeAlpha( 0, 0.4f, false );
            ColorUtility.TryParseHtmlString( "#0B477B", out Color color );
            UIMaskManager.Instance.SetMaskColor( color );
        }
    }
}
