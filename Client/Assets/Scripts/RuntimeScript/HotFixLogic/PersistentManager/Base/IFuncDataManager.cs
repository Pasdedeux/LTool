using ILRBaseModel.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LitFramework.Persistent
{
    public interface IFuncDataManager :IManager
    {
        /// <summary>
        /// 首次登录初始化
        /// </summary>
        public void FirstIniteData();
    }
}
