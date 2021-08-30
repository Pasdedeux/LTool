/*======================================
* 项目名称 ：LitFramework.MsgSystem
* 项目描述 ：
* 类 名 称 ：InternalEvent
* 类 描 述 ：
* 命名空间 ：LitFramework.MsgSystem
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/7/30 18:10:38
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
    public sealed class InternalEvent
    {
        public enum RemoteStatus
        {
            Download,
            Write
        }

        //远程下载出错
        public static string REMOTE_UPDATE_ERROR = "lhwUtVKjI=REMOTE_LOAD_FAIL";
        //开始远程下载配置文件
        public static string START_LOAD_REMOTE_CONFIG = "lhwUtVKjI=START_LOAD_REMOTE_CONFIG";
        //远程更新结束
        public static string END_LOAD_REMOTE_CONFIG = "lhwUtVKjI=END_LOAD_REMOTE_CONFIG";
        //带参数。对特定文件进入处理阶段：下载、写入
        public static string HANDLING_REMOTE_RES = "lhwUtVKjI=HANDLING_REMOTE_RES";
    }
}
