/*======================================
* 项目名称 ：Assets.Scripts.DataModel
* 项目描述 ：
* 类 名 称 ：GuideDataModel
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.DataModel
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Chengdu Lewang Technology Co., Ltd 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using LitFramework;
using LitFramework.Input;
using LitFramework.LitTool;
using LitFramework.Mono;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 分部类。可以在业务逻辑上继续扩展
/// </summary>
public partial class GuideDataModel : Singleton<GuideDataModel>
{
    /// <summary>
    /// 圆形/矩形聚焦完毕回调
    /// </summary>
    public Action FocusTargetDoneEvent;
    /// <summary>
    /// 点击目标位置完成本轮
    /// </summary>
    public Action GuideDoneEvent;
    
    
    
    
}
