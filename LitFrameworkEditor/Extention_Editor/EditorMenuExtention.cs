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
using System.Linq;
using System.Text;

namespace LitFrameworkEditor.EditorExtended
{
    using UnityEngine;
    using System.IO;
    using System.Data;
#if UNITY_EDITOR
    using UnityEditor;
    using ExcelDataReader;
#endif
    using System.Reflection;

    //% - CTRL on Windows / CMD on OSX
    //# - Shift
    //& -Alt
    //LEFT/RIGHT/UP/DOWN - Arrow keys
    //F1 … F2 - F keys
    //HOME,END,PGUP,PGDN
    //字母键 - _ + 字母（如:_g代表按键）
    //[MenuItem("Tools/New Option %#a"]//CTRL-SHIFT-A

    /// <summary>
    /// 
    /// </summary>
    public class EditorMenuExtention
    {

#if UNITY_EDITOR
        [MenuItem( "Tools/配置文件->CSV",priority =20)]
#endif
        public static void XlsxToCSV()
        {
            string xlsxpath = Application.dataPath + "/XLSX";
            string csvpath = Application.dataPath + "/StreamingAssets/csv";
            //文件列表
            string listpath = Application.dataPath + "/StreamingAssets/csvList.txt";
            FileStream fs = new FileStream( listpath, FileMode.Create );
            StreamWriter listwriter = new StreamWriter( fs, Encoding.UTF8 );
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
                        listwriter.WriteLine( NextFile.Name.Split( '.' )[ 0 ] + ".csv" );
                    }
                    else if ( Path.GetExtension( NextFile.Name ) == ".txt" )
                    {
                        FileInfo fi = new FileInfo( csvpath + "/" + NextFile.Name );
                        if ( fi.Exists )
                            fi.Delete();
                        NextFile.CopyTo( csvpath + "/" + NextFile.Name );
                        listwriter.WriteLine( NextFile.Name );
                    }
                }
            }
            catch ( Exception e ) { Debug.LogError( e.Message ); }
            finally
            {
                listwriter.Close();
                listwriter.Dispose();
                fs.Dispose();
#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
            }
        }

#if UNITY_EDITOR
        [MenuItem( "Tools/配置文件->CSV+代码", priority = 21 )]
#endif
        public static void CsvToCs()
        {
            Debug.Log( "配置文件转化为代码  开始!" );
            string xlsxpath = Application.dataPath + "/XLSX";
            string csvOutPath = Application.dataPath + "/StreamingAssets/csv";
            string csOutPath = Application.dataPath + "/Scripts/CSV";
            DirectoryInfo theXMLFolder = new DirectoryInfo( xlsxpath );

            //文件列表
            string listpath = Application.dataPath + "/StreamingAssets/csvList.txt";
            FileStream fs = new FileStream( listpath, FileMode.Create );
            StreamWriter listwriter = new StreamWriter( fs, Encoding.UTF8 );

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
                //对文件进行遍历
                foreach ( var NextFile in theXMLFolder.GetFiles() )
                {
                    if ( Path.GetExtension( NextFile.Name ) == ".xlsx" )
                    {
                        string csvfile = XLSXTOCSV( NextFile.OpenRead() );
                        CSVParser cp = new CSVParser();
                        CreateCSFile( csOutPath, NextFile.Name.Split( '.' )[ 0 ] + ".cs", cp.CreateCS( NextFile.Name.Split( '.' )[ 0 ], csvfile ) );
                        CreateCSVFile( csvOutPath + "/" + NextFile.Name.Split( '.' )[ 0 ] + ".csv", csvfile );
                        Debug.Log( NextFile.Name.Split( '.' )[ 0 ] + "  文件生成成功！" );

                        listwriter.WriteLine( NextFile.Name.Split( '.' )[ 0 ] + ".csv" );
                    }
                    else if ( Path.GetExtension( NextFile.Name ) == ".txt" )
                    {
                        FileInfo fi = new FileInfo( csvOutPath + "/" + NextFile.Name );
                        if ( fi.Exists )
                            fi.Delete();
                        NextFile.CopyTo( csvOutPath + "/" + NextFile.Name );
                        listwriter.WriteLine( NextFile.Name );
                    }
                }
            }
            catch ( Exception e ) { Debug.LogError( e.Message ); }
            finally
            {
                listwriter.Close();
                listwriter.Dispose();
                fs.Dispose();
#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
            }
        }

        /// <summary>
        /// 创建CS文件
        /// </summary>
        static void CreateCSFile( string path, string className, string cs )
        {
            if ( !Directory.Exists( path ) )
            {
                //该路径不存在
                Directory.CreateDirectory( path );
            }
            path += "/";
            path += className;

            FileStream fs = new FileStream( path, FileMode.Create );
            StreamWriter sw = new StreamWriter( fs, Encoding.UTF8 );
            sw.Write( cs );
            sw.Close();
            sw.Dispose();
            fs.Dispose();
        }

        static void CreateCSVFile( string path, string data )
        {
            FileStream fs = new FileStream( path, FileMode.Create );
            StreamWriter sw = new StreamWriter( fs, Encoding.UTF8 );
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
        [MenuItem( "Tools/开启PersistentDataPath(可同步目录)", priority = 2 )]
        private static void OpenPersistentDataPath()
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.Start( Application.persistentDataPath );
            p.Close();
        }

        [MenuItem( "Tools/开启TemporaryCachePath(临时存储目录)", priority = 3 )]
        private static void OpenTemporaryCachePath()
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.Start( Application.temporaryCachePath );
            p.Close();
        }

        [MenuItem( "Tools/开启StreamingAssetsPath(项目内存储目录)", priority = 1 )]
        private static void OpenSteamingAssetsPath()
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.Start( Application.streamingAssetsPath );
            p.Close();
        }

        [MenuItem( "Tools/清档(PlayerPrefs)", priority = 90 )]
        private static void RemoveAllKey()
        {
            PlayerPrefs.DeleteAll();
        }


        [MenuItem( "Tools/删除空白目录", priority = 91 )]
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

        [MenuItem( "Tools/隐藏选中的物体 &q", priority = 101 )]
        public static void HideChoosedObject()
        {
            var choosed = Selection.gameObjects;
            foreach ( var item in choosed )
            {
                item.SetActive( !item.activeSelf );
            }
        }
#endif
    }


    /// <summary>
    /// 查找引用预制件窗口
    /// </summary>
    public class SearchRefrenceEditorWindow : EditorWindow
    {
        /// <summary>
        /// 查找引用
        /// </summary>
        [MenuItem( "Tools/查找引用的预制件" )]
        static void SearchRefrence()
        {
            SearchRefrenceEditorWindow window = ( SearchRefrenceEditorWindow )EditorWindow.GetWindow( typeof( SearchRefrenceEditorWindow ), false, "Searching", true );
            window.Show();
        }

        private static Object _searchObject;
        private List<Object> _result = new List<Object>();
        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            _searchObject = EditorGUILayout.ObjectField( _searchObject, typeof( Object ), true, GUILayout.Width( 200 ) );
            if ( GUILayout.Button( "查找", GUILayout.Width( 100 ) ) )
            {
                _result.Clear();

                if ( _searchObject == null )
                    return;

                string assetPath = AssetDatabase.GetAssetPath( _searchObject );
                string assetGuid = AssetDatabase.AssetPathToGUID( assetPath );
                //只检查prefab
                string[] guids = AssetDatabase.FindAssets( "t:Prefab", new[] { "Assets" } );

                int length = guids.Length;
                for ( int i = 0; i < length; i++ )
                {
                    string filePath = AssetDatabase.GUIDToAssetPath( guids[ i ] );
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
            EditorGUILayout.BeginVertical();
            for ( int i = 0; i < _result.Count; i++ )
            {
                EditorGUILayout.ObjectField( _result[ i ], typeof( Object ), true, GUILayout.Width( 300 ) );
            }
            EditorGUILayout.EndHorizontal();
        }

    }
}
