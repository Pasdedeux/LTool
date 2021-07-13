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
        //replace('\uFEFF', '')

        #region 全局设置

        /// <summary>
        /// 设置目标帧率
        /// </summary>
        [Header("目标帧率")]
        public int TargetFrameRate = 30;
        /// <summary>
        /// 开启垂直同步。默认为0-不开启，1-Every VBlank（每个VBlank计算一帧）,2-Every Second VBlank（每两个VBlank计算一帧）
        /// </summary>
        [Header("垂直同步数。默认为0不开启")]
        public int vSyncCount = 0; 

        /// <summary>
        /// 延迟调用函数计时检测间隔
        /// </summary>
        [Header( "延迟调用函数计时检测间隔" )]
        public float DelayFuncDetectInterver = 0.1f;
        /// <summary>
        /// 开启使用逐帧遍历延迟函数调用。默认为false
        /// </summary>
        [Header( "开启使用逐帧遍历延迟函数调用。默认为false" )]
        public bool UseDelayFuncPreciseDetect = false;

        /// <summary>
        /// 当前项目是否是URP
        /// </summary>
        [Header( "URP项目" )]
        public LitRenderingType renderingType = LitRenderingType.Internal;

        #endregion

        #region UI设置

        /// <summary>
        /// 触碰/点击忽略UI。默认为true
        /// </summary>
        [Header( "触碰/点击忽略UI。true" )]
        public bool TouchDetectUI = true;

        #endregion

        #region 编辑器设置

        /// <summary>
        /// 开启UGUI组件优化（仅对象创建时）
        /// </summary>
        [Header("开启UGUI组件优化（对象创建）")]
        public bool UGUIOpt = true;

        [Header("静止按压判定时间")]
        public float TouchStationaryTime = 0.5f;

        #endregion

        #region 配置档设置

        [Header( "额外登记的文件后缀" )]
        public string configs_suffix = "json|dat|assetbundle";
        [ Header("使用可读写目录")]
        public bool UsePersistantPath = false;
        [Header( "开启远程更新" )]
        public bool UseRemotePersistantPath = false;
        [Header("远程配置参数文件")]
        public string RemoteUrlConfig = "";

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


    public enum LitRenderingType
    {
        Internal,
        SRP,
        URP,
        HDRP
    }
}
