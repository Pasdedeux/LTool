/*======================================
* 项目名称 ：Assets.Scripts.Essential.Managers.RsCom
* 项目描述 ：
* 类 名 称 ：RsLoadAB
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Essential.Managers.RsCom
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/9/27 13:46:45
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using LitFramework;
using LitFramework.LitTool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Essential.Managers.RsCom
{
    class ReBundle
    {
        public string Path;
        public AssetBundle assetBundle;
        public bool isEnd = false;
        public System.Action<AssetBundle> onComplent;
        public void Reset()
        {
            onComplent = null;
            assetBundle = null;
            Path = null;
            isEnd = false;
        }

    }

    /// <summary>
    /// 资源异步加载方式
    /// </summary>
    internal enum ResoucesASYLoadTP
    {
        File,
        Memory,
        Web,
    }

    public class RsLoadAB : IRsLoad
    {
        private string AbPath;
        private string m_Url = @"file:\\F:\UnityPackge\xscrosstalk2\UnityProject\Assets\StreamingAssets\";
        private ResoucesASYLoadTP m_ResoucesTP;
        private List<ReBundle> RecoveryReBundleList = new List<ReBundle>();
        private Dictionary<string, string> PathToAB = new Dictionary<string, string>();
        private Dictionary<string, ReBundle> m_BundlePool = new Dictionary<string, ReBundle>();

        public RsLoadAB()
        {
            
        }

        public void AfterInit()
        {
            string path = FrameworkConfig.Instance.UsePersistantPath ? Application.persistentDataPath + "/" : Application.streamingAssetsPath + "/";
            DocumentAccessor.LoadAsset(path + "ABPath.csv", (string e) =>
            {
                string[] str = e.Split('\n');
                for (int i = 1; i < str.Length; i++)
                {
                    string line = str[i];
                    if (line != "")
                    {
                        string[] content = line.Split(',');
                        PathToAB.Add(content[0].Trim(), content[1].Trim());
                    }
                }
            });
            m_ResoucesTP = ResoucesASYLoadTP.File;
            AbPath = path;
            AbPath = AbPath + FrameworkConfig.Instance.ABFolderName + "/";
            m_Url = m_Url + FrameworkConfig.Instance.ABFolderName + "/";
        }

        public UnityEngine.Object Load( string aPath )
        {
            string abName;
            UnityEngine.Object rs = null;

            if ( PathToAB.TryGetValue( aPath, out abName ) )
            {
                AssetBundle ab = LoadAB( abName );
                if ( ab )
                {
                    string aName = ResoucesName( aPath );
                    rs = ab.LoadAsset( aName );
                }

                CheckShader( rs );
            }
            else
            {
                rs = Resources.Load( aPath );
            }
            return rs;
        }

        public T Load<T>( string aPath ) where T : UnityEngine.Object
        {
            string abName;
            T rs = default( T );

            if ( PathToAB.TryGetValue( aPath, out abName ) )
            {
                AssetBundle ab = LoadAB( abName );
                if ( ab )
                {
                    string aName = ResoucesName( aPath );
                    rs = ab.LoadAsset<T>( aName );
                    CheckShader( rs );
                }
            }
            else
            {
                rs = Resources.Load<T>( aPath );
            }
            return rs;
        }

        public void LoadAsync( string aPath, Action<UnityEngine.Object> onComplent )
        {
            string abName;
            if ( PathToAB.TryGetValue( aPath, out abName ) )
            {
                LoadABAsync( abName, ( AssetBundle ab ) =>
                {
                    string aName = ResoucesName( aPath );
                    AssetBundleRequest assetBundleRequest = ab.LoadAssetAsync( aName );
                    assetBundleRequest.completed += ( AsyncOperation async ) =>
                    {
                        CheckShader( assetBundleRequest.asset );
                        onComplent?.Invoke( assetBundleRequest.asset );
                    };
                } );
            }
            else
            {
                ResourceRequest resourceRequest = Resources.LoadAsync( aPath );
                resourceRequest.completed += ( AsyncOperation async ) =>
                {
                    onComplent?.Invoke( resourceRequest.asset );
                };
            }
        }

        public void LoadAsync<T>( string aPath, Action<UnityEngine.Object> onComplent ) where T : UnityEngine.Object
        {
            string abName;
            if ( PathToAB.TryGetValue( aPath, out abName ) )
            {
                LoadABAsync( abName, ( AssetBundle ab ) =>
                {
                    if ( ab == null )
                    {
                        LDebug.LogError( abName + " is Null!!!" );
                        onComplent?.Invoke( null );
                    }
                    else
                    {

                        string aName = ResoucesName( aPath );
                        AssetBundleRequest assetBundleRequest = ab.LoadAssetAsync<T>( aName );
                        assetBundleRequest.completed += ( AsyncOperation async ) =>
                        {
                            CheckShader( assetBundleRequest.asset );
                            onComplent?.Invoke( assetBundleRequest.asset );
                        };
                    }

                } );
            }
            else
            {
                ResourceRequest resourceRequest = Resources.LoadAsync<T>( aPath );
                resourceRequest.completed += ( AsyncOperation async ) =>
                {
                    onComplent?.Invoke( resourceRequest.asset );
                };
            }
        }

        public void UnloadAsset()
        {
            AssetBundle.UnloadAllAssetBundles( false );
            Resources.UnloadUnusedAssets();
        }

        public AssetBundle LoadAB( string aABName )
        {
            ReBundle reAsset = null;
            if ( !m_BundlePool.TryGetValue( aABName, out reAsset ) )
            {
                reAsset = new ReBundle();
                reAsset.Path = aABName;
                reAsset.isEnd = false;
                m_BundlePool.Add( aABName, reAsset );
            }
            else if ( reAsset.assetBundle )
            {
                return reAsset.assetBundle;
            }
            reAsset.isEnd = false;
            AssetBundle assetBundle = null;
            assetBundle = AssetBundle.LoadFromFile( AbPath + aABName );
            reAsset.assetBundle = assetBundle;
            reAsset.isEnd = true;
            reAsset.onComplent?.Invoke( assetBundle );
            if ( assetBundle == null )
            {
                LDebug.LogError( AbPath + aABName + "加载错误" );
            }
            return assetBundle;
        }

        /// <summary>
        /// 强制卸载ab，用于场景包卸载
        /// </summary>
        /// <param name="aSceneName"></param>
        /// <param name="unloadAllLoadedObjects"></param>
        public void UnloadAb( string aBundleName, bool unloadAllLoadedObjects = false )
        {
            ReBundle reBundle = null;
            if ( m_BundlePool.TryGetValue( aBundleName, out reBundle ) )
            {
                reBundle.assetBundle.Unload( unloadAllLoadedObjects );
                m_BundlePool.Remove( aBundleName );
                reBundle.Reset();
                RecoveryReBundleList.Add( reBundle );
            }

        }

        /// <summary>
        /// 异步获取AssetBundle
        /// </summary>
        /// <param name="aABName"></param>
        /// <param name="onComplent"></param>
        public void LoadABAsync( string aABName, Action<AssetBundle> onComplent )
        {
            ReBundle reBundle = null;
            if ( !m_BundlePool.TryGetValue( aABName, out reBundle ) )
            {
                if ( RecoveryReBundleList.Count > 0 )
                {
                    reBundle = RecoveryReBundleList[ 0 ];
                }
                else
                {
                    reBundle = new ReBundle();
                }
                reBundle.Path = aABName;
                reBundle.isEnd = false;
                reBundle.onComplent += onComplent;
                m_BundlePool.Add( aABName, reBundle );
            }
            else if ( reBundle.isEnd && reBundle.assetBundle )
            {
                onComplent?.Invoke( reBundle.assetBundle );
                return;
            }
            else if ( !reBundle.isEnd )
            {
                reBundle.onComplent += onComplent;
                return;
            }
            else
            {

                reBundle.onComplent += onComplent;
            }
            reBundle.isEnd = false;
            AssetBundle assetBundle = null;
            if ( string.IsNullOrEmpty( m_Url ) || m_ResoucesTP == ResoucesASYLoadTP.File )
            {
                LoadFromFileABAsync( aABName, reBundle );
            }
            else if ( m_ResoucesTP == ResoucesASYLoadTP.Web )
            {
                LoadABAsyncWeb( aABName, reBundle );
            }
            else if ( m_ResoucesTP == ResoucesASYLoadTP.Memory )
            {
                LoadABAsyncMemory( aABName, reBundle );
            }
        }



        private string ResoucesName( string aPath )
        {
            int endIndex = aPath.LastIndexOf( '/' );
            if ( endIndex < 0 )
            {
                endIndex = aPath.LastIndexOf( '\\' );
            }
            string aNmae = null;
            endIndex = endIndex + 1;
            if ( endIndex > 0 && endIndex < aPath.Length )
            {
                aNmae = aPath.Substring( endIndex, aPath.Length - endIndex );
            }
            return aNmae;
        }
        private void LoadFromFileABAsync( string aABName, ReBundle reBundle )
        {
            AssetBundleCreateRequest assetRequest = AssetBundle.LoadFromFileAsync( AbPath + aABName );
            assetRequest.completed += ( AsyncOperation async ) =>
            {
                reBundle.assetBundle = assetRequest.assetBundle;
                reBundle.isEnd = true;
                reBundle.onComplent?.Invoke( reBundle.assetBundle );
            };
        }
        private void LoadABAsyncWeb( string aABName, ReBundle reBundle )
        {
            LitTool.MonoBehaviour.StartCoroutine( ILoadABAsyncWeb( aABName, reBundle ) );
        }
        private IEnumerator ILoadABAsyncWeb( string aABName, ReBundle reBundle )
        {
            UnityWebRequest assetRequest = UnityWebRequestAssetBundle.GetAssetBundle( m_Url + aABName );
            yield return assetRequest.SendWebRequest();
            AssetBundle ab = ( assetRequest.downloadHandler as DownloadHandlerAssetBundle ).assetBundle;
            reBundle.assetBundle = ab;
            reBundle.isEnd = true;
            reBundle.onComplent?.Invoke( reBundle.assetBundle );
        }
        private void LoadABAsyncMemory( string aABName, ReBundle reBundle )
        {
            AssetBundleCreateRequest assetRequest = AssetBundle.LoadFromMemoryAsync( File.ReadAllBytes( AbPath + aABName ) );
            assetRequest.completed += ( AsyncOperation async ) =>
            {
                reBundle.assetBundle = assetRequest.assetBundle;
                reBundle.isEnd = true;
                reBundle.onComplent?.Invoke( reBundle.assetBundle );
            };
        }
        private void CheckShader( UnityEngine.Object asset )
        {

#if UNITY_EDITOR
            if ( asset is GameObject )
            {
                GameObject obj = asset as GameObject;
                Renderer[] renders = obj.GetComponentsInChildren<Renderer>();
                foreach ( Renderer ren in renders )
                {
                    Material[] materials = ren.sharedMaterials;
                    foreach ( Material m in materials )
                    {
                        var shaderName = m.shader.name;

                        var newShader = Shader.Find( shaderName );
                        if ( newShader != null )
                        {
                            m.shader = newShader;
                        }
                    }

                }
            }
#endif
        }
    }
}
