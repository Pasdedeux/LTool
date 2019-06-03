/*======================================
* 项目名称 ：Assets.Scripts
* 项目描述 ：
* 类 名 称 ：Statistician
* 类 描 述 ：
* 命名空间 ：Assets.Scripts
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/6/3 10:33:09
* 更新时间 ：2019/6/3 10:33:09
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Inplayable 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2019/6/3 10:33:09
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitFramework;
using LitFramework.Base;

namespace Assets.Scripts
{
    public class StatisticManager : Singleton<StatisticManager>,IManager
    {
        public Action<StatisticManager> InstallEventHandler;
        //统计绑定接口
        public event Action<string, string, string> DOTEventHandler;
        public List<BaseStatistician> StatisticianList { get; set; }

        public void DOT( string key, string value, string tag = null )
        {
            if ( DOTEventHandler != null )
            {
                DOTEventHandler.Invoke( key, value, tag );
            }
        }

        public void Install()
        {
            StatisticianList = new List<BaseStatistician>();
            if ( InstallEventHandler != null ) InstallEventHandler( this );
        }

        public void Uninstall()
        {
            for ( int i = StatisticianList.Count-1; i > -1; i-- )
            {
                StatisticianList[ i ].Dispose();
                StatisticianList[ i ] = null;
                StatisticianList.RemoveAt( i );
            }
            
            StatisticianList.Clear();
            StatisticianList = null;
            InstallEventHandler = null;
        }
    }
}
