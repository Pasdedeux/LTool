/*======================================
* 项目名称 ：ET.Codes.Model.Demo.Account
* 项目描述 ：
* 类 名 称 ：AccountInfoComponent
* 类 描 述 ：
* 命名空间 ：ET.Codes.Model.Demo.Account
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2022/2/7 13:17:45
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
    public class AccountInfoComponent : Entity, IAwake, IDestroy
    {
        public string Token;
        public long AccountID;

        public string RealmKey;
        public string RealmAddress;
    }
}
