/*======================================
* 项目名称 ：Assets.Scripts.Essential.Tools
* 项目描述 ：
* 类 名 称 ：JsonExtention
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Essential.Tools
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/9/12 12:28:48
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

namespace Assets.Scripts.Essential.Tools
{
    /// <summary>
    /// 用于扩展JSON类型解析数值
    /// </summary>
    public static class JsonExtention
    {
        public static void WriteProperty( this JsonWriter w, string name, long value )
        {
            w.WritePropertyName( name );
            w.Write( value );
        }

        public static void WriteProperty( this JsonWriter w, string name, string value )
        {
            w.WritePropertyName( name );
            w.Write( value );
        }

        public static void WriteProperty( this JsonWriter w, string name, bool value )
        {
            w.WritePropertyName( name );
            w.Write( value );
        }

        public static void WriteProperty( this JsonWriter w, string name, double value )
        {
            w.WritePropertyName( name );
            w.Write( value );
        }

        public static void WriteProperty( this JsonWriter w, string name, int value )
        {
            w.WritePropertyName( name );
            w.Write( value );
        }

    }
}
