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

using PathologicalGames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class ExtendSpawnPool:SpawnPool
    {
        public override void LoadSpawnConfig()
        {
            Dictionary<string, SpawnConfig> spawnConfigs = Configs.SpawnConfigDict;

            foreach ( var item in spawnConfigs )
            {
                List<PrefabPool> spawnList;
                SpawnConfig spawnItem = item.Value;
                if ( this.perPrefabPoolOptions.Exists( e => e.SortSpawnName == spawnItem.SpawnType ) )
                {
                    var sorted = this.perPrefabPoolOptions.Where( e => e.SortSpawnName == spawnItem.SpawnType ).First();
                    spawnList = sorted.Pools;
                }
                else
                {
                    SortSpawnPool ssp = new SortSpawnPool();
                    ssp.SortSpawnName = spawnItem.SpawnType;
                    perPrefabPoolOptions.Add( ssp );
                    ssp.Pools = spawnList = new List<PrefabPool>();
                }

                GameObject instantiateObj = null;
                switch ( loadType )
                {
                    case LoadType.AB:
                        instantiateObj = AssetDriver.Instance.LoadFromAB<GameObject>( spawnItem.ABName );
                        break;
                    case LoadType.Resources:
                        instantiateObj = Instantiate( Resources.Load<GameObject>( spawnItem.resPath ) ) as GameObject;
                        break;
                }
                if ( instantiateObj == null )
                {
                    LDebug.LogError( string.Format( "检查配置表SpawnConfig的ID: {0} 资源是否存在", spawnItem.ID ) );
                    continue;
                }
                string name = instantiateObj.name.Replace( "(Clone)", "" );
                Transform spawnTransform = instantiateObj.transform;
                spawnTransform.gameObject.SetActive( false );
                spawnTransform.name = name;

                PrefabPool spawnObj = new PrefabPool();
                spawnObj.prefab = spawnTransform;
                spawnObj.preloadAmount = spawnItem.PreloadAmount;
                spawnObj.inspectorInstanceConstructor();

                CreatePrefabPool( spawnObj );

                spawnList.Add( spawnObj );

                Resources.UnloadUnusedAssets();
            }
        }
    }
}
