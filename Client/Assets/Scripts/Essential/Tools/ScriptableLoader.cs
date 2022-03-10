/*======================================
* 项目名称 ：Assets.Scripts.Essential.ConfigParsing
* 项目描述 ：
* 类 名 称 ：ScriptableLoader
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Essential.ConfigParsing
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/9/8 13:56:48
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
using Assets.Scripts.Module;
using LitFramework;
using UnityEngine;

namespace Assets.Scripts.Essential.ConfigParsing
{
    /// <summary>
    /// 这个类用于对接项目中使用Scriptable并且需要支持热更的情况。
    /// 流程上需要在主工程中取出其中的信息，注入到对接类中（热更情况下，此类在热更工程中）
    /// </summary>
    public class ScriptableLoader: Singleton<ScriptableLoader>
    {
        /// <summary>
        /// 用于将Resource下的ScriptableObject读取并注入到对应环境下的类里
        /// </summary>
        /// <param name="scriptCommonData"></param>
        public void InjectScriptableData( string scriptCommonData )
        {
            //例如：
            //var commonData = RsLoadManager.Instance.Load<UnityEngine.Object>("CommonAsset/CommonData");

            //if ( FrameworkConfig.Instance.scriptEnvironment != RunEnvironment.ILRuntime )
            //    Assets.Scripts.DotNetScriptCall.SetCommonData( commonData );
            //else
            //    Assets.Scripts.ILRScriptCall.SetCommonData( commonData );
        }
    }
}
