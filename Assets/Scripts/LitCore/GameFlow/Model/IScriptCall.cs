/*======================================
* 项目名称 ：Assets.Scripts.ILRuntime
* 项目描述 ：
* 类 名 称 ：IScriptCall
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.ILRuntime
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/8/16 17:02:57
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

namespace Assets.Scripts
{
    public interface IScriptCall
    {
        void Load();
        void PreStartRun();
        void StartRun();
    }
}
