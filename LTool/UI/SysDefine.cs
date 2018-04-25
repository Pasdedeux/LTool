using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 引导挂载到不同节点
/// </summary>
public enum UINodeTypeEnum
{
    /// <summary>
    /// 普通窗体
    /// <para>一般性可拖动</para>
    /// </summary>
    Normal,
    /// <summary>
    /// 固定窗体
    /// <para>非全屏非弹出窗体</para>
    /// </summary>
    Fixed,
    /// <summary>
    /// 弹出窗体
    /// </summary>
    PopUp,
}

/// <summary>
/// 窗体显示方式
/// </summary>
public enum UIShowModeEnum
{
    /// <summary>
    /// 示窗体与其他窗体可以同时显示
    /// </summary>
    Normal,
    /// <summary>
    /// 多层弹窗，同时需要维护多个弹出窗口并依次恢复显示的情况
    /// </summary>
    PopUp,
    /// <summary>
    /// 独占窗口，显示时其他界面隐藏
    /// </summary>
    //独占窗口，显示时其他界面隐藏
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


public class SysDefine
{
    #region UI

    public static string SYS_TAG_ROOTCANVAS = "Canvas_Root";
    public static string SYS_TAG_NORMALCANVAS = "Canvas_RootNormal";
    public static string SYS_TAG_FIXEDCANVAS = "Canvas_RootFixed";
    public static string SYS_TAG_POPUPCANVAS = "Canvas_RootPopUp";
    public static string SYS_TAG_MANAGERCANVAS = "Canvas_RootScript";

    public static string SYS_TAG_UICAMERA = "UICamera";

    #endregion
}
