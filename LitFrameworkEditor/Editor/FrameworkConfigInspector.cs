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

        _foldOut = EditorGUILayout.Foldout( _foldOut, "基于Update的[延迟函数]参数设置" );
        if ( _foldOut )
        {
            if ( !_config.UseDelayFuncPreciseDetect )
                _config.DelayFuncDetectInterver = EditorGUILayout.FloatField( "计时间隔(值↑ 性能↑)", _config.DelayFuncDetectInterver );

            _config.UseDelayFuncPreciseDetect = EditorGUILayout.Toggle( "高计时精确度", _config.UseDelayFuncPreciseDetect );
            
            EditorGUILayout.Space();
            
        }
        
        
        EditorGUILayout.EndVertical();
    }
}
