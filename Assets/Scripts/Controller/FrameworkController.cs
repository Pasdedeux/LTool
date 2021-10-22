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
                LocalDataManager.Instance.InstallEventHandler += e =>
                {
                    //顺次加载本地配置表、存储的JSON数据
                    new CSVConfigData( e );
                    new JsonConfigData( e );
                };
            },
            afterExecuteFunc: () =>
            {
                //TODO 在这里写项目代码或封装启动方法
                //..
            }
            );
        }

        /// <summary>
        /// UI Loading
        /// </summary>
        public void InitLoadingLogo()
        {
            //UI模块
            UIManager.Instance.LoadResourceFunc = ( e ) => { return Resources.Load( e ) as GameObject; };
            UIManager.Instance.Install();
            //默认启动游戏先开始显示UI界面
            UIManager.Instance.Show( ResPath.UI.UILOADING );

            UIManager.Instance.FadeImage.CrossFadeAlpha( 0, 0.4f, false );
            ColorUtility.TryParseHtmlString( "#0B477B", out Color color );
            UIMaskManager.Instance.SetMaskColor( color );
        }
    }
}
