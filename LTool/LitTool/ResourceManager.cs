/*======================================
* 项目名称 ：Assets.Scripts.Manager
* 项目描述 ：
* 类 名 称 ：ResourceManager
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Manager
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/7/5 11:33:15
* 更新时间 ：2019/7/5 11:33:15
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Inplayable 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note: 直接使用Unity2018 以后的版本，暂不支持旧版本
*修改时间：2019/7/5 11:33:15
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using LitFramework;
using LitFramework.Base;
using LitFramework.LitTool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


namespace LitFramework.LitTool
{
    public class ResourceManager : Singleton<ResourceManager>, IManager
    {
        public event Action<string> LoadErrorEventHandler;
        public event Action<string> LoadFileWithStringEventHandler;
        public event Action<byte[]> LoadFileWithRawDataEventHandler;
        public event Action<Texture2D> LoadTextureEventHandler;
        public event Action<AssetBundle> LoadAssetBundleEventHandler;

        private Dictionary<string, Sprite[]> _atlasDict;
        public void Install()
        {
            _atlasDict = new Dictionary<string, Sprite[]>();
        }

        public void Uninstall()
        {
            _atlasDict.Clear();
            _atlasDict = null;
            Resources.UnloadUnusedAssets();
        }



        /// <summary>
        /// 加载图集中的子对象
        /// </summary>
        /// <param name="spriteName"></param>
        /// <param name="atlasPath"></param>
        /// <returns></returns>
        public Sprite LoadSpriteAtlas(string spriteName, string atlasPath = null)
        {
            //常驻内存
            Sprite sprite = Resources.Load<Sprite>(spriteName);

            if (sprite != null || string.IsNullOrEmpty(atlasPath))
            {
                return GameObject.Instantiate<Sprite>( sprite );
            }
            if (!_atlasDict.ContainsKey(atlasPath))
            {
                Sprite[] atlasSprites = Resources.LoadAll<Sprite>(atlasPath);
                _atlasDict.Add(atlasPath, atlasSprites);
            }

            var sprites = _atlasDict[atlasPath];
            var length = _atlasDict[atlasPath].Length;
            for (int i = 0; i < length; i++)
            {
                if (sprites[i].name.Equals(string.Concat(new string[] { atlasPath, "_", spriteName })))

                    return sprite = sprites[i];
            }
            return GameObject.Instantiate<Sprite>( sprite );
        }


        #region UnityWebRequest  尚未完成
        /// <summary>
        /// 加载 .txt/ .dat/ .csv等文件
        /// </summary>
        /// <param name="searchPath">要加载文件的【带后缀】完整路径</param>
        /// <param name="useRawDataArray">true - 返回的是download data    false - 直接返回结果字符串</param>
        /// <returns></returns>
        private IEnumerator DoLoadFile( string searchPath , bool useRawDataArray = false )
        {
            //todo 尚未完成整合
            using ( UnityWebRequest request = UnityWebRequest.Get( searchPath ) ) 
            {
                yield return request.SendWebRequest();
                if ( request.isHttpError || request.isNetworkError )
                {
                    LoadErrorEventHandler?.Invoke( request.error );
                    Debug.LogError( request.error );
                }
                else
                {
                    if ( useRawDataArray )
                        LoadFileWithRawDataEventHandler?.Invoke( request.downloadHandler.data );
                    else
                        LoadFileWithStringEventHandler?.Invoke( request.downloadHandler.text );
                }
            }
        }

        /// <summary>
        /// 加载png 等图像格式
        /// </summary>
        /// <param name="searchPath">要加载文件的【带后缀】完整路径</param>
        /// <returns></returns>
        private IEnumerator DoLoadTexture( string searchPath )
        {
            using ( UnityWebRequest request = UnityWebRequestTexture.GetTexture( searchPath ) )
            {
                yield return request.SendWebRequest();
                if ( request.isHttpError || request.isNetworkError )
                {
                    LoadErrorEventHandler?.Invoke( request.error );
                    Debug.LogError( request.error );
                }
                else
                {
                    Texture2D texture = ( request.downloadHandler as DownloadHandlerTexture ).texture;
                    LoadTextureEventHandler?.Invoke( texture );
                }
            }
        }

        /// <summary>
        /// 加载AssetBundle
        /// </summary>
        /// /// <param name="searchPath">要加载文件的【带后缀】完整路径</param>
        /// <returns></returns>
        private IEnumerator DoLoadAssetBundle( string searchPath )
        {
            using ( UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle( searchPath ) )
            {
                yield return request.SendWebRequest();
                if ( request.isHttpError || request.isNetworkError )
                {
                    LoadErrorEventHandler?.Invoke( request.error );
                    Debug.LogError( request.error );
                }
                else
                {
                    AssetBundle assetBundle = ( request.downloadHandler as DownloadHandlerAssetBundle ).assetBundle;
                    LoadAssetBundleEventHandler?.Invoke( assetBundle );
                }
            }
        }
        #endregion
    }
}
