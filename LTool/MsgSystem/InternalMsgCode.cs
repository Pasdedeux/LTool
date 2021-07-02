/*======================================
* 项目名称 ：LitFramework.MsgSystem
* 项目描述 ：
* 类 名 称 ：InternalMsgCode
* 类 描 述 ：
* 命名空间 ：LitFramework.MsgSystem
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/7/2 18:12:02
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LitFramework.MsgSystem
{
    /// <summary>
    /// 框架内通信事件，主要是必要组件初始化时序管控
    /// </summary>
    public class InternalMsgCode
    {
        //配置表加载完毕
        public static string ConfigLoadedEvent = "LHW_ConfigLoadedEvent";
    }
}
