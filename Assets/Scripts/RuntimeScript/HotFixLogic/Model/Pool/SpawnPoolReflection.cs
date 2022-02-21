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
            LDebug.Log("成功反射调用执行 SpawnPoolReflection -> SpawnReflection");

            RsLoadManager.Instance.UseSpawnPool = false;
            var spawnConfigs = Configs.SpawnConfigDict;
            List<int> ids = spawnConfigs.Keys.ToList();
            try
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    bool poolExist = false;
                    List<PrefabPool> spawnList;
                    var spawnItem = spawnConfigs[ids[i]];
                    if (sp.perPrefabPoolOptions.Exists(e => e.SortSpawnName == spawnItem.SpawnType))
                    {
                        var sorted = sp.perPrefabPoolOptions.Where(e => e.SortSpawnName == spawnItem.SpawnType).First();
                        spawnList = sorted.Pools;
                    }
                    else
                    {
                        SortSpawnPool ssp = new SortSpawnPool();
                        ssp.SortSpawnName = spawnItem.SpawnType;
                        sp.perPrefabPoolOptions.Add(ssp);
                        ssp.Pools = spawnList = new List<PrefabPool>();
                    }

                    GameObject instantiateObj = null;
                    instantiateObj = RsLoadManager.Instance.Load<GameObject>(spawnItem.resPath, FrameworkConfig.Instance.loadType);
                    if (instantiateObj == null)
                    {
                        throw new Exception(string.Format("检查配置表SpawnConfig的ID: {0} 资源是否存在", spawnItem.ID));
                    }

                    int hashCode = instantiateObj.GetHashCode();
                    //判定是否存在于池表，如果存在直接叠加
                    poolExist = spawnList.Exists(e => e.prefabGO.GetHashCode() == hashCode);
                    if (!poolExist)
                    {
                        PrefabPool spawnObj = new PrefabPool();
                        spawnObj.prefab = instantiateObj.transform;
                        spawnObj.preloadAmount = spawnItem.PreloadAmount;
                        spawnObj.inspectorInstanceConstructor();

                        sp.CreatePrefabPool(spawnObj);
                        //动态加池
                        spawnList.Add(spawnObj);
                    }
                    else
                    {
                        var targetPrefabPool = spawnList.Where(e => e.prefabGO.GetHashCode() == hashCode).First();
                        targetPrefabPool.preloadAmount += spawnItem.PreloadAmount;
                        targetPrefabPool.PreloadInstances();
                    }
                }
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
