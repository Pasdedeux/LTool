/*======================================
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

    private bool _foldOut = true;
    public override void OnInspectorGUI()
    {
        //设置整个界面是以垂直方向来布局
        EditorGUILayout.BeginVertical();

        _foldOut = EditorGUILayout.Foldout( _foldOut, "基于Update的延迟函数参数设置" );
        if ( _foldOut )
        {
            EditorGUILayout.LabelField( "延迟调用函数计时检测间隔" );
            _config.DelayFuncDetectInterver = EditorGUILayout.FloatField( "DelayFuncDetectInterver", _config.DelayFuncDetectInterver );
            EditorGUILayout.Space();
            EditorGUILayout.LabelField( "开启使用逐帧遍历延迟函数调用。默认为false" );
            _config.UseDelayFuncPreciseDetect = EditorGUILayout.Toggle( "UseDelayFuncPreciseDetect", _config.UseDelayFuncPreciseDetect );
        }
        
        
        EditorGUILayout.EndVertical();
    }
}
