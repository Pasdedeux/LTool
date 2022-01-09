using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitFramework.LitTool;
using System;
using UnityEngine.Networking;
using System.IO;
using LitFramework;
using LitFramework.Base;
using Assets.Scripts.Essential.Managers.RsCom;

public class RsLoadManager : Singleton<RsLoadManager>, IManager, IRsLoad
{
    private IRsLoad _rsLoad;
    
    //也允许外部使用指定加载器加载物体
    private RsLoadResource _resourceLoader;
    private RsLoadAB _abLoader;
    public void Install()
    {
        _abLoader = new RsLoadAB();
        _resourceLoader = new RsLoadResource();

        if (FrameworkConfig.Instance.resLoadType == ResLoadType.AssetBundle) _rsLoad = _abLoader;
        else _rsLoad = _resourceLoader;
    }

    public UnityEngine.Object Load( string aPath )
    {
        return _rsLoad.Load( aPath );
    }

    public UnityEngine.Object Load(string aPath, ResLoadType loadType )
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

    public T Load<T>( string aPath ) where T : UnityEngine.Object
    {
        return _rsLoad.Load<T>( aPath );
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

    public AssetBundle LoadAB( string aPath )
    {
        return _rsLoad.LoadAB( aPath );
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

    public void LoadAsync( string aPath, Action<UnityEngine.Object> onComplete )
    {
        _rsLoad.LoadAsync( aPath, onComplete );
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

    public void LoadAsync<T>( string aPath, Action<UnityEngine.Object> onComplete ) where T : UnityEngine.Object
    {
        _rsLoad.LoadAsync<T>( aPath, onComplete );
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

