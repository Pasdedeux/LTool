﻿/*======================================
* 项目名称 ：LitFrameworkEditor.Extention_Editor.MenuExtention
* 项目描述 ：
* 类 名 称 ：UGUIRaycastCheck
* 类 描 述 ：
* 命名空间 ：LitFrameworkEditor.Extention_Editor.MenuExtention
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/3/29 11:09:45
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LitFrameworkEditor.Extention_Editor.MenuExtention
{
    /// <summary>
    /// RayCast线框显示
    /// </summary>
    public class UGUIRaycastCheck: EditorWindow
    {
        private MaskableGraphic[] graphics;
        private bool hideUnchecked = false;
        private bool showBorders = true;
        private Color borderColor = Color.blue;
        private Vector2 scrollPosition = Vector2.zero;

        private static UGUIRaycastCheck _selfTarget = null;

        [MenuItem( "Tools/RaycastTarget检测器 &1" )]
        private static void Open()
        {
            _selfTarget = _selfTarget ?? EditorWindow.GetWindow<UGUIRaycastCheck>( "RaycastTarget检测器" );
            _selfTarget.Show();
        }

        void OnEnable()
        {
            _selfTarget = this;
        }

        void OnDisable()
        {
            _selfTarget = null;
        }

        void OnGUI()
        {
            using ( EditorGUILayout.HorizontalScope horizontalScope = new EditorGUILayout.HorizontalScope() )
            {
                showBorders = EditorGUILayout.Toggle( "Show Gizmos", showBorders, GUILayout.Width( 200.0f ) );
                borderColor = EditorGUILayout.ColorField( borderColor );
            }
            hideUnchecked = EditorGUILayout.Toggle( "隐藏未勾选对象", hideUnchecked );

            //------------分割线-----------//
            GUILayout.Space( 12.0f );
            Rect rect = GUILayoutUtility.GetLastRect();
            GUI.color = new Color( 0.0f, 0.0f, 0.0f, 0.25f );
            GUI.DrawTexture( new Rect( 0.0f, rect.yMin + 6.0f, Screen.width, 4.0f ), EditorGUIUtility.whiteTexture );
            GUI.DrawTexture( new Rect( 0.0f, rect.yMin + 6.0f, Screen.width, 1.0f ), EditorGUIUtility.whiteTexture );
            GUI.DrawTexture( new Rect( 0.0f, rect.yMin + 9.0f, Screen.width, 1.0f ), EditorGUIUtility.whiteTexture );
            GUI.color = Color.white;

            //获取所有拥有MaskableGraphic的对象
            graphics = GameObject.FindObjectsOfType<MaskableGraphic>();
            using ( GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope( scrollPosition ) )
            {
                scrollPosition = scrollViewScope.scrollPosition;
                for ( int i = 0; i < graphics.Length; i++ )
                {
                    MaskableGraphic graphic = graphics[ i ];
                    if ( hideUnchecked == false || graphic.raycastTarget == true )
                    {
                        DrawElement( graphic );
                    }
                }
            }
            //foreach ( var item in graphics )
            //{
            //    EditorUtility.SetDirty( item );
            //}
            Repaint();
        }
        /// <summary>
        /// 绘制拥有MaskableGraphic的对象项
        /// </summary>
        /// <param name="graphic"></param>
        private void DrawElement( MaskableGraphic graphic )
        {
            using ( EditorGUILayout.HorizontalScope horizontalScope = new EditorGUILayout.HorizontalScope() )
            {
                graphic.raycastTarget = EditorGUILayout.Toggle( graphic.raycastTarget, GUILayout.Width( 20 ) );
                EditorGUI.BeginDisabledGroup( true );
                EditorGUILayout.ObjectField( graphic, typeof( MaskableGraphic ), true );
                EditorGUI.EndDisabledGroup();
            }
        }
        /// <summary>
        /// 根据选中与否，绘制交叉选中框
        /// </summary>
        /// <param name="source"></param>
        /// <param name="gizmoType"></param>
        [DrawGizmo( GizmoType.Selected | GizmoType.NonSelected )]
        private static void DrawGizmos( MaskableGraphic source, GizmoType gizmoType )
        {
            if ( _selfTarget != null && _selfTarget.showBorders == true && source.raycastTarget == true )
            {
                Vector3[] corners = new Vector3[ 4 ];
                source.rectTransform.GetWorldCorners( corners );
                Gizmos.color = _selfTarget.borderColor;
                for ( int i = 0; i < 4; i++ )
                {
                    Gizmos.DrawLine( corners[ i ], corners[ ( i + 1 ) % 4 ] );
                }
                if ( Selection.activeGameObject == source.gameObject )
                {
                    Gizmos.DrawLine( corners[ 0 ], corners[ 2 ] );
                    Gizmos.DrawLine( corners[ 1 ], corners[ 3 ] );
                }
            }
            SceneView.RepaintAll();
        }
    }
}
