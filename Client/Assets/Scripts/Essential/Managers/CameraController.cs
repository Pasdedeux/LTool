/*======================================
* 项目名称 ：Assets.Scripts.Controller
* 项目描述 ：
* 类 名 称 ：CameraController
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Controller
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/5/8 10:53:20
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using LitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Assets.Scripts.Controller;
using LitFramework.InputSystem;
using LitFramework.HotFix;

/// <summary>
/// 相机控制器
/// </summary>
public class CameraController : SingletonMono<CameraController>
{
    public Camera sCamera { get; private set; }
    private void Awake()
    {
        DontDestroyOnLoad(this);
        sCamera = Camera.main;
    }
}
