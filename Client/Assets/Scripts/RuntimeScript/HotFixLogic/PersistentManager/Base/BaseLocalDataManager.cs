using LitFramework;
using LitFramework.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LitFramework.Persistent
{
    public abstract class BaseLocalDataManager<T, M> : BaseFuncDataManager<T>, ILocalDataManager
    where T : BaseLocalDataManager<T, M>, new()
    where M : LocalDataBase, new()
    {

        public virtual void LoadLocalData()
        {
            _localData = LocalDataHandle.LoadData<M>();
        }

        protected M _localData;
        public virtual M LocalData
        {
            get => _localData;
        }

        LocalDataBase ILocalDataManager.LocalData => LocalData;
    }
}

