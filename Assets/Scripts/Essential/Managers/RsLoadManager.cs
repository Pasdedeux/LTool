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

public class RsLoadManager : Singleton<RsLoadManager>, IManager, IRsLoad
{
    //是否优先使用对象池加载
    public bool UseSpawnPool = true;

    private IRsLoad _rsLoad;
    //也允许外部使用指定加载器加载物体
    private RsLoadAB _abLoader;
    private RsLoadResource _resourceLoader;
    public void Install()
    {
        _abLoader = new RsLoadAB();
        _resourceLoader = new RsLoadResource();

        if (FrameworkConfig.Instance.resLoadType == ResLoadType.AssetBundle) _rsLoad = _abLoader;
        else _rsLoad = _resourceLoader;
    }

    public UnityEngine.Object Load(string aPath)
    {
        return _rsLoad.Load(aPath);
    }

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

    public T Load<T>(string aPath) where T : UnityEngine.Object
    {
        return _rsLoad.Load<T>(aPath);
    }

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

    public AssetBundle LoadAB(string aPath)
    {
        return _rsLoad.LoadAB(aPath);
    }

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

    public void LoadAsync(string aPath, Action<UnityEngine.Object> onComplete)
    {
        _rsLoad.LoadAsync(aPath, onComplete);
    }

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

    public void LoadAsync<T>(string aPath, Action<UnityEngine.Object> onComplete) where T : UnityEngine.Object
    {
        _rsLoad.LoadAsync<T>(aPath, onComplete);
    }

    public void LoadAsync<T>(string aPath, ResLoadType loadType, Action<UnityEngine.Object> onComplete) where T : UnityEngine.Object
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
        _rsLoad = null;
    }

    public void AfterInit()
    {
        _rsLoad.AfterInit();
    }
}

