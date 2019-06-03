/*======================================
* 项目名称 ：Assets.Scripts
* 项目描述 ：
* 类 名 称 ：BaseStatitician
* 类 描 述 ：
* 命名空间 ：Assets.Scripts
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/6/3 11:37:43
* 更新时间 ：2019/6/3 11:37:43
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Inplayable 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2019/6/3 11:37:43
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public abstract class BaseStatistician
    {
        public StatisticManager staticMng { get; private set; }
        public BaseStatistician( StatisticManager mng )
        {
            staticMng = mng;
            staticMng.StatisticianList.Add( this );
            staticMng.DOTEventHandler += DOT;
        }

        public abstract void DOT( string key, string value, string tag = null );

        public void Dispose()
        {
            staticMng.DOTEventHandler -= DOT;
            staticMng = null;
        }
    }
}
