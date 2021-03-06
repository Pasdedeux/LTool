﻿/*======================================
* 项目名称 ：LitFramework.Unity
* 项目描述 ：
* 类 名 称 ：LDebug
* 类 描 述 ：一个自定义的日志输出
* 命名空间 ：LitFramework.Unity
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/5/17 16:40:02
* 更新时间 ：2019/5/17 16:40:02
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:  首版本增加颜色控制，便于阅读，根据需要支持更多类型
*修改时间：2019/5/17 16:40:02
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using System.Collections;

public enum LogColor
{
    black,
    white,
    red,
    yellow,
    green,
    blue,
    purple,
    orange,
    grey
}

public static class LDebug
{
    private static bool _enabeld = true;
    public static bool Enable
    {
        get { return _enabeld; }
        set
        {
            _enabeld = value;
            UnityEngine.Debug.unityLogger.logEnabled = value;
        }
    }

    public static void Log(object content)
    {
        if ( content is string ) Log( ( string )content );
        else UnityEngine.Debug.Log( content );
    }

    public static void LogWarning( object content )
    {
        if ( content is string ) LogWarning( ( string )content );
        else UnityEngine.Debug.LogWarning( content );
    }

    public static void LogError( object content )
    {
        if ( content is string ) LogError( ( string )content );
        else UnityEngine.Debug.LogError( content );
    }

    public static void Log( string content, LogColor color = LogColor.green )
    {
        if ( !Enable ) return;
        UnityEngine.Debug.LogFormat( "<color={0}>{1}</color>", color, content );
    }

    public static void LogWarning( string content, LogColor color = LogColor.green )
    {
        if ( !Enable ) return;
        UnityEngine.Debug.LogWarningFormat( "<color={0}>{1}</color>", color, content );
    }

    public static void LogError( string content, LogColor color = LogColor.green )
    {
        if ( !Enable ) return;
        UnityEngine.Debug.LogErrorFormat( "<color={0}>{1}</color>", color, content );
    }

    public static void LogForEach( ICollection contens, LogColor color = LogColor.green )
    {
        if ( !Enable ) return;
        foreach ( var item in contens )
        {
            UnityEngine.Debug.LogErrorFormat( "<color={0}>{1}</color>", color, item.ToString() );
        }
    }
}
