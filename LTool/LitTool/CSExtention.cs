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
using UnityEngine.UI;

namespace LitFramework.LitTool
{
    /// <summary>
    /// 方法扩展类
    /// </summary>
    public static partial class CSExtention
    {
        #region string

        /// <summary>
        /// TODO 这个方法通用性优先，需要再思考下用处
        /// 对字符串内容进行批量替换
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args">需要替换成的字符串</param>
        /// <returns></returns>
        static string FormatWith( this string format, params object[] args )
        {
            if ( format == null || args == null )
                throw new ArgumentNullException( ( format == null ) ? "format" : "args" );

            var capacity = format.Length + args.Where( p => p != null ).Select( p => p.ToString() ).Sum( p => p.Length );
            var stringBuilder = new StringBuilder( capacity );
            stringBuilder.AppendFormat( format, args );
            return stringBuilder.ToString();
        }

        #endregion
        
        #region Camera
        /// <summary>
        /// 获取【透视相机】指定距离下相机视口四个角的坐标
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="distance">相对于相机的距离</param>
        /// <param name="corners"></param>
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
        /// <param name="corners"></param>
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

        #region Transform
        /// <summary>
        /// localscale=0，localPosition=0，localRotation=identity
        /// </summary>
        /// <param name="trans"></param>
        public static void IdentityTransform(this Transform trans)
        {
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
        }
        #endregion

        #region IList
        /// <summary>
        /// 数组内随机排列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> RandomSortList<T>( this List<T> list )
        {
            System.Random random = new System.Random();
            
            List<T> newList = new List<T>();
            foreach ( T item in list )
                newList.Insert( random.Next( newList.Count + 1 ), item );

            return newList;
        }
        #endregion

        #region UI
        private static Material grayMat;

        /// <summary>
        /// 创建置灰材质球
        /// </summary>
        /// <returns></returns>
        private static Material GetGrayMat()
        {
            if ( grayMat == null )
            {
                Shader shader = Resources.Load<Shader>( "Shader/UI/UIGrey" );
                if ( shader == null )
                {
                    LDebug.LogWarning( "未发现Shader Custom/UI-Gray" );
                    return null;
                }
                Material mat = new Material( shader );
                grayMat = mat;
            }

            return grayMat;
        }

        /// <summary>
        /// 图片置灰
        /// </summary>
        public static void SetUIGray( this Image img )
        {
            img.material = GetGrayMat();
            img.SetMaterialDirty();
        }

        /// <summary>
        /// 图片回复
        /// </summary>
        public static void SetUIGrayRecover( this Image img )
        {
            img.material = null;
        }
        #endregion


    }
}
