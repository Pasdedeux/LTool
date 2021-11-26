/*======================================
* 项目名称 ：Assets.Scripts.Module.HotFix
* 项目描述 ：
* 类 名 称 ：IHotFix
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Module.HotFix
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/8/2 15:20:08
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using System.Collections;

namespace Assets.Scripts.Module.HotFix
{
    public interface IHotFix
    {
        void MoveExecute();
        IEnumerator HotFixExecute();
        void FinalExecute();
    }
}
