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
using LitFramework.LitPool;
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

namespace Assets.Scripts.Essential.Managers
{
    class ReBundle
    {
        public string Path;
        public AssetBundle assetBundle;
        public bool isEnd = false;
        public System.Action<AssetBundle> onComplete;
        public void Reset()
        {
            onComplete = null;
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
        private string _abPath;
        private string _url = @"file:\\F:\xxx\UnityProject\Assets\StreamingAssets\";
        private ResoucesASYLoadTP _resoucesTP;
        private List<ReBundle> _recoveryReBundleList = new List<ReBundle>();
        private Dictionary<string, string> _pathToAB = new Dictionary<string, string>();
        private Dictionary<string, ReBundle> _bundlePool = new Dictionary<string, ReBundle>();

        public RsLoadAB() { }

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
                        _pathToAB.Add(content[0].Trim(), content[1].Trim());
                    }
                }
            });
            _resoucesTP = ResoucesASYLoadTP.File;
            _abPath = path;
            _abPath = _abPath + FrameworkConfig.Instance.ABFolderName + "/";
            _url = _url + FrameworkConfig.Instance.ABFolderName + "/";
        }

        public UnityEngine.Object Load(string aPath)
        {
            string abName;
            UnityEngine.Object rs = null;

            if (_pathToAB.TryGetValue(aPath, out abName))
            {
                AssetBundle ab = LoadAB(abName);
                if (ab)
                {
                    string aName = ResoucesName(aPath);
                    rs = ab.LoadAsset(aName);
                }

                CheckShader(rs);
            }
            else
            {
                rs = Resources.Load(aPath);
            }
            return rs;
        }

        public T Load<T>(string aPath) where T : UnityEngine.Object
        {
            string abName;
            T rs = default(T);

            if (_pathToAB.TryGetValue(aPath, out abName))
            {
                AssetBundle ab = LoadAB(abName);
                if (ab)
                {
                    string aName = ResoucesName(aPath);
                    rs = ab.LoadAsset<T>(aName);
                    CheckShader(rs);
                }
            }
            else
            {
                rs = Resources.Load<T>(aPath);
            }

            return rs;
        }

        public void LoadAsync(string aPath, Action<UnityEngine.Object> onComplete)
        {
            string abName;

            if (_pathToAB.TryGetValue(aPath, out abName))
            {
                LoadABAsync(abName, (AssetBundle ab) =>
                {
                    string aName = ResoucesName(aPath);
                    AssetBundleRequest assetBundleRequest = ab.LoadAssetAsync(aName);
                    assetBundleRequest.completed += (AsyncOperation async) =>
                    {
                        CheckShader(assetBundleRequest.asset);
                        onComplete?.Invoke(assetBundleRequest.asset);
                    };
                });
            }
            else
            {
                ResourceRequest resourceRequest = Resources.LoadAsync(aPath);
                resourceRequest.completed += (AsyncOperation async) =>
                {
                    onComplete?.Invoke(resourceRequest.asset);
                };
            }
        }

        public void LoadAsync<T>(string aPath, Action<UnityEngine.Object> onComplete) where T : UnityEngine.Object
        {
            string abName;

            if (_pathToAB.TryGetValue(aPath, out abName))
            {
                LoadABAsync(abName, (AssetBundle ab) =>
                {
                    if (ab == null)
                    {
                        LDebug.LogError(abName + " is Null!");
                        onComplete?.Invoke(null);
                    }
                    else
                    {
                        string aName = ResoucesName(aPath);
                        AssetBundleRequest assetBundleRequest = ab.LoadAssetAsync<T>(aName);
                        assetBundleRequest.completed += (AsyncOperation async) =>
                        {
                            CheckShader(assetBundleRequest.asset);
                            onComplete?.Invoke(assetBundleRequest.asset);
                        };
                    }
                });
            }
            else
            {
                ResourceRequest resourceRequest = Resources.LoadAsync<T>(aPath);
                resourceRequest.completed += (AsyncOperation async) =>
                {
                    onComplete?.Invoke(resourceRequest.asset);
                };
            }
        }

        public void UnloadAsset()
        {
            AssetBundle.UnloadAllAssetBundles(false);
            Resources.UnloadUnusedAssets();
        }

        public AssetBundle LoadAB(string aABName)
        {
            ReBundle reAsset = null;
            if (!_bundlePool.TryGetValue(aABName, out reAsset))
            {
                reAsset = new ReBundle();
                reAsset.Path = aABName;
                reAsset.isEnd = false;
                _bundlePool.Add(aABName, reAsset);
            }
            else if (reAsset.assetBundle)
            {
                return reAsset.assetBundle;
            }
            reAsset.isEnd = false;
            AssetBundle assetBundle = null;
            assetBundle = AssetBundle.LoadFromFile(_abPath + aABName);
            reAsset.assetBundle = assetBundle;
            reAsset.isEnd = true;
            reAsset.onComplete?.Invoke(assetBundle);
            if (assetBundle == null)
            {
                LDebug.LogError(_abPath + aABName + "加载错误");
            }
            return assetBundle;
        }

        /// <summary>
        /// 强制卸载ab，用于场景包卸载
        /// </summary>
        /// <param name="aSceneName"></param>
        /// <param name="unloadAllLoadedObjects"></param>
        public void UnloadAb(string aBundleName, bool unloadAllLoadedObjects = false)
        {
            ReBundle reBundle = null;
            if (_bundlePool.TryGetValue(aBundleName, out reBundle))
            {
                reBundle.assetBundle.Unload(unloadAllLoadedObjects);
                _bundlePool.Remove(aBundleName);
                reBundle.Reset();
                _recoveryReBundleList.Add(reBundle);
            }

        }

        /// <summary>
        /// 异步获取AssetBundle
        /// </summary>
        /// <param name="aABName"></param>
        /// <param name="onComplete"></param>
        public void LoadABAsync(string aABName, Action<AssetBundle> onComplete)
        {
            ReBundle reBundle = null;
            if (!_bundlePool.TryGetValue(aABName, out reBundle))
            {
                if (_recoveryReBundleList.Count > 0)
                {
                    reBundle = _recoveryReBundleList[0];
                }
                else
                {
                    reBundle = new ReBundle();
                }
                reBundle.Path = aABName;
                reBundle.isEnd = false;
                reBundle.onComplete += onComplete;
                _bundlePool.Add(aABName, reBundle);
            }
            else if (reBundle.isEnd && reBundle.assetBundle)
            {
                onComplete?.Invoke(reBundle.assetBundle);
                return;
            }
            else if (!reBundle.isEnd)
            {
                reBundle.onComplete += onComplete;
                return;
            }
            else
            {

                reBundle.onComplete += onComplete;
            }
            reBundle.isEnd = false;
            AssetBundle assetBundle = null;
            if (string.IsNullOrEmpty(_url) || _resoucesTP == ResoucesASYLoadTP.File)
            {
                LoadFromFileABAsync(aABName, reBundle);
            }
            else if (_resoucesTP == ResoucesASYLoadTP.Web)
            {
                LoadABAsyncWeb(aABName, reBundle);
            }
            else if (_resoucesTP == ResoucesASYLoadTP.Memory)
            {
                LoadABAsyncMemory(aABName, reBundle);
            }
        }



        private string ResoucesName(string aPath)
        {
            int endIndex = aPath.LastIndexOf('/');
            if (endIndex < 0)
            {
                endIndex = aPath.LastIndexOf('\\');
            }
            string aNmae = null;
            endIndex = endIndex + 1;
            if (endIndex > 0 && endIndex < aPath.Length)
            {
                aNmae = aPath.Substring(endIndex, aPath.Length - endIndex);
            }
            return aNmae;
        }
        private void LoadFromFileABAsync(string aABName, ReBundle reBundle)
        {
            AssetBundleCreateRequest assetRequest = AssetBundle.LoadFromFileAsync(_abPath + aABName);
            assetRequest.completed += (AsyncOperation async) =>
            {
                reBundle.assetBundle = assetRequest.assetBundle;
                reBundle.isEnd = true;
                reBundle.onComplete?.Invoke(reBundle.assetBundle);
            };
        }
        private void LoadABAsyncWeb(string aABName, ReBundle reBundle)
        {
            LitTool.MonoBehaviour.StartCoroutine(ILoadABAsyncWeb(aABName, reBundle));
        }
        private IEnumerator ILoadABAsyncWeb(string aABName, ReBundle reBundle)
        {
            UnityWebRequest assetRequest = UnityWebRequestAssetBundle.GetAssetBundle(_url + aABName);
            yield return assetRequest.SendWebRequest();
            AssetBundle ab = (assetRequest.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
            reBundle.assetBundle = ab;
            reBundle.isEnd = true;
            reBundle.onComplete?.Invoke(reBundle.assetBundle);
        }
        private void LoadABAsyncMemory(string aABName, ReBundle reBundle)
        {
            AssetBundleCreateRequest assetRequest = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(_abPath + aABName));
            assetRequest.completed += (AsyncOperation async) =>
            {
                reBundle.assetBundle = assetRequest.assetBundle;
                reBundle.isEnd = true;
                reBundle.onComplete?.Invoke(reBundle.assetBundle);
            };
        }
        private void CheckShader(UnityEngine.Object asset)
        {

#if UNITY_EDITOR
            if (asset is GameObject)
            {
                GameObject obj = asset as GameObject;
                Renderer[] renders = obj.GetComponentsInChildren<Renderer>();
                foreach (Renderer ren in renders)
                {
                    Material[] materials = ren.sharedMaterials;
                    foreach (Material m in materials)
                    {
                        var shaderName = m.shader.name;

                        var newShader = Shader.Find(shaderName);
                        if (newShader != null)
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
