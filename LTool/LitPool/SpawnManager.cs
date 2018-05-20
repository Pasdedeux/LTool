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
* Copyright @ Liu Hanwen 2018. All rights reserved.
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

namespace LitFramework.LitPool
{
    /// <summary>
    /// 对外控制用脚本
    /// </summary>
    class SpawnManager : Singleton<SpawnManager>,IManager
    {
        private SpawnPool _pool;
        private List<string> _prefabNameList;


        public SpawnManager()
        {

        }

        /// <summary>
        /// 初始化并获取对象池
        /// </summary>
        public void Install()
        {
            _prefabNameList = new List<string>();

            //初始化对象池
            var pool = GameObject.Find( "PoolManager" );
            if( pool == null )
                pool = new GameObject( "PoolManager" );

            _pool = pool.transform.GetComponent<SpawnPool>();
            if( _pool == null )
                throw new Exception( "对象池初始化失败" );

            //建立对象池库及名单列表
            foreach( var item in _pool.prefabList )
            {
                _prefabNameList.Add( item.name );
                AddPoolManager( _pool , item.transform );
            }
        }


        /// <summary>
        /// 卸载模块
        /// </summary>
        public void Uninstall()
        {
            _prefabNameList.Clear();
            _prefabNameList = null;

            _pool._perPrefabPoolOptions.Clear();
            _pool._perPrefabPoolOptions = null;
            _pool.prefabList.Clear();
            _pool.prefabList = null;
            _pool = null;
            GC.Collect();
        }

        /// <summary>
        /// 从对象池中获取一个对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject SpwanObject( string name )
        {
            if( !_prefabNameList.Contains( name ) )
                throw new Exception( "不存在预制件 " + name );

            return _pool.Spawn( name ).gameObject;
        }

        /// <summary>
        /// 回收入对象池
        /// </summary>
        /// <param name="item"></param>
        public void DespawnObject( Transform item )
        {
            _pool.Despawn( item , _pool.transform );
        }


        /// <summary>
        /// 增加池库
        /// </summary>
        /// <param name="spawnPool"></param>
        /// <param name="transform"></param>
        public static void AddPoolManager( SpawnPool spawnPool , Transform transform , int initNum = 5 , int limitAmount = 50 , bool limitInstances = true , bool limitFIFO = true )
        {
            PrefabPool refabPool = new PrefabPool( transform );
            if( !spawnPool._perPrefabPoolOptions.Contains( refabPool ) )
            {
                refabPool = new PrefabPool( transform );
                //默认初始化两个Prefab
                refabPool.preloadAmount = initNum;
                //开启限制
                refabPool.limitInstances = limitInstances;
                //关闭无限取Prefab
                refabPool.limitFIFO = limitFIFO;
                //限制池子里最大的Prefab数量
                refabPool.limitAmount = limitAmount;
                //开启自动清理池子
                refabPool.cullDespawned = true;
                //最终保留
                refabPool.cullAbove = 20;
                //多久清理一次
                refabPool.cullDelay = 15;
                //每次清理几个
                refabPool.cullMaxPerPass = 10;
                //初始化内存池
                spawnPool._perPrefabPoolOptions.Add( refabPool );
                spawnPool.CreatePrefabPool( refabPool );
            }
        }
    }
}
