#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.LitTool
* 项目描述 ：
* 类 名 称 ：CSExtention
* 类 描 述 ：
* 所在的域 ：DEREK-SURFACE
* 命名空间 ：LitFramework.LitTool
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

namespace LitFramework.LitTool
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

        #region 相机相关
        /// <summary>
        /// 获取【透视相机】指定距离下相机视口四个角的坐标
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="distance">相对于相机的距离</param>
        /// <returns></returns>
        public static void GetCameraBounds( this Camera cam, float distance , ref Vector3[] corners )
        {
            //Vector3[] corners = new Vector3[ 4 ];

            float halfFOV = ( cam.fieldOfView * 0.5f ) * Mathf.Deg2Rad;
            float aspect = cam.aspect;

            float height = distance * Mathf.Tan( halfFOV );
            float width = height * aspect;

            Transform tx = cam.transform;

            // 左上角
            corners[ 0 ] = tx.position - ( tx.right * width );
            corners[ 0 ] += tx.up * height;
            corners[ 0 ] += tx.forward * distance;

            // 右上角
            corners[ 1 ] = tx.position + ( tx.right * width );
            corners[ 1 ] += tx.up * height;
            corners[ 1 ] += tx.forward * distance;

            // 左下角
            corners[ 2 ] = tx.position - ( tx.right * width );
            corners[ 2 ] -= tx.up * height;
            corners[ 2 ] += tx.forward * distance;

            // 右下角
            corners[ 3 ] = tx.position + ( tx.right * width );
            corners[ 3 ] -= tx.up * height;
            corners[ 3 ] += tx.forward * distance;

            //return corners;
        }

        /// <summary>
        /// 获取【正交相机】视口四个角的坐标
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
        public static void GetCameraBounds( this Camera cam , ref Vector3[] corners )
        {
            float aspect = cam.aspect;
            float halfHeight = cam.orthographicSize;
            float width = halfHeight * aspect;

            Transform tx = cam.transform;

            // 左上角
            corners[ 0 ] = tx.position - ( tx.right * width );
            corners[ 0 ] += tx.up * halfHeight;

            // 右上角
            corners[ 1 ] = tx.position + ( tx.right * width );
            corners[ 1 ] += tx.up * halfHeight;

            // 左下角
            corners[ 2 ] = tx.position - ( tx.right * width );
            corners[ 2 ] -= tx.up * halfHeight;

            // 右下角
            corners[ 3 ] = tx.position + ( tx.right * width );
            corners[ 3 ] -= tx.up * halfHeight;

            //return corners;
        }
        #endregion

        #region Unity
        public static void IdentityTransform(this Transform trans)
        {
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
        }
        #endregion
    }
}
