/*======================================
* 项目名称 ：Assets.Scripts.Controller
* 项目描述 ：
* 类 名 称 ：URPCamManager
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Controller
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2022/2/18 15:08:10
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2022. All rights reserved.
*******************************************************************
======================================*/

using UnityEngine;
using System;
using LitFramework;
using LitFramework.Base;
using Assets.Scripts.Essential.Managers;
using LitFramework.LitPool;

public class RsLoadManager : Singleton<RsLoadManager>, IManager, IRsLoad
{
    //是否优先使用对象池加载
    public bool UseSpawnPool = true;

    private IRsLoad _rsLoad;
    //也允许外部使用指定加载器加载物体
    private RsLoadAB _abLoader;
    private RsLoadResource _resourceLoader;
    //对象池引用
    private SpawnManager _spawnManager;
    public void Install()
    {
        _abLoader = new RsLoadAB();
        _resourceLoader = new RsLoadResource();
        _spawnManager = SpawnManager.Instance;

        if (FrameworkConfig.Instance.resLoadType == ResLoadType.AssetBundle) _rsLoad = _abLoader;
        else _rsLoad = _resourceLoader;
    }

    /// <summary>
    /// 获取指定路径/名称下的GameObject
    /// </summary>
    /// <param name="aPath"></param>
    /// <returns></returns>
    public GameObject Get(string aPath)
    {
        var alter = CheckAlternaltiveName(aPath);
        if (UseSpawnPool && _spawnManager.IsSpawned(alter)) return _spawnManager.SpwanObject(alter);

        var loadedPreafab = Load<GameObject>(aPath);
        if (loadedPreafab != null) return GameObject.Instantiate(loadedPreafab);

        return null;
    }

    ///// <summary>
    ///// 获取指定路径/名称下的指定类型对象
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="aPath"></param>
    ///// <returns></returns>
    //public T Get<T>(string aPath) where T : UnityEngine.Object
    //{
    //    var alter = CheckAlternaltiveName(aPath);
    //    if (UseSpawnPool && _spawnManager.IsSpawned(alter)) return _spawnManager.SpwanObject(CheckAlternaltiveName(alter)) as T;

    //    var loadedPreafab = Load<GameObject>(aPath) as T;
    //    if (loadedPreafab != null) return GameObject.Instantiate<T>(loadedPreafab);

    //    return null;
    //}

    /// <summary>
    /// 获取指定预制（非实例）
    /// </summary>
    /// <param name="aPath"></param>
    /// <returns></returns>
    public UnityEngine.Object Load(string aPath)
    {
        return _rsLoad.Load(aPath);
    }

    /// <summary>
    /// 获取指定预制（非实例）
    /// </summary>
    /// <param name="aPath"></param>
    /// <param name="loadType"></param>
    /// <returns></returns>
    public UnityEngine.Object Load(string aPath, ResLoadType loadType)
    {
        switch (loadType)
        {
            case ResLoadType.AssetBundle:
                return _abLoader.Load(aPath);
            case ResLoadType.Resource:
                return _resourceLoader.Load(aPath);
        }
        return null;
    }

    /// <summary>
    /// 获取指定预制（非实例）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="aPath"></param>
    /// <returns></returns>
    public T Load<T>(string aPath) where T : UnityEngine.Object
    {
        return _rsLoad.Load<T>(aPath);
    }

    /// <summary>
    /// 获取指定预制（非实例）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="aPath"></param>
    /// <param name="loadType"></param>
    /// <returns></returns>
    public T Load<T>(string aPath, ResLoadType loadType) where T : UnityEngine.Object
    {
        switch (loadType)
        {
            case ResLoadType.AssetBundle:
                return _abLoader.Load<T>(aPath);
            case ResLoadType.Resource:
                return _resourceLoader.Load<T>(aPath);
        }
        return null;
    }

    /// <summary>
    /// 指定读取AB包
    /// </summary>
    /// <param name="aPath">AB包路径</param>
    /// <returns></returns>
    public AssetBundle LoadAB(string aPath)
    {
        return _rsLoad.LoadAB(aPath);
    }

    /// <summary>
    /// 指定读取AB包特定内容
    /// </summary>
    /// <param name="aPath"></param>
    /// <param name="loadType"></param>
    /// <returns></returns>
    public AssetBundle LoadAB(string aPath, ResLoadType loadType)
    {
        switch (loadType)
        {
            case ResLoadType.AssetBundle:
                return _abLoader.LoadAB(aPath);
            case ResLoadType.Resource:
                return _resourceLoader.LoadAB(aPath);
        }
        return null;
    }

    /// <summary>
    /// 获取指定预制（非实例）
    /// </summary>
    /// <param name="aPath"></param>
    /// <param name="onComplete"></param>
    public void LoadAsync(string aPath, Action<UnityEngine.Object> onComplete)
    {
        _rsLoad.LoadAsync(aPath, onComplete);
    }

    /// <summary>
    /// 获取指定预制（非实例）
    /// </summary>
    /// <param name="aPath"></param>
    /// <param name="loadType"></param>
    /// <param name="onComplete"></param>
    public void LoadAsync(string aPath, ResLoadType loadType, Action<UnityEngine.Object> onComplete)
    {
        switch (loadType)
        {
            case ResLoadType.AssetBundle:
                _abLoader.LoadAsync(aPath, onComplete);
                break;
            case ResLoadType.Resource:
                _resourceLoader.LoadAsync(aPath, onComplete);
                break;
        }
    }

    /// <summary>
    /// 获取指定预制（非实例）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="aPath"></param>
    /// <param name="onComplete"></param>
    public void LoadAsync<T>(string aPath, Action<T> onComplete) where T : UnityEngine.Object
    {
        _rsLoad.LoadAsync<T>(aPath, onComplete);
    }

    /// <summary>
    /// 获取指定预制（非实例）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="aPath"></param>
    /// <param name="loadType"></param>
    /// <param name="onComplete"></param>
    public void LoadAsync<T>(string aPath, ResLoadType loadType, Action<T> onComplete) where T : UnityEngine.Object
    {
        switch (loadType)
        {
            case ResLoadType.AssetBundle:
                _abLoader.LoadAsync<T>(aPath, onComplete);
                break;
            case ResLoadType.Resource:
                _resourceLoader.LoadAsync<T>(aPath, onComplete);
                break;
        }
    }

    public void UnloadAsset()
    {
        _rsLoad.UnloadAsset();
    }

    public void Uninstall()
    {
        _spawnManager = null;
        _rsLoad = null;
    }

    public void AfterInit()
    {
        _rsLoad.AfterInit();
    }

    /// <summary>
    /// 取出预制件基类
    /// </summary>
    /// <param name="aPath"></param>
    /// <returns></returns>
    private string CheckAlternaltiveName( string aPath )
    {
        if (aPath.IndexOf("/") != -1) return aPath.Substring(aPath.LastIndexOf("/") + 1);
        return aPath;
    }
}


/// <summary>
/// 资源加载其扩展类
/// 
/// 用于基于资源加载器的对象释放过程
/// </summary>
public static class RsLoadManagerSystem
{
    public static void Destroy(this GameObject self)
    {
        if (self == null) throw new Exception("被销毁对象为空");
        if (SpawnManager.Instance.IsSpawned(self.transform)) { SpawnManager.Instance.DespawnObject(self.transform); return; }
        GameObject.Destroy(self);
    }

    public static void Destroy(this Transform self)
    {
        if (self == null) throw new Exception("被销毁对象为空");
        if (SpawnManager.Instance.IsSpawned(self)) { SpawnManager.Instance.DespawnObject(self); return; }
        GameObject.Destroy(self.gameObject);
    }
}


