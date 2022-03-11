/*======================================
* 项目名称 ：ET.Codes.Hotfix.Demo.RoleInfo
* 项目描述 ：
* 类 名 称 ：RoleInfoSystem
* 类 描 述 ：
* 命名空间 ：ET.Codes.Hotfix.Demo.RoleInfo
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2022/2/9 16:55:28
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
    public static class RoleInfoSystem
    {
        public static void FromMessage( this RoleInfo self, RoleInfoProto roleInfoProto )
        {
            self.AccountId = roleInfoProto.AccountId;
            self.Name = roleInfoProto.Name;
            self.CreateTime = roleInfoProto.CreateTime;
            self.Id = roleInfoProto.Id;
            self.ServerId = roleInfoProto.ServerId;
            self.State = roleInfoProto.State;
            self.LastLoginTime = roleInfoProto.LastLoginTime;
        }

        public static RoleInfoProto ToMessage( this RoleInfo self)
        {
            return new RoleInfoProto()
            {
                AccountId = self.AccountId,
                Name = self.Name,
                CreateTime = self.CreateTime,
                Id = self.Id,
                State = self.State,
                ServerId = self.ServerId,
                LastLoginTime = self.LastLoginTime,
                
            };
        }
    }
}
