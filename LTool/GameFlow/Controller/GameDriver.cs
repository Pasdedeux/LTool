using LitFramework;
using LitFramework.Input;
using LitFramework.LitTool;
using LitFramework.TimeRecord;
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
    /// <summary>
    /// 零点刷新回调
    /// </summary>
    public event Action<DateTime> DelZeroTime;
    private DateTime _localTime;

    private void Update()
    {
        UpdateEventHandler?.Invoke();

        _localTime = DateTime.Now;
        if ( _localTime.Day != ZeroTimeRecord.RecordDay || _localTime.Month != ZeroTimeRecord.RecordMonth || _localTime.Year != ZeroTimeRecord.RecordYear )
            DelZeroTime?.Invoke( _localTime );
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
