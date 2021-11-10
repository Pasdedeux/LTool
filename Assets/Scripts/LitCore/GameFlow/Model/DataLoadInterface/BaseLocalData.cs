/*======================================
* 项目名称 ：LitFramework.GameFlow.Model.DataLoadInterface
* 项目描述 ：
* 类 名 称 ：BaseLocalData
* 类 描 述 ：
* 命名空间 ：LitFramework.GameFlow.Model.DataLoadInterface
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/7/4 16:23:06
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LitFramework.GameFlow.Model.DataLoadInterface
{
    public abstract class BaseLocalData
    {
        public LocalDataManager staticMng { get; private set; }

        public BaseLocalData( LocalDataManager mng )
        {
            staticMng = mng;
            staticMng.LocalDataList.Add( this );
            Load();
        }

        public void Dispose()
        {
            staticMng.LocalDataList.Remove( this );
            staticMng = null;
        }

        public abstract void Load();
    }
}
