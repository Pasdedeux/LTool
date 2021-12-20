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
            if ( Pool == null ) throw new Exception( "对象池初始化需要预先建立对象 PoolManager 并挂载配置 SpawnPool" );

            GameObject.DontDestroyOnLoad( Pool.gameObject );

            Pool.LoadSpawnConfig();
        }


        /// <summary>
        /// 卸载模块
        /// </summary>
        public void Uninstall()
        {
            Pool.prefabPools.Clear();
            Pool.perPrefabPoolOptions.Clear();
            Pool.perPrefabPoolOptions = null;
            Pool = null;
            GC.Collect();
        }

        /// <summary>
        /// 从对象池中获取一个对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject SpwanObject( string name )
        {
            return Pool.Spawn( name ).gameObject;
        }

        /// <summary>
        /// 回收入对象池
        /// </summary>
        /// <param name="item"></param>
        public void DespawnObject( Transform item )
        {
            Pool.Despawn( item, Pool.transform );
        }
        /// <summary>
        /// 动态创建对象进入对象池
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="spawnName"></param>
        public void CreateSpawnPool(Transform prefab)
        {
            PrefabPool newPrefabPool = new PrefabPool(prefab);
            Pool.CreatePrefabPool(newPrefabPool);
        }
    }
}
