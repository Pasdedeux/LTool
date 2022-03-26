#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.TimeRecord
* 项目描述 ：
* 类 名 称 ：ZeroTimeRecord
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFramework.TimeRecord
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2021/3/30 0:35:26
* 更新时间 ：2021/3/30 0:35:26
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Liu Hanwen 2021. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using LitFramework.Base;
using LitFramework.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LitFramework.TimeRecord
{
    /// <summary>
    /// 零点访问器。用于单机版本记录零点刷新
    /// 单机做零点刷新，一般就以本地时间即可
    /// </summary>
    public class ZeroTimeRecord:Singleton<ZeroTimeRecord>, IManager
    {
        public void Install()
        {
            GameDriver.Instance.DelZeroTime += ZeroUpdateTime;
        }

        public void Uninstall()
        {
            GameDriver.Instance.DelZeroTime -= ZeroUpdateTime;
        }

        private void ZeroUpdateTime( DateTime dateTime )
        {
            RecordDay = dateTime.Day;
            RecordMonth = dateTime.Month;
            RecordYear = dateTime.Year;
        }

        private static int _recordDay;
        public static int RecordDay
        {
            set { _recordDay = value; PlayerPrefs.SetInt( "dd", _recordDay ); }
            get
            {
                if ( _recordDay != 0 ) return _recordDay;
                return _recordDay = PlayerPrefs.GetInt( "dd", 1 );
            }
        }

        private static int _recordMonth;
        public static int RecordMonth
        {
            set { _recordMonth = value; PlayerPrefs.SetInt( "mm", _recordMonth ); }
            get
            {
                if ( _recordMonth != 0 ) return _recordMonth;
                return _recordMonth = PlayerPrefs.GetInt( "mm", 1 );
            }
        }

        private static int _recordYear;
        public static int RecordYear
        {
            set { _recordYear = value; PlayerPrefs.SetInt( "yy", _recordYear ); }
            get
            {
                if ( _recordYear != 0 ) return _recordYear;
                return _recordYear = PlayerPrefs.GetInt( "yy", 1970 );
            }
        }
    }
}
