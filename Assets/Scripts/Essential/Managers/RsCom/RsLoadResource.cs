/*======================================
* 项目名称 ：Assets.Scripts.Essential.Managers.RsCom
* 项目描述 ：
* 类 名 称 ：RsLoadResource
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Essential.Managers.RsCom
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/9/27 13:40:34
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
    public class RsLoadResource : IRsLoad
    {
        public RsLoadResource()
        {

        }

        public UnityEngine.Object Load( string aPath )
        {
            UnityEngine.Object rs = Resources.Load( aPath );
            return rs;
        }

        public T Load<T>( string aPath ) where T : UnityEngine.Object
        {
            T rs = Resources.Load<T>( aPath );
            return rs;
        }

        public void LoadAsync( string aPath, Action<UnityEngine.Object> onComplent )
        {
            ResourceRequest resourceRequest = Resources.LoadAsync( aPath );
            resourceRequest.completed += ( AsyncOperation async ) =>
            {
                onComplent?.Invoke( resourceRequest.asset );
            };
        }

        public void LoadAsync<T>( string aPath, Action<UnityEngine.Object> onComplent ) where T : UnityEngine.Object
        {
            ResourceRequest resourceRequest = Resources.LoadAsync<T>( aPath );
            resourceRequest.completed += ( AsyncOperation async ) =>
            {
                onComplent?.Invoke( resourceRequest.asset );
            };
        }

        public void UnloadAsset()
        {
            Resources.UnloadUnusedAssets();
        }


        //FAKE
        public AssetBundle LoadAB( string aPath )
        {
            throw new NotImplementedException();
        }

    }
}
