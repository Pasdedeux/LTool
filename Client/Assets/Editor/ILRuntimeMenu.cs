#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeMenu
{
   [MenuItem("ILRuntime/安装VS调试插件")]
    static void InstallDebugger()
    {
        Application.OpenURL( "https://github.com/Ourpalm/ILRuntime/releases" );
    }

    [MenuItem("ILRuntime/打开ILRuntime中文文档")]
    static void OpenDocumentation()
    {
        Application.OpenURL("https://ourpalm.github.io/ILRuntime/");
    }

    [MenuItem("ILRuntime/打开ILRuntime Github项目")]
    static void OpenGithub()
    {
        Application.OpenURL("https://github.com/Ourpalm/ILRuntime");
    }


    [MenuItem( "ILRuntime/拷贝到Unity工程" )]
    public static void CopyToUnity()
    {
        if ( EditorUtility.DisplayDialog( "提示", "是否拷贝到Unity工程", "确认", "取消" ) )
        {
            string uPath = Application.dataPath + "/../HotfixProject/HotFixLogic/";
            string hPath = Application.dataPath + "/Scripts/RuntimeScript/HotFixLogic/";

            Directory.Delete( hPath ,true );
            FolderCopy.CopyTo( uPath, hPath );
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog( "提示", "所有文件拷贝完毕", "OK" );
        }
        else
        {
            Debug.Log( "Cancel" );
        }

    }

    [MenuItem( "ILRuntime/拷贝到热更工程" )]
    public static void CopyToHotFix()
    {
        if ( EditorUtility.DisplayDialog( "提示", "是否拷贝到热更新工程", "确认", "取消" ) )
        {
            string uPath = Application.dataPath + "/../HotfixProject/HotFixLogic/";
            string hPath = Application.dataPath + "/Scripts/RuntimeScript/HotFixLogic/";

            Directory.Delete( uPath, true );
            FolderCopy.CopyTo( hPath, uPath );
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.Log( "Cancel" );
        }
    }

    [MenuItem( "ILRuntime/拷贝热更工程引用" )]
    public static void CopyDllToHotFix()
    {
        //拷贝工程dll文件
        string udpath = Application.dataPath + "/../Library/ScriptAssemblies/Assembly-CSharp.dll";
        string hdpath = Application.dataPath + "/../HotfixProject/Assembly-CSharp.dll";
        FileInfo file = new FileInfo( udpath );
        FileInfo dfile = new FileInfo( hdpath );
        if ( dfile.Exists )
            dfile.Delete();

        file.CopyTo( hdpath );

        string ufdpath = Application.dataPath + "/../Library/ScriptAssemblies/Assembly-CSharp-firstpass.dll";
        string hfdpath = Application.dataPath + "/../HotfixProject/Assembly-CSharp-firstpass.dll";
        FileInfo ffile = new FileInfo( ufdpath );
        FileInfo fdfile = new FileInfo( hfdpath );
        if ( fdfile.Exists )
            fdfile.Delete();

        ffile.CopyTo( hfdpath );

        Debug.Log( "所有DLL拷贝完毕" );
        AssetDatabase.Refresh();
    }
}
#endif


[CustomEditor( typeof( MonoBehaviourAdapter.Adaptor ), true )]
public class MonoBehaviourAdapterEditor : UnityEditor.UI.GraphicEditor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        MonoBehaviourAdapter.Adaptor clr = target as MonoBehaviourAdapter.Adaptor;
        var instance = clr.ILInstance;
        if ( instance != null )
        {
            EditorGUILayout.LabelField( "Script", clr.ILInstance.Type.FullName );
            foreach ( var i in instance.Type.FieldMapping )
            {
                //这里是取的所有字段，没有处理不是public的
                var name = i.Key;
                var type = instance.Type.FieldTypes[ i.Value ];

                var cType = type.TypeForCLR;
                if ( cType.IsPrimitive )//如果是基础类型
                {
                    if ( cType == typeof( float ) )
                    {
                        instance[ i.Value ] = EditorGUILayout.FloatField( name, ( float )instance[ i.Value ] );
                    }
                    else
                        throw new System.NotImplementedException();//剩下的大家自己补吧
                }
                else
                {
                    object obj = instance[ i.Value ];
                    if ( typeof( UnityEngine.Object ).IsAssignableFrom( cType ) )
                    {
                        //处理Unity类型
                        var res = EditorGUILayout.ObjectField( name, obj as UnityEngine.Object, cType, true );
                        instance[ i.Value ] = res;
                    }
                    else
                    {
                        //其他类型现在没法处理
                        if ( obj != null )
                            EditorGUILayout.LabelField( name, obj.ToString() );
                        else
                            EditorGUILayout.LabelField( name, "(null)" );
                    }
                }
            }
        }
    }
}