﻿/****************************************************************
 * 作    者：Derek Liu
 * CLR 版本：4.0.30319.42000
 * 创建时间：2018/2/2 22:08:14
 * 当前版本：1.0.0.1
 * 
 * 描述说明：
 *
 * 修改历史：
 *
*****************************************************************
 * Copyright @ Derek 2018 All rights reserved
*****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityEngine
{
    public static class LMath
    {
        /// <summary>
        /// 返回 -180°~180°/ 0°~180° 向量夹角
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="fromZero">是否最小值为0°。默认为 False</param>
        /// <returns></returns>
        public static float VectorAngle( Vector2 from , Vector2 to , bool fromZero = false )
        {
            float angle;
            if ( !fromZero )
            {
                Vector3 cross = Vector3.Cross( from , to );
                angle = Vector2.Angle( from , to );
                return cross.z > 0 ? -angle : angle;
            }
            else
            {
                angle = Vector2.Angle( from , to );
                return angle;
            }
        }


        /// <summary>
        /// 屏幕坐标转3D坐标系的本地坐标
        /// </summary>
        /// <param name="screenPos"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static Vector3 ScreenToLocalPos( Vector3 screenPos, Transform parent )
        {
            Vector3 point = new Vector3( screenPos.x, screenPos.y, -Camera.main.transform.position.z );
            Vector3 worldPos = Camera.main.ScreenToWorldPoint( point );
            return parent.worldToLocalMatrix.MultiplyPoint( worldPos );
        }

        public static T Find<T>( Transform parent, string namePath ) where T : Component
        {
            Transform trans = parent.Find( namePath );
            if ( trans != null )
            {
                return trans.GetComponent<T>();
            }
            return null;
        }


        /// <summary>
        /// 得到权重的随机数
        /// </summary>
        /// <param name="prob"></param>
        /// <returns></returns>
        public static int RandomIndex( List<int> prob )
        {
            int result = 0;

            int sum = 0;
            for ( int i = 0; i < prob.Count; i++ )
            {
                sum += prob[ i ];

            }
            int n = sum * 1000;           //计算概率总和，放大1000倍
            int x = Random.Range( 0, n );       //随机生成0~概率总和的数字

            int pre = 0;  //区间下界
            int next = 0;//区间上界
            for ( int i = 0; i < prob.Count; i++ )
            {
                //pre += prob[i]; 
                if ( prob[ i ] == 0 ) continue;
                next = pre + prob[ i ];
                if ( x >= pre * 1000 && x < next * 1000 )//如果在该区间范围内，就返回结果退出循环
                {
                    result = i;
                    break;
                }
                pre = next;
            }
            return result;
        }

        /// <summary>
        /// 截图
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static Texture2D Snapshoot( Camera camera, int w, int h )
        {
            RenderTexture temp = RenderTexture.GetTemporary( w, h, 24 );
            RenderTexture lastTargetTexture = null;
            Texture2D texture2D = new Texture2D( w, h, TextureFormat.ARGB32, false );

            lastTargetTexture = camera.targetTexture;
            camera.targetTexture = temp;
            camera.Render();
            camera.targetTexture = lastTargetTexture;


            lastTargetTexture = RenderTexture.active;
            RenderTexture.active = temp;
            texture2D.ReadPixels( new Rect( 0, 0, w, h ), 0, 0 );
            texture2D.Apply();
            RenderTexture.active = lastTargetTexture;

            RenderTexture.ReleaseTemporary( temp );

            return texture2D;
        }

        
        #region  数学
        /// <summary>
        /// 二次贝塞尔曲线
        /// </summary>
        /// <param name="P0"></param>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 BezierCurve( Vector3 P0, Vector3 P1, Vector3 P2, float t )
        {
            Vector3 B = Vector3.zero;
            float t1 = ( 1 - t ) * ( 1 - t );
            float t2 = t * ( 1 - t );
            float t3 = t * t;
            B = P0 * t1 + 2 * t2 * P1 + t3 * P2;
            return B;
        }
        /// <summary>
        /// 点到线段最近的一个点位置和距离
        /// </summary>
        /// <param name="linePt1"> 线段起始点</param>
        /// <param name="linePt2"> 线段终点</param>
        /// <param name="point">任意一点</param>
        /// <param name="retPoint">out 相交点</param>
        /// <param name="d">out 点到线段的最近距离</param>
        /// <returns>是否有垂线与线段相交</returns>
        public static bool ClosestPointOnLine( Vector2 linePt1, Vector2 linePt2, Vector2 point, out Vector2 retPoint, out float d )
        {
            Matrix4x4 mat;
            Matrix4x4 mat_inv;
            Vector2 p2;
            mat = Matrix4x4.TRS( linePt1,
                                     Quaternion.Euler( 0, 0, CalcIncludedAngle( Vector2.right, linePt2 - linePt1 ) ),
                                     Vector3.one );
            mat_inv = mat.inverse;
            point = mat_inv.MultiplyPoint( point );
            p2 = mat_inv.MultiplyPoint( linePt2 );

            bool ret;
            ret = ( point.x > 0 ) != ( point.x > p2.x );

            if ( ret )
            {
                d = Mathf.Abs( point.y );
                retPoint = mat.MultiplyPoint( new Vector3( point.x, 0, 0 ) );
            }
            else
            {
                float d1, d2;
                d1 = point.magnitude;
                d2 = ( point - p2 ).magnitude;
                d = Mathf.Min( d1, d2 );
                retPoint = d1 < d2 ? linePt1 : linePt2;
            }
            return ret;
        }

        public static bool IsPointInPoly( Vector2 testPoint, Vector2[] poly )
        {
            bool ret = false;
            for ( int i = 0, j = poly.Length - 1; i < poly.Length; j = i++ )
            {
                if ( ( poly[ i ].y > testPoint.y ) != ( poly[ j ].y > testPoint.y ) &&
                    testPoint.x > poly[ i ].x + ( poly[ j ].x - poly[ i ].x ) * ( poly[ i ].y - testPoint.y ) / ( poly[ i ].y - poly[ j ].y ) )
                {
                    ret = !ret;
                }
            }
            return ret;
        }

        /// <summary>
        /// 判断点是否在直线上
        /// </summary>
        /// <param name="point">任意点</param>
        /// <param name="start">直线起点</param>
        /// <param name="end">直线终点</param>
        /// <returns>返回值越接近0就是表示点越靠近反之越远。当为0时，点完全在线上</returns>
        public static float IsPointOnLine( Vector2 p, Vector2 start, Vector2 end )
        {
            return ( start.x - p.x ) * ( end.y - p.y ) - ( end.x - p.x ) * ( start.y - p.y );
        }

        public static Vector2 CalcLineIntersection( Vector2 p1, Vector2 p2, //第一根直线
                                                   Vector2 p3, Vector2 p4 ) //第二根直线
        {
            Vector2 result = new Vector2();
            float left, right;

            left = ( p2.y - p1.y ) * ( p4.x - p3.x ) - ( p4.y - p3.y ) * ( p2.x - p1.x );
            right = ( p3.y - p1.y ) * ( p2.x - p1.x ) * ( p4.x - p3.x ) + ( p2.y - p1.y ) * ( p4.x - p3.x ) * p1.x - ( p4.y - p3.y ) * ( p2.x - p1.x ) * p3.x;
            result.x = right / left;

            left = ( p2.x - p1.x ) * ( p4.y - p3.y ) - ( p4.x - p3.x ) * ( p2.y - p1.y );
            right = ( p3.x - p1.x ) * ( p2.y - p1.y ) * ( p4.y - p3.y ) + p1.y * ( p2.x - p1.x ) * ( p4.y - p3.y ) - p3.y * ( p4.x - p3.x ) * ( p2.y - p1.y );
            result.y = right / left;
            return result;
        }

        protected class Vector2Node
        {
            public Vector2 point = Vector2.zero;
            public Vector2Node next = null;
        }

        public static void OptimizePolygon( Vector2[] inpoints, out Vector2[] outpoints, float optimization = 1.0f )
        {
            Vector2Node head, now;

            now = new Vector2Node();
            now.point = inpoints[ 0 ];
            head = now;

            for ( int i = 1; i < inpoints.Length; ++i )
            {
                now.next = new Vector2Node();
                now = now.next;
                now.point = inpoints[ i ];
            }

            now.next = head;

            now = head;
            do
            {
                if ( Mathf.Abs( IsPointOnLine( now.next.point, now.point, now.next.next.point ) ) < optimization )
                {
                    if ( now.next == head )
                    {
                        now.next = now.next.next;
                        break;
                    }
                    now.next = now.next.next;
                }
                else
                {
                    now = now.next;
                }

            } while ( head != now );

            List<Vector2> plist;
            plist = new List<Vector2>( 8 );
            head = now;
            do
            {
                plist.Add( now.point );
                now = now.next;
            } while ( head != now );

            outpoints = ( plist.Count > 0 ) ? plist.ToArray() : null;
        }

        // -180 ---- 180.
        public static float CalcIncludedAngle( Vector3 from, Vector3 to )
        {
            Vector2 v1, v2;
            from.y = from.z;
            to.y = to.z;

            v1 = from;
            v2 = to;

            return CalcIncludedAngle( v1, v2 );
        }

        public static float CalcIncludedAngle( Vector2 from, Vector2 to )
        {
            Vector3 v3;
            v3 = Vector3.Cross( from, to );
            return v3.z > 0 ? Vector2.Angle( from, to ) : -Vector2.Angle( from, to );
        }

        public static Bounds CalcGamaObjectBoundsInWorld( GameObject go )
        {
            Bounds bounds;
            MeshFilter mf;
            Vector3 min, max;
            Transform trans = go.transform;
            Vector3[] point;

            max = Vector3.one * float.MinValue;
            min = Vector3.one * float.MaxValue;
            bounds = new Bounds();
            point = new Vector3[ 8 ];

            if ( !go.activeInHierarchy )
                return bounds;

            mf = go.GetComponent<MeshFilter>();
            if ( mf && mf.sharedMesh )
            {
                mf.sharedMesh.RecalculateBounds();
                bounds = mf.sharedMesh.bounds;
            }
            else
            {
                SkinnedMeshRenderer smr;
                smr = go.GetComponent<SkinnedMeshRenderer>();
                if ( smr && smr.rootBone )
                {
                    trans = smr.rootBone;
                    bounds = smr.localBounds;
                }
            }
            Vector3 e = bounds.extents;
            point[ 0 ] = bounds.center + new Vector3( -e.x, e.y, e.z );
            point[ 1 ] = bounds.center + new Vector3( -e.x, e.y, -e.z );
            point[ 2 ] = bounds.center + new Vector3( e.x, e.y, e.z );
            point[ 3 ] = bounds.center + new Vector3( e.x, e.y, -e.z );

            point[ 4 ] = bounds.center + new Vector3( -e.x, -e.y, e.z );
            point[ 5 ] = bounds.center + new Vector3( -e.x, -e.y, -e.z );
            point[ 6 ] = bounds.center + new Vector3( e.x, -e.y, e.z );
            point[ 7 ] = bounds.center + new Vector3( e.x, -e.y, -e.z );

            for ( int i = 0; i < point.Length; ++i )
                point[ i ] = trans.localToWorldMatrix.MultiplyPoint( point[ i ] );

            for ( int i = 0; i < point.Length; ++i )
            {
                min.x = Mathf.Min( point[ i ].x, min.x );
                min.y = Mathf.Min( point[ i ].y, min.y );
                min.z = Mathf.Min( point[ i ].z, min.z );

                max.x = Mathf.Max( point[ i ].x, max.x );
                max.y = Mathf.Max( point[ i ].y, max.y );
                max.z = Mathf.Max( point[ i ].z, max.z );
            }
            bounds.SetMinMax( min, max );

            for ( int i = 0; i < go.transform.childCount; ++i )
            {
                bounds.Encapsulate( CalcGamaObjectBoundsInWorld( go.transform.GetChild( i ).gameObject ) );
            }
            return bounds;
        }

        public static float Clamp( float value, float min, float max, float bufferFactor, float damping = 2.0f )
        {
            float result = 0.0f;
            result = Mathf.Clamp( value, min, max );
            if ( Mathf.Abs( value - result ) > 0.001f )
            {
                float overDistance;
                float distance;
                float ratio;

                overDistance = Mathf.Abs( value - result );
                distance = Mathf.Abs( max - min );

                ratio = overDistance / distance;
                ratio = Mathf.Min( 1.0f, ratio / damping );
                ratio = Mathf.Sin( ratio * Mathf.PI / 2.0f );

                float buffer;
                buffer = Mathf.Abs( bufferFactor ) * Mathf.Abs( max - min );
                buffer *= ratio;

                result += ( value - result > 0 ) ? buffer : -buffer;
            }
            return result;
        }

        #endregion


    }
}
