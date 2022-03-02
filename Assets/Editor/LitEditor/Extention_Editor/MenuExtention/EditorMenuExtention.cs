#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.EditorTool
* 项目描述 ：
* 类 名 称 ：EditorTools
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFramework.EditorTool
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/5/9 22:49:28
* 更新时间 ：2018/5/9 22:49:28
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Liu Hanwen 2018. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace LitFrameworkEditor.EditorExtended
{
    using UnityEngine;
    using UnityEngine.Networking;
    using System.IO;
    using System.Data;
#if UNITY_EDITOR
    using UnityEditor;
    using ExcelDataReader;
    using UnityEditor.Experimental.SceneManagement;
    using UnityEditor.SceneManagement;
    using LitFramework;
    using LitFramework.LitTool;
    using System.Linq;
    using Litframework.ExcelTool;
#endif

    /// <summary>
    /// 包含Tools菜单下一系列工具包文件
    /// </summary>
    public class EditorMenuExtention
    {
        /// <summary>
        /// 将XLSX文件夹的excel文档转换为csv文件
        /// </summary>
#if UNITY_EDITOR
        //[MenuItem( "Tools/配置文件->CSV", priority = 20 )]
#endif
        private static void XlsxToCSV()
        {
            LDebug.Log("配置文件==>CSV  开始!", LogColor.yellow);

            ExcelExport.ProjectPath = Application.dataPath;
            ExcelExport.Xlsx_2_CSV(FrameworkConfig.Instance.configs_suffix);

            LDebug.Log("配置文件==>CSV  成功!", LogColor.green);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        /// <summary>
        /// 将SA文件夹下CSV配置文件由csv格式转换为CS代码文件
        /// </summary>
#if UNITY_EDITOR
        [MenuItem( "Tools/配置文件->CSV+代码", priority = 21 )]
#endif
        public static void CsvToCs()
        {
            LDebug.Log("配置文件==>代码  开始!", LogColor.yellow);

            ExcelExport.ProjectPath = Application.dataPath;
            ExcelExport.Xlsx_2_CsvCs(FrameworkConfig.Instance.UseHotFixMode, FrameworkConfig.Instance.configs_suffix);

            LDebug.Log("配置文件==>代码  成功!", LogColor.green);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }


#if UNITY_EDITOR
        [MenuItem( "Tools/开启PersistentData文件夹", priority = 2 )]
        private static void OpenPersistentDataPath()
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.Start( Application.persistentDataPath );
            p.Close();
        }

        [MenuItem( "Tools/开启TemporaryCache文件夹", priority = 3 )]
        private static void OpenTemporaryCachePath()
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.Start( Application.temporaryCachePath );
            p.Close();
        }

        [MenuItem( "Tools/开启StreamingAssets文件夹", priority = 1 )]
        private static void OpenSteamingAssetsPath()
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.Start( Application.streamingAssetsPath );
            p.Close();
        }

        [MenuItem( "Tools/清档(PlayerPrefs)", priority = 40 )]
        private static void RemoveAllKey()
        {
            PlayerPrefs.DeleteAll();
        }

        /// <summary>
        /// 删除Assets空白目录下文件
        /// </summary>
        [MenuItem( "Tools/删除空白目录", priority = 41 )]
        private static void CleanEmptyDirectories()
        {
            var di = new DirectoryInfo( "Assets/" );
            var dis = new List<DirectoryInfo>();

            DoRemoveEmptyDirectory( di, dis );

            if ( dis.Count == 0 )
            {
                EditorUtility.DisplayDialog( "Remove Empty Directories", "No Empty Directory", "OK" );
                return;
            }

            var sb = new System.Text.StringBuilder();
            for ( int i = 0; i < dis.Count; ++i )
            {
                int index = i + 1;
                sb.AppendLine( index.ToString() + " " + dis[ i ].FullName );
            }

            if ( EditorUtility.DisplayDialog( "Remove Empty Directories", sb.ToString(), "OK", "Cancel" ) )
            {
                foreach ( var target in dis )
                {
                    if ( File.Exists( target.FullName + ".meta" ) )
                        File.Delete( target.FullName + ".meta" );

                    target.Delete( true );
                }
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// 移除目录下空白文件夹
        /// </summary>
        /// <param name="target">指定检测目录</param>
        /// <param name="dis">传入用于存储已删除目录记录的列表</param>
        /// <returns></returns>
        private static bool DoRemoveEmptyDirectory( DirectoryInfo target, List<DirectoryInfo> dis )
        {
            bool hasDirOrFile = false;
            foreach ( var di in target.GetDirectories() )
            {
                bool result = DoRemoveEmptyDirectory( di, dis );
                if ( result ) hasDirOrFile = true;
            }

            foreach ( var fi in target.GetFiles() )
            {
                if ( !fi.Name.StartsWith( "." ) && !fi.FullName.EndsWith( ".meta" ) )
                {
                    hasDirOrFile = true;
                }
            }

            if ( hasDirOrFile == false )
            {
                if ( dis.Contains( target ) == false )
                    dis.Add( target );
            }

            return hasDirOrFile;
        }

        /// <summary>
        /// 隐藏/显示当前选中的物体，同时将当前场景和预制件设为Dirty
        /// </summary>
        [MenuItem( "Tools/显隐选中的物体 &q" )]
        public static void HideChoosedObject()
        {
            var choosed = Selection.gameObjects;
            foreach ( var item in choosed )
            {
                item.SetActive( !item.activeSelf );
            }
            //预模式下保存制件
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            if ( Application.isPlaying ) return;
            if ( prefabStage != null )
            {
                EditorSceneManager.MarkSceneDirty( prefabStage.scene );
            }
            //场景保存
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty( UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene() );
        }

        /// <summary>
        /// 锁定当前开启的Inspector窗口面板
        /// </summary>
        [MenuItem( "Tools/锁定面板 &w" )]
        public static void LockedChoosedComponet()
        {
            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }

        [MenuItem( "Tools/Clear Shader Cache" )]
        static public void ClearShaderCache_Command()
        {
            var shaderCachePath = Path.Combine( Application.dataPath, "../Library/ShaderCache" );
            Directory.Delete( shaderCachePath, true );
        }
#endif



        /// <summary>
        /// 查找指定文件夹下指定后缀名的文件
        /// </summary>
        /// <param name="directory">文件夹</param>
        /// <param name="pattern">后缀名</param>
        /// <returns>文件路径</returns>
        private static void GetFiles( DirectoryInfo directory, string pattern, StreamWriter listwriter )
        {
            if ( directory.Exists || pattern.Trim() != string.Empty )
            {
                try
                {
                    foreach ( FileInfo info in directory.GetFiles( "*." + pattern ) )
                    {
                        //TODO 未能提供检查是否重名
                        listwriter.WriteLine( info.Name );
                    }
                }
                catch ( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
                foreach ( DirectoryInfo info in directory.GetDirectories() )//获取文件夹下的子文件夹
                {
                    GetFiles( info, pattern, listwriter );//递归调用该函数，获取子文件夹下的文件
                }
            }
        }

        /// <summary>
        /// 查找指定文件夹下指定后缀名的文件
        /// </summary>
        /// <param name="directory">文件夹</param>
        /// <param name="pattern">后缀名</param>
        /// <returns>文件路径</returns>
        private static void GetFiles( DirectoryInfo directory, string pattern, List<ABVersion> listwriter )
        {
            if ( directory.Exists || pattern.Trim() != string.Empty )
            {
                try
                {
                    foreach (FileInfo info in directory.GetFiles("*." + pattern))
                    {
                        string realName = info.FullName.Replace("\\", "/");
                        realName = realName.Substring(realName.LastIndexOf("StreamingAssets") + "StreamingAssets".Length + 1);
                        if (listwriter.Any(e => e.AbName == realName))
                        {
                            LDebug.LogWarning($">>配置档 {realName} 重复录入。已忽略 ");
                            continue;
                        }
                        listwriter.Add(new ABVersion { AbName = realName, MD5 = LitFramework.Crypto.Crypto.md5.GetFileHash(info.FullName), Version = 0 });
                    }
                }
                catch ( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
                foreach ( DirectoryInfo info in directory.GetDirectories() )//获取文件夹下的子文件夹
                {
                    GetFiles( info, pattern, listwriter );//递归调用该函数，获取子文件夹下的文件
                }
            }
        }
    }


    /// <summary>
    /// 查找引用预制件窗口
    /// </summary>
    public class SearchRefrenceEditorWindow : EditorWindow
    {
        [MenuItem( "Tools/查找引用的预制件or材质球 #w" )]
        static void SearchRefrence()
        {
            SearchRefrenceEditorWindow window = ( SearchRefrenceEditorWindow )EditorWindow.GetWindow( typeof( SearchRefrenceEditorWindow ), false, "Searching", true );
            window.Show();
        }

        private Vector2 _vevPos = Vector2.zero;
        private static Object _searchObject;
        private List<Object> _result = new List<Object>();
        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            _searchObject = EditorGUILayout.ObjectField( _searchObject, typeof( Object ), true, GUILayout.Width( 200 ) );
            if ( GUILayout.Button( "查找预制件", GUILayout.Width( 100 ) ) )
            {
                _result.Clear();
                _vevPos = Vector2.zero;

                if ( _searchObject == null )
                    return;

                string assetPath = AssetDatabase.GetAssetPath( _searchObject );
                string assetGuid = AssetDatabase.AssetPathToGUID( assetPath );
                //检查prefab
                string[] guids = AssetDatabase.FindAssets( "t:Prefab", new[] { "Assets" } );

                var result = guids;
                int length = result.Length;
                for ( int i = 0; i < length; i++ )
                {
                    string filePath = AssetDatabase.GUIDToAssetPath( result[ i ] );
                    EditorUtility.DisplayCancelableProgressBar( "Checking", filePath, i / length * 1.0f );

                    //检查是否包含guid
                    string content = File.ReadAllText( filePath );
                    if ( content.Contains( assetGuid ) )
                    {
                        Object fileObject = AssetDatabase.LoadAssetAtPath( filePath, typeof( Object ) );
                        _result.Add( fileObject );
                    }
                }

                EditorUtility.ClearProgressBar();
            }
            if ( GUILayout.Button( "查找材质球", GUILayout.Width( 100 ) ) )
            {
                _result.Clear();
                _vevPos = Vector2.zero;

                if ( _searchObject == null )
                    return;

                string assetPath = AssetDatabase.GetAssetPath( _searchObject );
                string assetGuid = AssetDatabase.AssetPathToGUID( assetPath );
                //检查Mat
                string[] guidsMat = AssetDatabase.FindAssets( "t:Material", new[] { "Assets" } );

                var result = guidsMat;
                int length = result.Length;
                for ( int i = 0; i < length; i++ )
                {
                    string filePath = AssetDatabase.GUIDToAssetPath( result[ i ] );
                    EditorUtility.DisplayCancelableProgressBar( "Checking", filePath, i / length * 1.0f );

                    //检查是否包含guid
                    string content = File.ReadAllText( filePath );
                    if ( content.Contains( assetGuid ) )
                    {
                        Object fileObject = AssetDatabase.LoadAssetAtPath( filePath, typeof( Object ) );
                        _result.Add( fileObject );
                    }
                }

                EditorUtility.ClearProgressBar();
            }
            EditorGUILayout.EndHorizontal();

            //显示结果
            _vevPos = EditorGUILayout.BeginScrollView( _vevPos );
            for ( int i = 0; i < _result.Count; i++ )
            {
                EditorGUILayout.ObjectField( _result[ i ], typeof( Object ), true, GUILayout.Width( 400 ) );
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
