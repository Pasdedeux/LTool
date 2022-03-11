/*======================================
* 项目名称 ：ET.Codes.Model.Demo.RoleInfo
* 项目描述 ：
* 类 名 称 ：RoleInfo
* 类 描 述 ：
* 命名空间 ：ET.Codes.Model.Demo.RoleInfo
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2022/2/9 16:39:19
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2022. All rights reserved.
*******************************************************************
======================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public enum RoleInfoState
    {
        Normal,
        Freeze,
    }

    public class RoleInfo : Entity, IAwake
    {
        public string Name;
        public int ServerId;
        public int State;
        public long AccountId;
        public long LastLoginTime;
        public long CreateTime;
    }
}
