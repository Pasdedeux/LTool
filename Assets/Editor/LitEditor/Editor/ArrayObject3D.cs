#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFrameworkEditor.Editor
* 项目描述 ：
* 类 名 称 ：ArrayObject3D
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFrameworkEditor.Editor
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2021/3/29 23:03:25
* 更新时间 ：2021/3/29 23:03:25
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Liu Hanwen 2021. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace LitFrameworkEditor.Editor
{
    /// <summary>
    /// 编辑器模式下，对所选择的3D物体阵列分布
    /// </summary>
    public class ArrayObject3D: EditorWindow
    {
        private static ArrayObject3D _array3DSelf;
        private static Vector2 _arrayXYVector = Vector2.one;
        private static int _colNum = 5;

        [ MenuItem( "Tools/排列../打开平面排列窗口 %5" )]
        private static void Array3DObjectWindowOpen()
        {
            _array3DSelf = _array3DSelf ?? EditorWindow.GetWindow<ArrayObject3D>("阵列分布");
            _array3DSelf.Show();
        }

        [MenuItem( "Tools/排列../按当前间距设置直接排列 %#5" )]
        private static void Array3DObject()
        {
            var sections = Selection.gameObjects;
            List<GameObject> sorted = new List<GameObject>( sections );
            sorted.Sort( Comparison );
            int rows = Mathf.CeilToInt( sections.Length / ( float )_colNum );

            int index = 0, length = sections.Length;
            for ( int i = 0; i < rows; i++ )
            {
                for ( int j = 0; j < _colNum; j++ )
                {
                    var item = sorted[ index ];
                    item.transform.position = new Vector3( i * _arrayXYVector.x, 0, j * _arrayXYVector.y );
                    if ( length == ++index ) break;
                }
            }
        }

        private static int Comparison(GameObject a, GameObject b)
        {
            return a.transform.GetSiblingIndex() - b.transform.GetSiblingIndex();
        }


        void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            _arrayXYVector = EditorGUILayout.Vector2Field( "元素间隔距离（单位距离）", _arrayXYVector );
            _colNum = EditorGUILayout.IntField( "行数", _colNum );

            if ( GUILayout.Button("排列") )
                Array3DObject();

            EditorGUILayout.EndVertical();
        }
    }
}
