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
        [ExecuteInEditMode]
#if UNITY_EDITOR
        [MenuItem( "Tools/Build/Build Audio &s" )]
#endif
        public static void RegisterAudios()
        {
            FileInfo _saveLocalFileInfo = new FileInfo( AssetPathManager.Instance.GetStreamAssetDataPath( GlobalEditorSetting.JSON_FILE_NAME, false ) );

            //============JSON文件取出============//
            ResPathTemplate rpt = null;
            //如果文件存在，则读取解析为存储类，写入相关数据条后写入JSON并保存
            if ( DocumentAccessor.IsExists( AssetPathManager.Instance.GetStreamAssetDataPath( GlobalEditorSetting.JSON_FILE_NAME, false ) ) )
            {
                var content = DocumentAccessor.ReadFile( AssetPathManager.Instance.GetStreamAssetDataPath( GlobalEditorSetting.JSON_FILE_NAME, false ) );

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
            DirectoryInfo folder = new DirectoryInfo( "Assets/Resources/"+ GlobalEditorSetting.AUDIO_PATH );

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
            EditorMenuExtention.CreateCSFile( Application.dataPath + "/Scripts", GlobalEditorSetting.OUTPUT_FILENAME, rpp.CreateCS( rpt ) );
            AssetDatabase.Refresh();
        }
    }
}
