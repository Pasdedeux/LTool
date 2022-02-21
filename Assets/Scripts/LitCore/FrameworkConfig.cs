#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework
* 项目描述 ：
* 类 名 称 ：AppConfig
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFramework
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/5/26 16:29:30
* 更新时间 ：2018/5/26 16:29:30
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2018. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LitFramework
{
    /// <summary>
    /// 框架控制参数
    /// </summary>
    public class FrameworkConfig:SingletonMono<FrameworkConfig>
    {
        #region 基础设置
        [FoldoutGroup("基础设置")]
        /// <summary>
        /// 设置目标帧率
        /// </summary>
        [LabelText("目标帧率")]
        public int TargetFrameRate = 30;
        [FoldoutGroup("基础设置")]
        /// <summary>
        /// 开启垂直同步。默认为0-不开启，1-Every VBlank（每个VBlank计算一帧）,2-Every Second VBlank（每两个VBlank计算一帧）
        /// </summary>
        [LabelText("垂直同步数。默认为不开启")]
        public int vSyncCount = 0;
        [FoldoutGroup("基础设置")]
        /// <summary>
        /// 延迟调用函数计时检测间隔
        /// </summary>
        [LabelText( "延迟调用函数计时检测间隔" )]
        public float DelayFuncDetectInterver = 0.1f;
        [FoldoutGroup("基础设置")]
        /// <summary>
        /// 开启使用逐帧遍历延迟函数调用。默认为false
        /// </summary>
        [LabelText( "使用逐帧遍历延迟函数" )]
        public bool UseDelayFuncPreciseDetect = false;
        //[FoldoutGroup("基础设置")]
        /// <summary>
        /// 当前项目是否是URP
        /// </summary>
        //[LabelText( "URP项目" )]
        [HideInInspector]
        public LitRenderingType renderingType = LitRenderingType.Internal;
        [FoldoutGroup("基础设置")]
        /// <summary>
        /// 当前项目资源加载方式
        /// </summary>
        [LabelText( "资源加载方式" )]
        [OnValueChanged("CheckRsLoadType")]
        public ResLoadType resLoadType = ResLoadType.Resource;
        private void CheckRsLoadType()
        {
            loadType = resLoadType;
        }
        #endregion

        #region UI设置
        [FoldoutGroup("UI设置")]
        [LabelText("以热更模式制作")]
        public bool UseHotFixMode = true;
        [FoldoutGroup("UI设置")]
        /// <summary>
        /// 触碰/点击忽略UI。默认为true
        /// </summary>
        [LabelText( "触碰/点击UI检测" )]
        public bool TouchDetectUI = true;
        [FoldoutGroup("UI设置")]
        [LabelText("UI DOT动画默认进场ID")]
        public string OPENID = "open";
        [FoldoutGroup("UI设置")]
        [LabelText("UI DOT动画默认出场ID")]
        public string CLOSEID = "close";
        [FoldoutGroup("UI设置")]
        [LabelText("UI低帧率")]
        public int UI_LOW_FRAMERATE = 10;

        #endregion

        #region 编辑器设置
        [FoldoutGroup("编辑器设置")]
        /// <summary>
        /// 开启UGUI组件优化（仅对象创建时）
        /// </summary>
        [LabelText("开启UGUI组件优化（对象创建）")]
        public bool UGUIOpt = true;
        [FoldoutGroup("编辑器设置")]
        [LabelText("静止按压判定时间")]
        public float TouchStationaryTime = 0.5f;

        #endregion

        #region 配置档设置
        
        [FoldoutGroup("配置档设置")]
        [LabelText( "AB包文件夹名称" )]
        public string ABFolderName = "ABPackages";
        //[LabelText( "AB总包名称" )]
        [HideInInspector]
        public string ABTotalName = "ABPackages";
        
        [FoldoutGroup("配置档设置")]
        [LabelText( "额外登记的文件后缀" )]
        public string configs_suffix = "json|dat|assetbundle";
       
        [FoldoutGroup("配置档设置")]
        [LabelText("是否打成压缩包")]
        public bool useZIP = false;

        [FoldoutGroup("配置档设置")]
        [OnValueChanged("CheckPersistPath")]
        [LabelText("是否使用Sqllite")]
        public bool useSql = false;
        private void CheckPersistPath()
        {
            if (useSql && !UsePersistantPath) UsePersistantPath = true;
        }

        [FoldoutGroup("配置档设置")]
        [LabelText("是否动态添加对象池")]
        public bool useSpawnConfig = false;
       
        [FoldoutGroup("配置档设置")]
        [ShowIf("useSpawnConfig")]
        [LabelText("动态加载方式")]
        public ResLoadType loadType = ResLoadType.Resource;


        [FoldoutGroup("热更新")]
        [OnValueChanged("CheckUseSql")]
        [ LabelText("使用可读写目录")]
        public bool UsePersistantPath = false;
        private void CheckUseSql()
        {
            if (!UsePersistantPath && useSql) useSql = false;
        }

        [FoldoutGroup("热更新")]
        [ShowIf("UsePersistantPath")]
        [LabelText( "开启远程更新" )]
        public bool UseRemotePersistantPath = false;

        [FoldoutGroup("热更新")]
        [ShowIf("@UsePersistantPath && UseRemotePersistantPath")]
        [LabelText("远程配置参数文件(*/)")]
        public string RemoteUrlConfig = "http://192.168.1.102/";

        #endregion

        #region 调试设置

        [BoxGroup("调试设置",centerLabel:true)]
        [LabelText("是否打印日志")]
        public bool showLog = true;
        [BoxGroup("调试设置", centerLabel: true)]
        [LabelText("程序开发调试")]
        public bool isProgramTest = false;
        [BoxGroup("调试设置", centerLabel: true)]
        [ShowIf("UsePersistantPath")]
        [LabelText("强制更新读写目录")]
        public bool ForceUpdatePersistant = false;

        [Space(10)]
        [LabelText("代码运行环境")]
        public RunEnvironment scriptEnvironment = RunEnvironment.DotNet;
        
        #endregion

        #region Func
        void Awake()
        {
            DontDestroyOnLoad( transform.parent );
            //帧率设定
            Application.targetFrameRate = TargetFrameRate;
            QualitySettings.vSyncCount = vSyncCount;//默认不开启垂直同步
        }
        #endregion
    }

    /// <summary>
    /// 渲染管线
    /// </summary>
    public enum LitRenderingType
    {
        Internal,
        SRP,
        URP,
        HDRP
    }
    /// <summary>
    /// 代码执行方式
    /// </summary>
    public enum RunEnvironment
    {
        DotNet,
        ILRuntime,
    }
    /// <summary>
    /// 项目默认资源加载方式
    /// </summary>
    public enum ResLoadType
    {
        Resource,
        AssetBundle,
    }


    /// <summary>
    /// csv资源文件内容
    /// </summary>
    public class ABVersion
    {
        public string AbName;
        public int Version;
        public string MD5;
    }
}
