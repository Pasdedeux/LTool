using LitFramework;
using LitFramework.Input;
using LitFramework.LitTool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 框架驱动器。提供Update FixedUpdate LateUpdate驱动
/// </summary>
public partial class GameDriver : SingletonMono<GameDriver>
{
    public Action UpdateEventHandler;
    public Action FixedUpdateEventHandler;
    public Action LateUpdateEventHandler;
    
    private void Update()
    {
        UpdateEventHandler?.Invoke();
    }

    private void FixedUpdate()
    {
        FixedUpdateEventHandler?.Invoke();
    }

    private void LateUpdate()
    {
        LateUpdateEventHandler?.Invoke();
    }
}
