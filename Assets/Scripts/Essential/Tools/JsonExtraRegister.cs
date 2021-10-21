/*======================================
* 项目名称 ：Assets.Scripts.Essential.Tools
* 项目描述 ：
* 类 名 称 ：JsonExtraRegister
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Essential.Tools
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/9/12 12:39:55
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Essential.Tools
{
    /// <summary>
    /// 此处用与扩展Vector2/Vector3等其它各类数据解析类型
    /// </summary>
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public static class JsonExtraRegister
    {
        static JsonExtraRegister()
        {
            JsonMapper.RegisterExporter<Vector2>( ( v, w ) =>
            {
                w.WriteObjectStart();
                w.WriteProperty( "x", v.x );
                w.WriteProperty( "y", v.y );
                w.WriteObjectEnd();
            } );

            JsonMapper.RegisterExporter<Vector3>( ( v, w ) =>
            {
                w.WriteObjectStart();
                w.WriteProperty( "x", v.x );
                w.WriteProperty( "y", v.y );
                w.WriteProperty( "z", v.z );
                w.WriteObjectEnd();
            } );
        }
    }
}
