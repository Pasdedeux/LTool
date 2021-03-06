﻿/*======================================
* 项目名称 ：LitFramework
* 项目描述 ：
* 类 名 称 ：IManager
* 类 描 述 ：
*                   
* 命名空间 ：LitFramework.UI.Base
* 机器名称 ：SKY-20170413SEJ 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/5/17 11:50:24
* 更新时间 ：2018/5/17 11:50:24
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ DerekLiu 2018. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2018/5/17 11:50:24
*修改人： LHW
*版本号： V1.0.0.0
*描述：
*
======================================*/

using System;
using UnityEngine;

namespace LitFramework.Base
{
    public interface IManager
    {
        void Install();
        void Uninstall();
    }
}
