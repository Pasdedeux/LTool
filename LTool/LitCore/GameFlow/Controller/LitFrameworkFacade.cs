#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.GameFlow.Controller
* 项目描述 ：
* 类 名 称 ：LitFrameworkFacade
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFramework.GameFlow.Controller
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2021/3/24 22:47:35
* 更新时间 ：2021/3/24 22:47:35
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Liu Hanwen 2021. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using LitFramework;
using LitFramework.Base;
using LitFramework.GameFlow.Model.DataLoadInterface;
using LitFramework.InputSystem;
using LitFramework.LitPool;
using LitFramework.LitTool;
using LitFramework.TimeRecord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 总启动器。
/// 包含各个模块的初始化统一初始化
/// </summary>
public class LitFrameworkFacade : SingletonMono<LitFrameworkFacade>
{
    /// <summary>
    /// 框架启动
    /// </summary>
    /// <param name="afterExecuteFunc">框架启动完成后，依次执行的自定义方法</param>
    /// <param name="beforeExecuteFunc">框架启动前，依次执行的自定义方法。主要是项目中顺次执行本地数据加载</param>
    /// <param name="debugEnable">框架启动时是否开启日志</param>
    public void StartUp( Action afterExecuteFunc = null, Action beforeExecuteFunc = null, bool debugEnable = true )
    {
        DontDestroyOnLoad( GameObject.Find( "Canvas_Root" ) );

        LDebug.Enable = debugEnable;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        beforeExecuteFunc?.Invoke();

        //UI模块
        if ( FrameworkConfig.Instance.UseHotFixMode )
        {
            LitFramework.HotFix.UIManager.Instance.LoadResourceFunc = ( e ) => { return Resources.Load( e ) as GameObject; };
            LitFramework.HotFix.UIManager.Instance.Install();
        }
        else
        {
            LitFramework.Mono.UIManager.Instance.LoadResourceFunc = ( e ) => { return Resources.Load( e ) as GameObject; };
            LitFramework.Mono.UIManager.Instance.Install();
        }
       

        //Audio System
        AudioManager.Instance.LoadResourceFunc = ( e ) => { return Resources.Load( e ) as AudioClip; };
        AudioManager.Instance.Install();
        
        //操作控制器，默认Enbale=true
        InputControlManager.Instance.Install();

        //零点计时器初始化
        ZeroTimeRecord.Instance.Install();

        //对象池开启
        SpawnManager.Instance.Install();
        
        //新手引导模块
        GuideShaderController.Instance.Install();

        //最后启动自定义模块
        afterExecuteFunc?.Invoke();
    }
    
}
