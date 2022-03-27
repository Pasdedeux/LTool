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
using LitFramework.Singleton;
using LitFramework.TimeRecord;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Diagnostics.DebuggerStepThrough]
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
    public void StartUp(Action beforeExecuteFunc = null, Action afterExecuteFunc = null )
    {
        beforeExecuteFunc?.Invoke();

        //系统自启动模块
        VibrateManager.Instance.Install();
        //Audio System
        AudioManager.Instance.LoadResourceFunc = (e) => RsLoadManager.Instance.Load<AudioClip>(e);
        AudioManager.Instance.Install();
        //RsLoader补充性调用，用以满足在起始RsLoader初始化时，AB相关数据尚未执行热更新的情况（这种情况下即使调用RsLoadAB接口也是以RsLoadResource方法实现的调用）
        RsLoadManager.Instance.AfterInit();
        //数据库选择性连接
        if (FrameworkConfig.Instance.useSql) SQLite.SQLManager.Install();
        //如果需要执行Loading，则将 LocalDataManager.Instance.Install() 直接放入LoadingTask即可
        //【配置档】加载流程预绑定，如果有其它自定文件类处理扩展
        LocalDataManager.Instance.InstallEventHandler += e =>
        {
            //顺次加载本地配置表、JSON数据
            new CSVConfigData(e);
            new JsonConfigData(e);
        };
        LocalDataManager.Instance.Install();
        //操作控制器，默认Enbale=true
        InputControlManager.Instance.Install();
        //零点计时器初始化
        ZeroTimeRecord.Instance.Install();
        //对象池开启
        SpawnManager.Instance.Install();
        //新手引导模块
        GuideShaderController.Instance.Install();
#if IAP
        //内购模块
        PurchaserDataModel.Instance.Install();
#endif
        //最后启动自定义模块
        afterExecuteFunc?.Invoke();
    }

    //防裁剪
    private static readonly Int32Converter i32c = new Int32Converter();
    private static readonly Int64Converter i64c = new Int64Converter();
    private static readonly StringConverter sc = new StringConverter();
    private static readonly NullableConverter nc = new NullableConverter(typeof(bool?));
    private static readonly BooleanConverter bc = new BooleanConverter();
}


