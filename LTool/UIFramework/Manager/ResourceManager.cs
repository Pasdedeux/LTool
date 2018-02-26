using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 加载resource文件夹资源，并缓存
/// </summary>
public class ResourceManager : Singleton<ResourceManager>
{
    //容器键值对集合
    private Hashtable _ht = null;

    private void Awake( )
    {
        _ht = new Hashtable();
    }

    /// <summary>
    /// 获取UI资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path">预制件路径</param>
    /// <param name="isCache">是否缓存</param>
    /// <returns></returns>
    private T LoadResource<T>( string path , bool isCache ) where T : UnityEngine.Object
    {
        if ( _ht.Contains( path ) )
            return _ht[ path ] as T;

        T tResource = Resources.Load<T>( path );
        if ( tResource == null )
            Debug.LogError( "找不到资源 path=" + path );
        else if ( isCache )
            _ht.Add( path , tResource );

        return tResource;
    }

    /// <summary>
    /// 获取UI资源
    /// </summary>
    /// <param name="path">预制件路径</param>
    /// <param name="isCache">是否缓存</param>
    /// <returns></returns>
    public GameObject LoadAssets( string path, bool isCache )
    {
        GameObject goObj = LoadResource<GameObject>( path , isCache );
        GameObject goObjClone = GameObject.Instantiate<GameObject>( goObj );
        if ( goObjClone == null ) Debug.LogError( "克隆资源失败;" );
        return goObjClone;
    }
}
