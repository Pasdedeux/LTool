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
using LitFramework.Input;
using LitFramework.LitPool;
using LitFramework.LitTool;
using LitFramework.Mono;
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
    /// <param name="startUpExecuteFunc">框架启动完成后，依次执行的自定义方法</param>
    /// <param name="debugEnable">框架启动时是否开启日志</param>
    public void StartUp( Action startUpExecuteFunc, bool debugEnable = true )
    {
        DontDestroyOnLoad( GameObject.Find( "Canvas_Root" ) );
        LDebug.Enable = debugEnable;

        //TODO AB模块
        //AssetDriver.Instance.Install();

        //资源加载模块
        ResourceManager.Instance.Install();

        //UI模块
        UIManager.Instance.LoadResourceFunc = ( e ) => { return Resources.Load( e ) as GameObject; };
        UIManager.Instance.Install();
        
        //UIManager.Instance.UseFading = true;
        //UIManager.Instance.FadeImage.CrossFadeAlpha( 0, 0.4f, false );
        //ColorUtility.TryParseHtmlString( "#0B477B", out Color color );
        //UIMaskManager.Instance.SetMaskColor( color );

        //Audio System
        AudioManager.Instance.LoadResourceFunc = ( e ) => { return Resources.Load( e ) as AudioClip; };
        AudioManager.Instance.Install();

        //震动
        VibrateManager.Instance.Install();

        //操作控制器，默认Enbale=true
        InputControlManager.Instance.Install();

        //零点计时器初始化
        ZeroTimeRecord.Instance.Install();

        //对象池开启
        SpawnManager.Instance.Install();

        //新手引导模块
        GuideShaderController.Instance.Install();
    }
    
}
