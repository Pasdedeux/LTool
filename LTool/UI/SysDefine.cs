/*======================================
* 项目名称 
* 项目描述 ：
* 类 名 称 ：UISysDefine
* 类 描 述 ：
*                   
* 命名空间 ：
* 机器名称 ：
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/5/17 11:50:24
* 更新时间 ：2018/5/17 11:50:24
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ DerekLiu 2018. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2018/5/17 11:50:24
*修改人： LHW
*版本号： V1.0.0.0
*描述：
*
======================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 引导挂载到不同节点
/// </summary>
public enum UINodeTypeEnum
{
    /// <summary>
    /// 节点 - 普通窗体
    /// <para>一般性可拖动</para>
    /// </summary>
    Normal,
    /// <summary>
    /// 节点 - 固定窗体
    /// <para>用于最底层的全屏界面UI   </para>
    /// </summary>
    Fixed,
    /// <summary>
    /// 节点 - 弹出窗体，可使用黑色遮罩
    /// </summary>
    PopUp,
}

/// <summary>
/// 窗体显示方式
/// </summary>
public enum UIShowModeEnum
{
    /// <summary>
    /// 展示窗体与其他窗体可以同时显示
    /// </summary>
    Parallel,
    /// <summary>
    /// 多层弹窗，同时需要维护多个弹出窗口并依次恢复显示的情况
    /// </summary>
    Stack,
    /// <summary>
    /// 独占窗口，显示时其他界面隐藏
    /// </summary>
    Unique,
}

/// <summary>
/// 定义模态窗体的透明度
/// </summary>
public enum UITransparentEnum
{
    /// <summary>
    /// 低透明，不可穿透
    /// </summary>
    NoPenetratingLow,
    /// <summary>
    /// 半透明，不可穿透
    /// </summary>
    NoPenetratingMiddle,
    /// <summary>
    /// 透明，不可穿透
    /// </summary>
    NoPenetratingTotal,
    /// <summary>
    /// 可穿透
    /// </summary>
    Penetrating
}


public class UISysDefine
{
    #region UI

    public static string SYS_TAG_ROOTCANVAS = "Canvas_Root";
    public static string SYS_TAG_NORMALCANVAS = "Canvas_RootNormal";
    public static string SYS_TAG_FIXEDCANVAS = "Canvas_RootFixed";
    public static string SYS_TAG_POPUPCANVAS = "Canvas_RootPopUp";
    public static string SYS_TAG_GLOBALCANVAS = "Canvas_RootGlobal";

    public static string SYS_TAG_UICAMERA = "UICamera";

    #endregion
}
