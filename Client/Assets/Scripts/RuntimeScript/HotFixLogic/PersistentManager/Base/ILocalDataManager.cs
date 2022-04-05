using LitFramework.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LitFramework.Persistent
{
    public interface ILocalDataManager : IFuncDataManager
    {
        public LocalDataBase LocalData { get; }
        public void LoadLocalData();
    }
}
