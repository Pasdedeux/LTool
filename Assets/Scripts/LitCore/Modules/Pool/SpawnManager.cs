/*======================================
* 项目名称 ：LitFramework
* 项目描述 ：
* 类 名 称 ：SpawnManager
* 类 描 述 ：
* 命名空间 ：LitFramework.LitPool
* 机器名称 ：SKY-20170413SEJ 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/5/7 15:23:37
* 更新时间 ：2018/5/7 15:23:37
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2018. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2018/5/8 15:23:37
*修改人： LHW
*版本号： V1.0.0.0
*描述：
*
* 
======================================*/

using PathologicalGames;
using System;
using System.Collections.Generic;
using UnityEngine;
using LitFramework;
using LitFramework.UI.Base;
using LitFramework.Base;
using System.Linq;
using UnityEngine.Assertions;
using LitFramework.Singleton;

namespace LitFramework.LitPool
{
    /// <summary>
    /// 对外控制用脚本
    /// </summary>
    public class SpawnManager : Singleton<SpawnManager>, IManager
    {
        public SpawnPool Pool { get; private set; }

        /// <summary>
        /// 初始化并获取对象池
        /// </summary>
        public void Install()
        {
            //初始化对象池
            Pool = GameObject.FindObjectOfType<SpawnPool>();
            if ( Pool == null )
            {
                GameObject pool = new GameObject( "PoolManager" );
                Pool = pool.gameObject.AddComponent<SpawnPool>();
            }
            if ( Pool == null ) throw new Exception( "对象池初始化需要预先建立对象 PoolConfig节点，并挂载配置 ExtentedSpawnPool" );

            GameObject.DontDestroyOnLoad( Pool.gameObject );

            Pool.LoadSpawnConfig();
        }

        /// <summary>
        /// 为指定对象创建池。并指定该池的归属标签及池化参数
        /// </summary>
        /// <param name="resPath">需要池化的对象数量</param>
        /// <param name="tag">池的归属标签</param>
        /// <param name="preloadAmount">预先缓存的物体数量</param>
        /// <param name="maxAmount">最大缓存数量，默认不设定最大值</param>
        /// <param name="basePool">池根节点。默认不指定</param>
        /// <exception cref="Exception">不允许整个池中出现多个相同待池化对象</exception>
        public void CreatePool(string resPath, string tag = null, int preloadAmount = 2, int maxAmount = -1, SpawnPool basePool = null )
        {
            bool poolExist = false;
            List<PrefabPool> spawnList;
            SpawnPool usePool = basePool ?? Pool;

            //检测当前标签池是否存在
            if (usePool.perPrefabPoolOptions.Exists(e => e.SortSpawnName.Equals(tag))) 
            {
                var sorted = usePool.perPrefabPoolOptions.Where(e => e.SortSpawnName.Equals(tag)).First();
                spawnList = sorted.Pools;
            }
            else
            {
                SortSpawnPool ssp = new SortSpawnPool();
                ssp.SortSpawnName = tag;
                usePool.perPrefabPoolOptions.Add(ssp);
                ssp.Pools = spawnList = new List<PrefabPool>();
            }

            //向该类表现生成指定数目的对象
            GameObject instantiateObj = null;
            instantiateObj = RsLoadManager.Instance.Load<GameObject>(resPath, FrameworkConfig.Instance.loadType);
            if (instantiateObj == null)
            {
                throw new Exception(string.Format("检查待池化对象是否存在: {0} ", resPath));
            }

            int hashCode = instantiateObj.GetHashCode();
            //目前并不允许多个标签池出现同一个预制对象
            var otherPool = usePool.perPrefabPoolOptions.Where(e => !e.SortSpawnName.Equals(tag));
            foreach (var item in otherPool)
            {
                if (item.Pools.Exists(e => e.prefabGO.GetHashCode() == hashCode))
                {
                    throw new Exception($"对象池存在重复对象：当前页签 {tag}  其它页签{item.SortSpawnName}");
                }
            }

            //判定是否存在于池表，如果存在直接叠加
            poolExist = spawnList.Exists(e => e.prefabGO.GetHashCode() == hashCode);
            if (!poolExist)
            {
                PrefabPool spawnObj = new PrefabPool();
                spawnObj.prefab = instantiateObj.transform;
                spawnObj.preloadAmount = preloadAmount;
                if(maxAmount>0)
                {
                    spawnObj.limitInstances = true;
                    spawnObj.limitAmount = maxAmount;
                }
                else
                {
                    spawnObj.limitInstances = false;
                }
                spawnObj.inspectorInstanceConstructor();

                usePool.CreatePrefabPool(spawnObj);
                //动态加池
                spawnList.Add(spawnObj);
            }
            else
            {
                var targetPrefabPool = spawnList.Where(e => e.prefabGO.GetHashCode() == hashCode).First();
                targetPrefabPool.preloadAmount += preloadAmount;
                targetPrefabPool.PreloadInstances();
            }
        }

        /// <summary>
        /// 移除指定对象的池
        /// </summary>
        /// <param name="target">销毁物体实例</param>
        /// <param name="basePool">默认不传</param>
        public void RemovePool(Transform targetTransInstance, SpawnPool basePool = null)
        {
            if (targetTransInstance == null) return;
            SpawnPool usePool = basePool ?? Pool;

            var targetPool = usePool.GetPrefabPoolByInstance(targetTransInstance);
            //targetPool = usePool.GetPrefabPool((GameObject)RsLoadManager.Instance.Load("Prefabs/Test/Image_1"));
            usePool.DeletePrefabPool(targetPool);
            if (targetPool != null) targetPool.Destroy();

            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        /// <summary>
        /// 移除指定对象的池
        /// </summary>
        /// <param name="targetGOInstance">销毁物体实例</param>
        /// <param name="basePool">默认不传</param>
        public void RemovePool(GameObject targetGOInstance, SpawnPool basePool = null)
        {
            if (targetGOInstance == null) return;
            RemovePool(targetGOInstance.transform, basePool);
        }

        /// <summary>
        ///  移除指定标签的池。可能包含多个对象池
        /// </summary>
        /// <param name="spawnType">池标签</param>
        /// <param name="basePool">默认不传</param>
        public void RemovePool(string spawnType, SpawnPool basePool = null)
        {
            SpawnPool usePool = basePool ?? Pool;
            if (usePool != null && usePool.perPrefabPoolOptions.Exists( e=>e.SortSpawnName.Equals(spawnType) ))
            {
                var poolOfTag = usePool.perPrefabPoolOptions.Where(e => e.SortSpawnName.Equals(spawnType)).First();
                poolOfTag.Pools.ForEach(pool => pool.Destroy());
                poolOfTag.Pools.Clear();
                usePool.perPrefabPoolOptions.Remove(poolOfTag);
            }
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }


        /// <summary>
        /// 卸载模块
        /// </summary>
        public void Uninstall()
        {
            for (int i = Pool.perPrefabPoolOptions.Count-1; i > -1; i--)
            {
                Pool.perPrefabPoolOptions[i].SortSpawnName = String.Empty;
                Pool.perPrefabPoolOptions[i].Pools.ForEach(pool => pool.Destroy());
                Pool.perPrefabPoolOptions[i].Pools.Clear();
                Pool.perPrefabPoolOptions.RemoveAt(i);
            }

            Pool.prefabPools.Clear();
            Pool.perPrefabPoolOptions.Clear();
            Pool.perPrefabPoolOptions = null;
            Pool = null;

            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        /// <summary>
        /// 从对象池中获取一个对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject SpwanObject( string name )
        {
            if (Pool == null) return null;
            return Pool.Spawn( name ).gameObject;
        }


        /// <summary>
        /// 是否属于池化
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public bool IsSpawned( Transform transform )
        {
            if (Pool == null) return false;
            return Pool.IsSpawned(transform);
        }

        /// <summary>
        /// 是否属于池化
        /// </summary>
        /// <param name="string"></param>
        /// <returns></returns>
        public bool IsSpawned(string name)
        {
            if (Pool == null) return false;
            return Pool.IsSpawned(name);
        }

        /// <summary>
        /// 回收入对象池
        /// </summary>
        /// <param name="item"></param>
        public void DespawnObject( Transform item )
        {
            if (Pool == null) return;
            Pool.Despawn( item, Pool.transform );
        }
    }
}
