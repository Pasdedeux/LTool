/*======================================
* 项目名称 ：Assets.Scripts
* 项目描述 ：
* 类 名 称 ：ExtendSpawnPool
* 类 描 述 ：
* 命名空间 ：Assets.Scripts
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/7/22 22:06:38
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using Assets.Scripts.Module;
using LitFramework;
using LitFramework.LitTool;
using PathologicalGames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class ExtendSpawnPool : SpawnPool
    {
        public override void LoadSpawnConfig( bool isHotFix = false )
        {
            if ( loadType == LoadType.None ) return;

            if ( FrameworkConfig.Instance.scriptEnvironment != RunEnvironment.ILRuntime )
                Assets.Scripts.DotNetScriptCall.SetSpawnPool( this );
            else if( isHotFix )
                Assets.Scripts.ILRScriptCall.SetSpawnPool( this );
        }
    }
}
