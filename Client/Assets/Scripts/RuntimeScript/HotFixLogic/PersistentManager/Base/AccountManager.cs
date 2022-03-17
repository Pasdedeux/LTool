using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitFramework;
using LitFramework.LitTool;
using System;
using LitFramework.Singleton;

public class AccountManager : BaseLocalConfigManager<AccountManager>
{
    public AccountLocalData LocalData;
    private System.DateTime IniteDate;
    private long lineTime;

    /// <summary>
    /// 离线时间（毫秒）
    /// </summary>
    public long LineTime
    {
        get => lineTime;
    }
   
    public override void Install()
    {
        lineTime = LocalData.localSaveTime;
        LocalDataHandle.onSaveData += RefreshSaveTime;
    }
    /// <summary>
    /// 更新保存时间
    /// </summary>
    private void RefreshSaveTime()
    {
        LocalData.localSaveTime = LitTool.GetTimeMillisStamp();
    }

}
