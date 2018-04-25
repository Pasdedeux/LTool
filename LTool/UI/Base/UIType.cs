using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIType 
{
    /// <summary>
    /// 是否需要清空弹出窗口栈
    /// </summary>
    public bool isClearPopUp = false;
    /// <summary>
    /// UI窗体挂载的节点类型
    /// </summary>
    public UINodeTypeEnum uiNodeType = UINodeTypeEnum.Normal;
    /// <summary>
    /// UI窗体显示方式
    /// Normal - 示窗体与其他窗体可以同时显示
    /// PopUp - 多层弹窗，同时需要维护多个弹出窗口并依次恢复显示的情况
    /// Unique - 独占窗口，显示时其他界面隐藏
    /// </summary>
    public UIShowModeEnum uiShowMode = UIShowModeEnum.Normal;
    /// <summary>
    /// UI窗体透明度类型(PopUp 类型时生效)
    /// </summary>
    public UITransparentEnum uiTransparent = UITransparentEnum.Penetrating;
}
