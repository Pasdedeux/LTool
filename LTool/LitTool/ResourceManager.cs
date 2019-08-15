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
*Fix Note:
*修改时间：2019/7/5 11:33:15
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using LitFramework;
using LitFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>, IManager
{
    private Dictionary<string, Sprite[]> _atlasDict;
    public void Install()
    {
        _atlasDict = new Dictionary<string, Sprite[]>();
    }

    public void Uninstall()
    {
        _atlasDict.Clear();
        Resources.UnloadUnusedAssets();
    }

    public Sprite LoadSprite( string spriteName, string atlasPath = null )
    {
        //常驻内存
        Sprite sprite = Resources.Load<Sprite>( spriteName );
        if ( sprite == null && !string.IsNullOrEmpty( atlasPath ) )
        {
            if ( !_atlasDict.ContainsKey( atlasPath ) )
            {
                Sprite[] atlasSprites = Resources.LoadAll<Sprite>( atlasPath );
                _atlasDict.Add( atlasPath, atlasSprites );
            }

            var sprites = _atlasDict[ atlasPath ];
            var length = _atlasDict[ atlasPath ].Length;
            for ( int i = 0; i < length; i++ )
            {
                if ( sprites[ i ].name.Equals( string.Concat( new string[] { atlasPath, "_", spriteName } ) ) ) return sprite = sprites[ i ];
            }
        }
        return sprite;
    }
}
