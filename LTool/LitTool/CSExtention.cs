#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.GameUtility
* 项目描述 ：
* 类 名 称 ：CSExtention
* 类 描 述 ：
* 所在的域 ：DEREK-SURFACE
* 命名空间 ：LitFramework.GameUtility
* 机器名称 ：DEREK-SURFACE 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：lc1027
* 创建时间 ：2018/12/27 11:42:00
* 更新时间 ：2018/12/27 11:42:00
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ lc1027 2018. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LitFramework.GameUtility
{
    /// <summary>
    /// 方法扩展类
    /// </summary>
    public static class CSExtention
    {
        #region 字符串

        /// <summary>
        /// 单字符串
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args">需要替换成的字符串</param>
        /// <returns></returns>
        public static string FormatWith( this string format, params object[] args )
        {
            if ( format == null || args == null )
                throw new ArgumentNullException( ( format == null ) ? "format" : "args" );

            var capacity = format.Length + args.Where( p => p != null ).Select( p => p.ToString() ).Sum( p => p.Length );
            var stringBuilder = new StringBuilder( capacity );
            stringBuilder.AppendFormat( format, args );
            return stringBuilder.ToString();
        }

        #endregion

        #region 协程
        /// <summary>
        /// 停止制定协程，并自动释放
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="func"></param>
        public static void StopCoroutineWith(this MonoBehaviour mono, ref Coroutine func )
        {
            mono.StopCoroutine( func );
            func = null;
        }
        #endregion
    }
}
