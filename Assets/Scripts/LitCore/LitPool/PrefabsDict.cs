/*======================================
* 项目名称 ：LitFramework.LitPool
* 项目描述 ：
* 类 名 称 ：PrefabsDict
* 类 描 述 ：
* 命名空间 ：LitFramework.LitPool
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/9/5 8:18:59
* 更新时间 ：2019/9/5 8:18:59
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2019/9/5 8:18:59
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PrefabsDict : IDictionary<string, Transform>
{
    #region Public Custom Memebers
    /// <summary>
    /// Returns a formatted string showing all the prefab names
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        // Get a string[] array of the keys for formatting with join()
        var keysArray = new string[ this._prefabs.Count ];
        this._prefabs.Keys.CopyTo( keysArray, 0 );

        // Return a comma-sperated list inside square brackets (Pythonesque)
        return string.Format( "[{0}]", System.String.Join( ", ", keysArray ) );
    }
    #endregion Public Custom Memebers


    #region Internal Dict Functionality
    // Internal Add and Remove...
    internal void _Add( string prefabName, Transform prefab )
    {
        this._prefabs.Add( prefabName, prefab );
    }

    internal bool _Remove( string prefabName )
    {
        return this._prefabs.Remove( prefabName );
    }

    internal void _Clear()
    {
        this._prefabs.Clear();
    }
    #endregion Internal Dict Functionality


    #region Dict Functionality
    // Internal (wrapped) dictionary
    private Dictionary<string, Transform> _prefabs = new Dictionary<string, Transform>();

    /// <summary>
    /// Get the number of SpawnPools in PoolManager
    /// </summary>
    public int Count { get { return this._prefabs.Count; } }

    /// <summary>
    /// Returns true if a prefab exists with the passed prefab name.
    /// </summary>
    /// <param name="prefabName">The name to look for</param>
    /// <returns>True if the prefab exists, otherwise, false.</returns>
    public bool ContainsKey( string prefabName )
    {
        return this._prefabs.ContainsKey( prefabName );
    }

    /// <summary>
    /// Used to get a prefab when the user is not sure if the prefabName is used.
    /// This is faster than checking Contains(prefabName) and then accessing the dict
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue( string prefabName, out Transform prefab )
    {
        return this._prefabs.TryGetValue( prefabName, out prefab );
    }

    #region Not Implimented

    public void Add( string key, Transform value )
    {
        throw new System.NotImplementedException( "Read-Only" );
    }

    public bool Remove( string prefabName )
    {
        throw new System.NotImplementedException( "Read-Only" );
    }

    public bool Contains( KeyValuePair<string, Transform> item )
    {
        string msg = "Use Contains(string prefabName) instead.";
        throw new System.NotImplementedException( msg );
    }

    public Transform this[ string key ]
    {
        get
        {
            Transform prefab;
            try
            {
                prefab = this._prefabs[ key ];
            }
            catch ( KeyNotFoundException )
            {
                string msg = string.Format( "A Prefab with the name '{0}' not found. " +
                                            "\nPrefabs={1}",
                                            key, this.ToString() );
                throw new KeyNotFoundException( msg );
            }

            return prefab;
        }
        set
        {
            throw new System.NotImplementedException( "Read-only." );
        }
    }

    public ICollection<string> Keys
    {
        get
        {
            return this._prefabs.Keys;
        }
    }


    public ICollection<Transform> Values
    {
        get
        {
            return this._prefabs.Values;
        }
    }


    #region ICollection<KeyValuePair<string, Transform>> Members
    private bool IsReadOnly { get { return true; } }
    bool ICollection<KeyValuePair<string, Transform>>.IsReadOnly { get { return true; } }

    public void Add( KeyValuePair<string, Transform> item )
    {
        throw new System.NotImplementedException( "Read-only" );
    }

    public void Clear() { throw new System.NotImplementedException(); }

    private void CopyTo( KeyValuePair<string, Transform>[] array, int arrayIndex )
    {
        string msg = "Cannot be copied";
        throw new System.NotImplementedException( msg );
    }

    void ICollection<KeyValuePair<string, Transform>>.CopyTo( KeyValuePair<string, Transform>[] array, int arrayIndex )
    {
        string msg = "Cannot be copied";
        throw new System.NotImplementedException( msg );
    }

    public bool Remove( KeyValuePair<string, Transform> item )
    {
        throw new System.NotImplementedException( "Read-only" );
    }
    #endregion ICollection<KeyValuePair<string, Transform>> Members
    #endregion Not Implimented




    #region IEnumerable<KeyValuePair<string, Transform>> Members
    public IEnumerator<KeyValuePair<string, Transform>> GetEnumerator()
    {
        return this._prefabs.GetEnumerator();
    }
    #endregion



    #region IEnumerable Members
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this._prefabs.GetEnumerator();
    }
    #endregion

    #endregion Dict Functionality

}

