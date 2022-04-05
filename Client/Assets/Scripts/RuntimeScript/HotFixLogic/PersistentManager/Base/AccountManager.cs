using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitFramework;
using LitFramework.LitTool;
using System.Linq;
using LitFramework.LitPool;
using System.Runtime.InteropServices;

namespace LitFramework.Persistent
{
    public class AccountManager : BaseLocalDataManager<AccountManager, AccountLocalData>
    {
        private long lineTime;
        private System.DateTime IniteDate;

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
            LocalData.localSaveTime = LitTool.LitTool.GetTimeMillisStamp();
        }
    }

}
