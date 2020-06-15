/*======================================
* 项目名称 ：Assets.Scripts.Extention_Editor
* 项目描述 ：
* 类 名 称 ：BuildSceneSettingEditor
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Extention_Editor
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2020/6/15 11:01:18
* 更新时间 ：2020/6/15 11:01:18
* 版 本 号 ：v1.0.0.0
*******************************************************************
======================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

/// <summary>
/// 构建场景编辑器
/// </summary>
public static class BuildSceneSettingEditor
{
    /// <summary>
    /// 构建场景设置为所有
    /// </summary>
    [MenuItem( "Tools/BuildSettings/同步所有场景到SceneSetting文件" )]
    public static void AddAllScenesToBuildSettings()
    {
        HashSet<string> sceneNames = new HashSet<string>();

        string[] paths = new string[] { "Assets/Scenes" };     //指定场景所在的文件夹名称，如果是“Assets”,则代表工程里面所有的场景
        string[] sceneArr = AssetDatabase.FindAssets( "t:Scene", paths );

        foreach ( string scene in sceneArr )
        {
            sceneNames.Add( AssetDatabase.GUIDToAssetPath( scene ) );
        }
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();

        int index = 0;
        foreach ( string scensName in sceneNames )
        {
            //string name = scensName.Split( '/' ).Last().Split( '.' )[ 0 ];
            //AppConfig.BuildSceneIndexDict.Add( name, index );
            scenes.Add( new EditorBuildSettingsScene( scensName, true ) );
            index++;
        }
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    /// <summary>
    /// 删除所有的构建场景
    /// </summary>
    [MenuItem( "Tools/BuildSettings/删除所有构建场景" )]
    public static void DeleteScenesFormBuildSettings()
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}