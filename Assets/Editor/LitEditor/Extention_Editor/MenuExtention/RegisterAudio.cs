/*======================================
* 项目名称 ：LitFrameworkEditor.Extention_Editor
* 项目描述 ：
* 类 名 称 ：RegisterAudio
* 类 描 述 ：
* 命名空间 ：LitFrameworkEditor.Extention_Editor
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/2/26 11:56:12
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using Litframework.ExcelTool;
using LitFramework;
using LitFramework.LitTool;
using LitFrameworkEditor.EditorExtended;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace LitFrameworkEditor.Extention_Editor
{
    class RegisterAudio
    {
        //Resources 目录
        private static string UIPrefabBaseDirectoryName = "Resources";

        [ExecuteInEditMode]
#if UNITY_EDITOR
        [MenuItem( "Tools/Build/Build Audio &s", priority = 51 )]
#endif
        public static void RegisterAudios()
        {
            FileInfo _saveLocalFileInfo = new FileInfo( Application.dataPath + "/Editor/" + GlobalEditorSetting.JSON_FILE_NAME );

            //============JSON文件取出============//
            ResPathTemplate rpt = null;
            //如果文件存在，则读取解析为存储类，写入相关数据条后写入JSON并保存
            if ( DocumentAccessor.IsExists( Application.dataPath + "/Editor/" + GlobalEditorSetting.JSON_FILE_NAME ) )
            {
                var content = DocumentAccessor.ReadFile( Application.dataPath + "/Editor/" + GlobalEditorSetting.JSON_FILE_NAME );

                rpt = JsonMapper.ToObject<ResPathTemplate>( content );
            }
            //如果文件不存在，则新建存储类，并保存相关的数据，然后写入JSON并保存
            else
            {
                rpt = new ResPathTemplate();
            }
            //=================================//

            //每次都重新写入
            rpt.Sound.Clear();

            //============存入音频配置============//

            List<string> allResourcesPath = new List<string>();
            RecursionAction( "Assets", allResourcesPath );

            foreach ( var childPath in allResourcesPath )
            {
                DirectoryInfo folder = new DirectoryInfo( "Assets" + childPath + "/" + GlobalEditorSetting.AUDIO_PATH );

                if ( !folder.Exists ) continue;

                foreach ( FileInfo file in folder.GetFiles() )
                {
                    string ss = file.Extension.ToUpper();
                    if ( ss.Contains( ".AIFF" ) || ss.Contains( ".WAV" ) || ss.Contains( ".MP3" ) || ss.Contains( ".OGG" ) )
                    {
                        var result = file.FullName.Substring( file.FullName.LastIndexOf( '\\' ) + 1 );
                        result = result.Split( '.' )[ 0 ];

                        rpt.Sound.Add( result.ToUpper(), GlobalEditorSetting.AUDIO_PATH + "/" + result );
                    }
                }
            }

            //=================================//

            //============JSON文件存入============//
            using ( StreamWriter sw = _saveLocalFileInfo.CreateText() )
            {
                var result = JsonMapper.ToJson( rpt );
                sw.Write( result );
            }
            //=================================//

            //============更新并保存CS============//
            ResPathParse rpp = new ResPathParse();
            if (!FrameworkConfig.Instance.UseHotFixMode)
                ExcelExport.CreateCSFile(Application.dataPath + "/Scripts", GlobalEditorSetting.OUTPUT_RESPATH, rpp.CreateCS(rpt));
            else
                ExcelExport.CreateCSFile( Application.dataPath + "/Scripts/RuntimeScript/HotFixLogic/UIExport", GlobalEditorSetting.OUTPUT_RESPATH, rpp.CreateCS( rpt ) );
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// 递归方法，用于查找所有文件夹
        /// </summary>
        /// <param name="dirName"></param>
        private static void RecursionAction( string dirName, List<string> pathList )
        {
            if ( dirName.Contains( UIPrefabBaseDirectoryName ) )
            {
                DirectoryInfo di = new DirectoryInfo( dirName );
                if ( di.Name == UIPrefabBaseDirectoryName )
                {
                    var liss = di.FullName.Split( new string[] { "Assets" }, StringSplitOptions.RemoveEmptyEntries );
                    //TDOO 这里需要检查下MAC环境下的下划线
                    liss[ 1 ] = liss[ 1 ].Replace( "\\", "/" );
                    pathList.Add( liss[ 1 ] );
                }
            }

            if ( dirName.Contains( "The3rd" )
                || dirName.Contains( "Plugins" )
                || dirName.Contains( "Scripts" )
                || dirName.Contains( "XLSX" ) ) return;

            string[] dir = Directory.GetDirectories( dirName );
            if ( dir.Length > 0 )
            {
                foreach ( string di in dir )
                    RecursionAction( di, pathList );
            }
        }
    }
}
