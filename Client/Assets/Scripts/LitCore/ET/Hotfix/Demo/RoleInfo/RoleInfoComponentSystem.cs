/*======================================
* 项目名称 ：ET.Codes.Hotfix.Demo.RoleInfo
* 项目描述 ：
* 类 名 称 ：RoleInfoComponentSystem
* 类 描 述 ：
* 命名空间 ：ET.Codes.Hotfix.Demo.RoleInfo
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2022/2/13 14:27:49
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

    public class RoleInfoComponentDestroySystem : DestroySystem<RoleInfoComponent>
    {
        public override void Destroy(RoleInfoComponent self)
        {
            foreach (var item in self.roleInfos)
            {
                item?.Dispose();
            }
            self.roleInfos.Clear();
            self.CurrentRoleId = 0;
        }
    }

    public static class RoleInfoComponentSystem
    {
    }
}
