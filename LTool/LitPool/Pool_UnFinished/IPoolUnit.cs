#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.LitPool
* 项目描述 ：
* 类 名 称 ：IPoolUnit
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFramework.LitPool
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/5/6 21:28:19
* 更新时间 ：2018/5/6 21:28:19
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Liu Hanwen 2018. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LitFramework.LitPool
{
    public interface IPoolUnit
    {
        PoolType State { get; set; }
        void SetParentList( object parentList );
        void Restore();
    }

    public enum PoolType
    {
        Idle,
        Work
    }
}
