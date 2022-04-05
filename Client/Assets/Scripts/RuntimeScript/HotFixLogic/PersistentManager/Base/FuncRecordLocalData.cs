using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LitFramework.Persistent
{
    public class FuncRecordLocalData : LocalDataBase
    {
        [Sirenix.OdinInspector.DictionaryDrawerSettings(KeyLabel = "功能名字", ValueLabel = "时间")]
        [Sirenix.OdinInspector.ShowInInspector]
        /// <summary>
        /// 记录功能时间
        /// </summary>
        public Dictionary<string, long> FuncTime = new Dictionary<string, long>();
        [Sirenix.OdinInspector.DictionaryDrawerSettings(KeyLabel = "功能名字", ValueLabel = "次数")]
        [Sirenix.OdinInspector.ShowInInspector]
        /// <summary>
        /// 记录功能数量
        /// </summary>
        public Dictionary<string, int> FuncNum = new Dictionary<string, int>();
    }
}
