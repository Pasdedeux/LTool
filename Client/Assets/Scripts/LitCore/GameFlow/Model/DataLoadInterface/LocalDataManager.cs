/*======================================
* 项目名称 ：LitFramework.GameFlow.Model.DataLoadInterface
* 项目描述 ：
* 类 名 称 ：LocalDataManager
* 类 描 述 ：
* 命名空间 ：LitFramework.GameFlow.Model.DataLoadInterface
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/7/4 16:17:26
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using LitFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LitFramework.GameFlow.Model.DataLoadInterface
{
    public class LocalDataManager : Singleton<LocalDataManager>, IManager
    {
        public Action<LocalDataManager> InstallEventHandler;
        public List<BaseLocalData> LocalDataList { get; set; }

        public void Install()
        {
            LocalDataList = new List<BaseLocalData>();
            InstallEventHandler?.Invoke( this );
            InstallEventHandler = null;
        }

        public void Uninstall()
        {
            for ( int i = LocalDataList.Count - 1; i > -1; i-- )
            {
                LocalDataList[ i ].Dispose();
                LocalDataList[ i ] = null;
                LocalDataList.RemoveAt( i );
            }

            LocalDataList.Clear();
            LocalDataList = null;
            InstallEventHandler = null;
        }
    }
}
