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
#endif

    /// <summary>
    /// 包含Tools菜单下一系列工具包文件
    /// </summary>
    public class EditorMenuExtention
    {
        /// <summary>
        /// 写入CSV标题栏
        /// </summary>
        private static string _csvListTitle = "CsvName,Version,MD5";
        /// <summary>
        /// 写入CSV的值
        /// </summary>
        private static string _csvContentValue = "{0},{1},{2}";
        /// <summary>
        /// 需要存档的配置文件
        /// </summary>
        private static List<ABVersion> _csvListToBeRestored = new List<ABVersion>();

        /// <summary>
        /// 将XLSX文件夹的excel文档转换为csv文件
        /// </summary>
#if UNITY_EDITOR
        [MenuItem( "Tools/配置文件->CSV", priority = 20 )]
#endif
        public static void XlsxToCSV()
        {
            string xlsxpath = Application.dataPath + "/XLSX";
            string streampath = Application.dataPath + "/StreamingAssets";
            string csvpath = Application.dataPath + "/StreamingAssets/csv";
            _csvListToBeRestored.Clear();
            //文件列表
            //string listpath = Application.dataPath + "/StreamingAssets/csvList.txt";
            //FileStream fs = new FileStream( listpath, FileMode.Create );
            //StreamWriter listwriter = new StreamWriter( fs, new UTF8Encoding(false) );
            DirectoryInfo TheFolder = new DirectoryInfo( xlsxpath );

            if ( !Directory.Exists( csvpath ) )
            {
                Directory.CreateDirectory( csvpath );
            }

            try
            {
                //对文件进行遍历
                foreach ( var NextFile in TheFolder.GetFiles() )
                {
                    if ( Path.GetExtension( NextFile.Name ) == ".xlsx" )
                    {
                        string csvfile = XLSXTOCSV( NextFile.OpenRead() );
                        CreateCSVFile( csvpath + "/" + NextFile.Name.Split( '.' )[ 0 ] + ".csv", csvfile );
                        Debug.Log( NextFile.Name.Split( '.' )[ 0 ] + "  文件生成成功！" );
                        //listwriter.WriteLine( "csv/" + NextFile.Name.Split( '.' )[ 0 ] + ".csv" );
                        string str = "csv/" + NextFile.Name.Split( '.' )[ 0 ] + ".csv";
                        _csvListToBeRestored.Add( new ABVersion { AbName = str, MD5 = LitFramework.Crypto.Crypto.md5.GetFileHash( csvpath + "/" + NextFile.Name.Split( '.' )[ 0 ] ), Version = 1 } ); 
                    }
                    else if ( Path.GetExtension( NextFile.Name ) == ".txt" )
                    {
                        FileInfo fi = new FileInfo( csvpath + "/" + NextFile.Name );
                        if ( fi.Exists )
                            fi.Delete();
                        NextFile.CopyTo( csvpath + "/" + NextFile.Name );
                        //listwriter.WriteLine( NextFile.Name );
                        _csvListToBeRestored.Add( new ABVersion { AbName = NextFile.Name, MD5 = string.Empty, Version = 0 } );
                    }
                }

                //遍历框架配置的额外后缀文件
                string[] extralFile = FrameworkConfig.Instance.configs_suffix.Split( '|' );
                foreach ( var item in extralFile )
                {
                    if ( item.Equals( "csv" ) ) continue;

                    GetFiles( new DirectoryInfo( streampath ), item , _csvListToBeRestored );
                }
            }
            catch ( Exception e ) { Debug.LogError( e.Message ); }
            finally
            {
                //加载本地文件，没有就创建完成。有则比对同名文件的MD5，不一样则version+1
                MatchCSVTotalFile( _csvListToBeRestored );

                //listwriter.Close();
                //listwriter.Dispose();
                //fs.Dispose();
#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
            }
        }

        /// <summary>
        /// 加载本地文件，没有就创建完成。有则比对同名文件的MD5，不一样则version+1
        /// </summary>
        /// <param name="csvListToBeRestored"></param>
        private static void MatchCSVTotalFile( List<ABVersion> csvListToBeRestored )
        {
            string listpath = Application.dataPath + "/StreamingAssets/csvList.txt";
            FileStream fs = new FileStream( listpath, FileMode.Create );
            StreamWriter listwriter = new StreamWriter( fs, new UTF8Encoding( false ) );
            listwriter.WriteLine( _csvListTitle );

            if ( DocumentAccessor.IsExists( AssetPathManager.Instance.GetStreamAssetDataPath( "csvList.txt" ) ) )
            {
                //本地主配置文件获取
                string localContent = null;
                string localFilePath = AssetPathManager.Instance.GetStreamAssetDataPath( "csvList.txt", false );
                DocumentAccessor.LoadAsset( localFilePath, ( string e ) => { localContent = e; } );
                List<ABVersion> localABVersionsDic = ResolveABContent( localContent );

                for ( int i = 0; i < csvListToBeRestored.Count; i++ )
                {
                    ABVersion toSave = csvListToBeRestored[ i ];
                    for ( int k = 0; k < localABVersionsDic.Count; k++ )
                    {
                        ABVersion local = localABVersionsDic[ k ];
                        if ( local.AbName == toSave.AbName )
                        {
                            toSave.Version = local.MD5 != toSave.MD5 ? local.Version + 1 : local.Version;
                        }
                    }
                    listwriter.WriteLine( string.Format( _csvContentValue, toSave.AbName, toSave.Version, toSave.MD5 ) );
                }
            }
            else
            {
                for ( int i = 0; i < csvListToBeRestored.Count; i++ )
                {
                    listwriter.WriteLine( string.Format( _csvContentValue , csvListToBeRestored[i].AbName, csvListToBeRestored[ i ].Version, csvListToBeRestored[ i ].MD5 ) );
                }
            }
            listwriter.Close();
            listwriter.Dispose();
            fs.Dispose();
        }

        //解析ABVersion配置表
        private static List<ABVersion> ResolveABContent( string contentResolve )
        {
            List<ABVersion> resultDict = new List<ABVersion>();

            string[] str = contentResolve.Split( "\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );
            int toLoadNum = str.Length;

            for ( int k = 1; k < str.Length; k++ )
            {
                string line = str[ k ];
                if ( line != "" )
                {
                    string[] content = line.Split( ',' );
                    ABVersion ab = new ABVersion
                    {
                        AbName = content[ 0 ],
                        Version = int.Parse( content[ 1 ] ),
                        MD5 = content[ 2 ].Trim()
                    };
                    resultDict.Add( ab );
                }
            }

            return resultDict;
        }

        /// <summary>
        /// 将SA文件夹下CSV配置文件由csv格式转换为CS代码文件
        /// </summary>
#if UNITY_EDITOR
        [MenuItem( "Tools/配置文件->CSV+代码", priority = 21 )]
#endif
        public static void CsvToCs()
        {
            Debug.Log( "配置文件转化为代码  开始!" );
            _csvListToBeRestored.Clear();
            string xlsxpath = Application.dataPath + "/XLSX";
            string streampath = Application.dataPath + "/StreamingAssets";
            string csvOutPath = Application.dataPath + "/StreamingAssets/csv";
            string csOutPath = Application.dataPath + "/Scripts/CSV";
            if ( !FrameworkConfig.Instance.UseHotFixMode )
                csOutPath = Application.dataPath + "/Scripts/CSV";
            else
                csOutPath = Application.dataPath + "/Scripts/ILRuntime/HotFixLogic/CSV";
            DirectoryInfo theXMLFolder = new DirectoryInfo( xlsxpath );

            //文件列表
            //string listpath = Application.dataPath + "/StreamingAssets/csvList.txt";
            //FileStream fs = new FileStream( listpath, FileMode.Create );
            //StreamWriter listwriter = new StreamWriter( fs, new UTF8Encoding(false) );

            if ( !Directory.Exists( csvOutPath ) )
            {
                Directory.CreateDirectory( csvOutPath );
            }
            if ( !Directory.Exists( csOutPath ) )
            {
                Directory.CreateDirectory( csOutPath );
            }

            try
            {
                ConfigsNamesTemplate cnt = new ConfigsNamesTemplate();
                //对文件进行遍历
                foreach ( var NextFile in theXMLFolder.GetFiles() )
                {
                    if ( Path.GetExtension( NextFile.Name ) == ".xlsx" )
                    {
                        string csvfile = XLSXTOCSV( NextFile.OpenRead() );
                        CSVParser cp = new CSVParser();
                        CreateCSFile( csOutPath, NextFile.Name.Split( '.' )[ 0 ] + ".cs", cp.CreateCS( NextFile.Name.Split( '.' )[ 0 ], csvfile ) );
                        CreateCSVFile( csvOutPath + "/" + NextFile.Name.Split( '.' )[ 0 ] + ".csv", csvfile );
                        LDebug.Log( NextFile.Name.Split( '.' )[ 0 ] + "  文件生成成功！" );

                        //这里固定取配置表第三行配置作为类型读取，如果需要修改配置表适配服务器（增加第四行），需要同步修改
                        CSVReader reader = new CSVReader( csvfile );
                        cnt.configsNameList.Add( NextFile.Name.Split( '.' )[ 0 ], reader.GetData( 0, 2 ) );
                        //listwriter.WriteLine( "csv/" + NextFile.Name.Split( '.' )[ 0 ] + ".csv" );
                        string str = "csv/" + NextFile.Name.Split( '.' )[ 0 ] + ".csv";
                        _csvListToBeRestored.Add( new ABVersion { AbName = str, MD5 = LitFramework.Crypto.Crypto.md5.GetFileHash( csOutPath + "/" + NextFile.Name.Split( '.' )[ 0 ] ), Version = 1 } );
                    }
                    else if ( Path.GetExtension( NextFile.Name ) == ".txt" )
                    {
                        FileInfo fi = new FileInfo( csvOutPath + "/" + NextFile.Name );
                        if ( fi.Exists )
                            fi.Delete();
                        NextFile.CopyTo( csvOutPath + "/" + NextFile.Name );
                        //listwriter.WriteLine( NextFile.Name );
                        _csvListToBeRestored.Add( new ABVersion { AbName = NextFile.Name, MD5 = string.Empty, Version = 0 } );
                    }
                }

                //遍历框架配置的额外后缀文件
                string[] extralFile = FrameworkConfig.Instance.configs_suffix.Split( '|' );
                foreach ( var item in extralFile )
                {
                    if ( item.Equals( "csv" ) ) continue;

                    GetFiles( new DirectoryInfo( streampath ), item, _csvListToBeRestored );
                }

                //============更新并保存CS============//
                ConfigsParse rpp = new ConfigsParse();
                
                if ( !FrameworkConfig.Instance.UseHotFixMode )
                    EditorMenuExtention.CreateCSFile( Application.dataPath + "/Scripts/Model/Const/", "Configs.cs", rpp.CreateCS( cnt ) );
                else
                    EditorMenuExtention.CreateCSFile( Application.dataPath + "/Scripts/ILRuntime/HotFixLogic/Model/Const/", "Configs.cs", rpp.CreateCS( cnt ) );
            }
            catch ( Exception e ) { LDebug.LogError( e.Message ); }
            finally
            {
                //加载本地文件，没有就创建完成。有则比对同名文件的MD5，不一样则version+1
                MatchCSVTotalFile( _csvListToBeRestored );

                //listwriter.Close();
                //listwriter.Dispose();
                //fs.Dispose();
#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
            }
        }

        /// <summary>
        /// 创建CS文件
        /// </summary>
        public static void CreateCSFile( string path, string className, string cs )
        {
            if ( !Directory.Exists( path ) )
            {
                //该路径不存在
                Directory.CreateDirectory( path );
            }
            path += "/";
            path += className;

            FileStream fs = new FileStream( path, FileMode.Create );
            StreamWriter sw = new StreamWriter( fs, new UTF8Encoding(false) );
            sw.Write( cs );
            sw.Close();
            sw.Dispose();
            fs.Dispose();
        }

        static void CreateCSVFile( string path, string data )
        {
            FileStream fs = new FileStream( path, FileMode.Create );
            StreamWriter sw = new StreamWriter( fs, new UTF8Encoding(false) );
            sw.Write( data );
            sw.Close();
            sw.Dispose();
            fs.Dispose();
        }

        static string XLSXTOCSV( FileStream stream )
        {
#if UNITY_EDITOR
            using ( IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader( stream ) )
            {
                DataSet result = excelReader.AsDataSet();
                CSVWriter writer = new CSVWriter();
                int rows = result.Tables[ 0 ].Rows.Count;

                //通常情况下，获取第一行的列数即可
                int rowlen = 0;
                for ( int j = 0; j < result.Tables[ 0 ].Rows[ 0 ].ItemArray.Length; j++ )
                {
                    if ( result.Tables[ 0 ].Rows[ 0 ].ItemArray[ j ].ToString() == "" )
                        break;
                    rowlen++;
                }

                for ( int i = 0; i < rows; i++ )
                {
                    if ( result.Tables[ 0 ].Rows[ i ].ItemArray[ 0 ].ToString() == "" )
                        break;

                    List<object> rowlist = new List<object>();

                    for ( int j = 0; j < rowlen; j++ )
                    {
                        rowlist.Add( result.Tables[ 0 ].Rows[ i ].ItemArray[ j ] );
                    }
                    writer.AddRow( rowlist.ToArray() );

                }
                return writer.ToString();
            }
#else
            return "";
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
                        listwriter.WriteLine( info.Name );
                    }
                }
                catch ( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
                foreach ( DirectoryInfo info in directory.GetDirectories() )//获取文件夹下的子文件夹
                {
                    GetFiles( info, pattern , listwriter );//递归调用该函数，获取子文件夹下的文件
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
                    foreach ( FileInfo info in directory.GetFiles( "*." + pattern ) )
                    {
                        listwriter.Add( new ABVersion { AbName = info.Name, MD5 = string.Empty, Version = 0 } );
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
