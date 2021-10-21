/*======================================
* 项目名称 ：Assets.Scripts.Module
* 项目描述 ：
* 类 名 称 ：ABLoader
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Module
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/8/2 16:12:31
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using LitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Module
{
    /// <summary>
    /// AB包加载器
    /// </summary>
    public class ABLoader : Singleton<ABLoader>
    {
        private AssetBundle _mainAB = null;
        private AssetBundleManifest _mainManifest = null;
        private Dictionary<string, AssetBundle> _bundleDic = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// AB包路径
        /// </summary>
        private string ABFilePath
        {
            get
            {
                return FrameworkConfig.Instance.UsePersistantPath ? Application.persistentDataPath + string.Format( "/{0}/", FrameworkConfig.Instance.ABFolderName ) : Application.streamingAssetsPath + string.Format( "/{0}/", FrameworkConfig.Instance.ABFolderName );
            }
        }

        /// <summary>
        /// 主包名字
        /// </summary>
        private string MainABName
        {
            get
            {
#if UNITY_IOS
            
#elif UNITY_ANDROID

#else

#endif
                return FrameworkConfig.Instance.ABTotalName;
            }
        }

        /// <summary>
        /// 加载AB包
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private AssetBundle LoadAB( string name )
        {
            if ( _mainAB == null )
            {
                Debug.Log( ABFilePath + MainABName );
                _mainAB = AssetBundle.LoadFromFile( ABFilePath + MainABName );
                _mainManifest = _mainAB.LoadAsset<AssetBundleManifest>( "AssetBundleManifest" );
            }
            string[] str = _mainManifest.GetAllDependencies( name );

            for ( int i = 0; i < str.Length; i++ )
            {
                if ( !_bundleDic.ContainsKey( str[ i ] ) )
                {
                    AssetBundle ab = AssetBundle.LoadFromFile( ABFilePath + str[ i ] );
                    _bundleDic.Add( str[ i ], ab );
                }
            }
            if ( !_bundleDic.ContainsKey( name ) )
            {
                AssetBundle ab = AssetBundle.LoadFromFile( ABFilePath + name );
                _bundleDic.Add( name, ab );
            }
            return _bundleDic[ name ];
        }

        /// <summary>
        /// 从AB包中加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        public T LoadFromAB<T>( string rule ) where T : UnityEngine.Object
        {
            string[] names = rule.Split( '#' );
            string abName = names[ 0 ];
            string resName = names[ 1 ];
            AssetBundle ab = LoadAB( abName );
            return ab.LoadAsset<T>( resName );
        }

        /// <summary>
        /// 卸载没有被使用的资源
        /// </summary>
        public void RemoveMenory()
        {
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 卸载单个AB包
        /// </summary>
        /// <param name="name"></param>
        public void ClearOneAB( string name )
        {
            if ( _bundleDic.ContainsKey( name ) )
            {
                _bundleDic[ name ].Unload( false );
                _bundleDic.Remove( name );
            }
        }

        /// <summary>
        /// 卸载所有AB包
        /// </summary>
        public void ClearAll()
        {
            AssetBundle.UnloadAllAssetBundles( false );
            _bundleDic.Clear();
            _mainAB = null;
            _mainManifest = null;
        }
    }
}
