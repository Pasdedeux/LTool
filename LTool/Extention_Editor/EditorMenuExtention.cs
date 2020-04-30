﻿#region << 版 本 注 释 >>
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

namespace LitFramework.EditorExtended
{
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using System.Data;
    using ExcelDataReader;
    using System.Reflection;

    public class EditorMenuExtention
    {

#if UNITY_EDITOR
        [MenuItem( "Tools/配置文件->CSV" )]
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
            }
        }

#if UNITY_EDITOR
        [MenuItem( "Tools/配置文件->CSV+代码" )]
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
            }
        }

#if UNITY_EDITOR
        [MenuItem( "Tools/拷贝到Unity工程" )]
#endif
        public static void CopyToUnity()
        {
            string uPath = Application.dataPath + "/Scripts/DllFramework/";
            string hPath = Application.dataPath + "/../../HoxLogic/HoxLogic/DllFramework/";

            FolderCopy.CopyTo( hPath, uPath );
            Debug.Log( "所有文件拷贝完毕" );
        }

#if UNITY_EDITOR
        [MenuItem( "Tools/拷贝到热更新工程" )]
        public static void CopyToHotFix()
        {
            if ( EditorUtility.DisplayDialog( "提示", "是否拷贝到热更新工程", "确认", "取消" ) )
            {
                string uPath = Application.dataPath + "/Scripts/DllFramework";
                string hPath = Application.dataPath + "/../../HoxLogic/HoxLogic/DllFramework";

                FolderCopy.CopyTo( uPath, hPath );
            }
            else
            {
                Debug.Log( "Cancel" );
            }
        }
#endif




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

#endif
            return "";
        }

        [MenuItem( "Tools/Remove Missing Scripts", priority = 10 )]
        public static void RemoveMissingScript()
        {
            var gos = GameObject.FindObjectsOfType<GameObject>();
            foreach ( var item in gos )
            {
                Debug.Log( item.name );
                SerializedObject so = new SerializedObject( item );
                var soProperties = so.FindProperty( "m_Component" );
                var components = item.GetComponents<Component>();
                int propertyIndex = 0;
                foreach ( var c in components )
                {
                    if ( c == null )
                    {
                        soProperties.DeleteArrayElementAtIndex( propertyIndex );
                    }
                    ++propertyIndex;
                }
                so.ApplyModifiedProperties();
            }

            AssetDatabase.Refresh();
            Debug.Log( "清理完成!" );
        }
        [MenuItem( "Tools/Remove Selected Rigidbody", priority = 10 )]
        public static void RemoveRig()
        {
            foreach ( var item in Selection.gameObjects )
            {
                Rigidbody rig = item.GetComponent<Rigidbody>();
                if ( rig )
                {
                    DestroyImmediate( rig );
                }
                Animator ani = item.GetComponent<Animator>();
                if ( ani )
                {
                    DestroyImmediate( ani );
                }
            }
            EditorSceneManager.MarkSceneDirty( SceneManager.GetActiveScene() );
        }

        [MenuItem( "Tools/Open PersistentDataPath", priority = 10 )]
        static void OpenPersistentDataPath()
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.Start( Application.persistentDataPath );
            p.Close();
        }

        [MenuItem( "Tools/Delete All Data", priority = 10 )]
        public static void RemoveAllKey()
        {
            PlayerPrefs.DeleteAll();
        }

    }
}
