#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.LitAttributes
* 项目描述 ：
* 类 名 称 ：SetProperty
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFramework.LitAttributes
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2021/4/10 21:03:20
* 更新时间 ：2021/4/10 21:03:20
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Liu Hanwen 2021. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace LitFrameworkEditor.LitAttributes
{
    /// <summary>
    /// 自定义私有化属性，在可序列之后于inspector上进行变量操作时，可以在修改自身时触发其封装器的Set属性
    /// </summary>
    [AttributeUsage( AttributeTargets.Field )]
    public class SetPropertyAttribute : PropertyAttribute
    {
        public string Name { get; private set; }
        public bool IsDirty { get; set; }

        public SetPropertyAttribute( string name )
        {
            this.Name = name;
        }
    }

    /// <summary>
    /// 编辑器扩展属性
    /// </summary>
     [CustomPropertyDrawer( typeof( SetPropertyAttribute ) )]
    public class SetPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField( position, property, label );

            //在必要时更新会刷新界面
            SetPropertyAttribute setProperty = attribute as SetPropertyAttribute;
            if ( EditorGUI.EndChangeCheck() )
            {
                //序列化的属性被修改时，实际变量值还是停留在原来的值上，知道ONGUI刷新。所以这里采用isDirty方式标记在其它所有OnGui事件全部完成后（比如Repaint）再更新对应变量
                setProperty.IsDirty = true;
            }
            else if ( setProperty.IsDirty )
            {
                //这里需要查找这个变量真正的父属性节点
                object parent = GetParentObjectOfProperty( property.propertyPath, property.serializedObject.targetObject );
                Type type = parent.GetType();
                PropertyInfo pi = type.GetProperty( setProperty.Name );
                if ( pi == null )
                {
                    LDebug.LogError( "Invalid property name: " + setProperty.Name + "\nCheck your [SetProperty] attribute" );
                }
                else
                {
                    pi.SetValue( parent, fieldInfo.GetValue( parent ), null );
                }
                setProperty.IsDirty = false;
            }
        }

        private object GetParentObjectOfProperty( string path, object obj )
        {
            string[] fields = path.Split( '.' );

            if ( fields.Length == 1 )
            {
                return obj;
            }

            FieldInfo fi = obj.GetType().GetField( fields[ 0 ], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
            obj = fi.GetValue( obj );

            return GetParentObjectOfProperty( string.Join( ".", fields, 1, fields.Length - 1 ), obj );
        }
    }

}
