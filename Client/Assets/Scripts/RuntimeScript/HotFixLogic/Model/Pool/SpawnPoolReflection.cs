/*======================================
* 项目名称 ：Assets.Scripts.ILRuntime.HotFixLogic.Module.FrameworkSys
* 项目描述 ：
* 类 名 称 ：SpawnPoolReflection
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.ILRuntime.HotFixLogic.Module.FrameworkSys
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/9/1 19:50:26
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using Assets.Scripts.Module;
using LitFramework;
using LitFramework.LitPool;
using LitFramework.LitTool;
using PathologicalGames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FrameworkSys
{
    public class SpawnPoolReflection
    {
        /// <summary>
        /// Configs文件在热更模式下存在于热更DLL中，为了方便调用，这个类需要跟随热更工程移动
        /// </summary>
        /// <param name="sp"></param>
        public static void SpawnReflection( SpawnPool sp )
        {
            Log.TraceInfo("成功反射调用执行 SpawnPoolReflection -> SpawnReflection");

            RsLoadManager.Instance.UseSpawnPool = false;
            var spawnConfigs = Configs.SpawnConfigDict;
            List<int> ids = spawnConfigs.Keys.ToList();
            try
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    var spawnItem = spawnConfigs[ids[i]];
                    SpawnManager.Instance.CreatePool(spawnItem.resPath, tag: spawnItem.SpawnType, spawnItem.PreloadAmount, basePool: sp);
                }
                Resources.UnloadUnusedAssets();
            }
            catch (Exception e)
            {
                LDebug.LogError("SpawnConfig Relection Error " + e);
            }
            finally
            {
                RsLoadManager.Instance.UseSpawnPool = true;
            }

        }
    }
}
