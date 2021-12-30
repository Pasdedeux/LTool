/*======================================
* 项目名称 ：Assets.Scripts.Essential.Managers.Interface
* 项目描述 ：
* 类 名 称 ：Interface1
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Essential.Managers.Interface
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/9/27 13:35:32
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Essential.Managers.RsCom
{
    interface IRsLoad
    {
        /// <summary>
        /// 同步获取Resources下资源
        /// </summary>
        /// <param name="aPath">Resources下相对路径不带后缀</param>
        /// <returns></returns>
        UnityEngine.Object Load( string aPath );
        /// <summary>
        /// 同步获取Resources下资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="aPath">Resources下相对路径不带后缀</param>
        /// <returns></returns>
        T Load<T>( string aPath ) where T : UnityEngine.Object;
        /// <summary>
        /// 异步获取Resources下资源
        /// </summary>
        /// <param name="aPath">Resources下相对路径带后缀</param>
        /// <returns></returns>
        void LoadAsync( string aPath, Action<UnityEngine.Object> onComplent );
        /// <summary>
        /// 异步获取Resources下资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="aPath">Resources下相对路径带后缀</param>
        /// <returns></returns>
        void LoadAsync<T>( string aPath, Action<UnityEngine.Object> onComplent ) where T : UnityEngine.Object;
        /// <summary>
        /// 回收资源
        /// </summary>
        /// <param name="aPath"></param>
        void UnloadAsset();
        /// <summary>
        /// 同步获取AssetBundle
        /// </summary>
        /// <param name="aABName"></param>
        /// <returns>AssetBundle</returns>
        AssetBundle LoadAB( string aPath );
    }
}
