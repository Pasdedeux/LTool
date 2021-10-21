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
    public void Install()
    {
        if ( FrameworkConfig.Instance.resLoadType == ResLoadType.AssetBundle ) _rsLoad = new RsLoadAB();
        else _rsLoad = new RsLoadResource();
    }

    public UnityEngine.Object Load( string aPath )
    {
        return _rsLoad.Load( aPath );
    }

    public T Load<T>( string aPath ) where T : UnityEngine.Object
    {
        return _rsLoad.Load<T>( aPath );
    }

    public AssetBundle LoadAB( string aPath )
    {
        return _rsLoad.LoadAB( aPath );
    }

    public void LoadAsync( string aPath, Action<UnityEngine.Object> onComplent )
    {
        _rsLoad.LoadAsync( aPath, onComplent );
    }

    public void LoadAsync<T>( string aPath, Action<UnityEngine.Object> onComplent ) where T : UnityEngine.Object
    {
        _rsLoad.LoadAsync<T>( aPath, onComplent );
    }

    public void RecoveryAsset()
    {
        _rsLoad.RecoveryAsset();
    }

    public void Uninstall()
    {
        _rsLoad = null;
    }
}

