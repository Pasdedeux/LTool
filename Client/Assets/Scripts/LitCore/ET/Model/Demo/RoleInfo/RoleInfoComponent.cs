/*======================================
* 项目名称 ：ET.Codes.Model.Demo.RoleInfo
* 项目描述 ：
* 类 名 称 ：RoleInfoComponent
* 类 描 述 ：
* 命名空间 ：ET.Codes.Model.Demo.RoleInfo
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2022/2/13 14:25:25
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
    public class RoleInfoComponent: Entity, IAwake, IDestroy
    {
        public List<RoleInfo> roleInfos = new List<RoleInfo>();
        public long CurrentRoleId = 0;
    }
}
