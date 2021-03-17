#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework
* 项目描述 ：
* 类 名 称 ：AppConfig
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFramework
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/5/26 16:29:30
* 更新时间 ：2018/5/26 16:29:30
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2018. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LitFramework
{
    /// <summary>
    /// 框架控制参数
    /// </summary>
    public class FrameworkConfig:SingletonMono<FrameworkConfig>
    {
        /// <summary>
        /// 延迟调用函数计时检测间隔
        /// </summary>
        [Header( "延迟调用函数计时检测间隔" )]
        public float DelayFuncDetectInterver = 0.1f;
        /// <summary>
        /// 开启使用逐帧遍历延迟函数调用。默认为false
        /// </summary>
        [Header( "开启使用逐帧遍历延迟函数调用。默认为false" )]
        public bool UseDelayFuncPreciseDetect = false;

        
    }
}
