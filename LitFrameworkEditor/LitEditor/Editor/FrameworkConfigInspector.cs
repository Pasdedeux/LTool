﻿/*======================================
* 项目名称 ：LitFrameworkEditor.Editor
* 项目描述 ：
* 类 名 称 ：FrameworkConfigInspector
* 类 描 述 ：
* 命名空间 ：LitFrameworkEditor.Editor
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/3/17 14:28:56
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using LitFramework;
using UnityEditor;
/// <summary>
/// FrameworkConfig
/// </summary>
[CustomEditor( typeof( FrameworkConfig ) )]
public class FrameworkConfigInspector : Editor
{
    private FrameworkConfig _config;

    private void OnEnable()
    {
        _config = ( FrameworkConfig )target;
    }

    private bool _foldOut = true, _folaOutEditor = true, _foldOutUI = true, _foldOutAB = true, _foldGameSetting = true;
    public override void OnInspectorGUI()
    {
        //设置整个界面是以垂直方向来布局
        EditorGUILayout.BeginVertical();

        _config.renderingType = ( LitRenderingType )EditorGUILayout.EnumPopup( "Rendering Type: ", _config.renderingType );
        _config.TargetFrameRate = EditorGUILayout.IntField( "目标帧率", _config.TargetFrameRate );
        _config.vSyncCount = EditorGUILayout.IntField( "是否开启vSunc(默认0即可)", _config.vSyncCount );

        EditorGUILayout.Space();

        _foldOut = EditorGUILayout.Foldout( _foldOut, "基于Update的[延迟函数]参数设置" );
        if ( _foldOut )
        {
            if ( !_config.UseDelayFuncPreciseDetect )
                _config.DelayFuncDetectInterver = EditorGUILayout.FloatField( "计时间隔(值↑ 性能↑)", _config.DelayFuncDetectInterver );

            _config.UseDelayFuncPreciseDetect = EditorGUILayout.Toggle( "逐帧计时", _config.UseDelayFuncPreciseDetect );

            EditorGUILayout.Space();
        }

        _folaOutEditor = EditorGUILayout.Foldout( _folaOutEditor, "编辑器设定" );
        if ( _folaOutEditor )
        {
            _config.UGUIOpt = EditorGUILayout.Toggle( "开启UGUI组件优化(对象创建)", _config.UGUIOpt );
        }

        EditorGUILayout.Space();
        _foldOutUI = EditorGUILayout.Foldout( _foldOutUI, "UI设定" );
        if ( _foldOutUI )
        {
            _config.TouchDetectUI = EditorGUILayout.Toggle( "开启UGUI触屏检测", _config.TouchDetectUI );
        }

        EditorGUILayout.Space();
        _foldOutAB = EditorGUILayout.Foldout( _foldOutAB, "AB设定" );
        if ( _foldOutAB )
        {
            _config.ABFolderName = EditorGUILayout.TextField( "AB包文件夹名称", _config.ABFolderName );
            _config.ABTotalName = EditorGUILayout.TextField( "AB总包名称", _config.ABTotalName );
        }

        EditorGUILayout.Space();
        _config.configs_suffix = EditorGUILayout.TextField( "录入的配置档后缀", _config.configs_suffix );
        _config.UsePersistantPath = EditorGUILayout.Toggle( "使用读写目录", _config.UsePersistantPath );
        if ( _config.UsePersistantPath )
        {
            _config.UseRemotePersistantPath = EditorGUILayout.Toggle( "使用远程更新", _config.UseRemotePersistantPath );
            if ( _config.UseRemotePersistantPath )
            {
                _config.RemoteUrlConfig = EditorGUILayout.TextField( "远程配置地址", _config.RemoteUrlConfig );
            }
        }

        EditorGUILayout.Space();
        _foldGameSetting = EditorGUILayout.Foldout( _foldGameSetting, "项目通用设定" );
        if ( _foldGameSetting )
        {
            _config.showLog = EditorGUILayout.Toggle( "开启调试日志", _config.showLog );
            _config.scriptEnvironment = ( RunEnvironment )EditorGUILayout.EnumPopup( "代码运行环境: ", _config.scriptEnvironment );
            _config.UseHotFixMode =EditorGUILayout.Toggle( "UI使用热更制作模式", _config.UseHotFixMode );
        }

        EditorGUILayout.EndVertical();
    }
}