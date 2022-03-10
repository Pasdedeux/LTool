/*======================================
* 项目名称 ：LitFramework.Unity
* 项目描述 ：
* 类 名 称 ：LDebug
* 类 描 述 ：一个自定义的日志输出,NLog
* 命名空间 ：LitFramework.Unity
* 版 本 号 ：v1.0.0.0
* /*======================================
*/

#if NLOG
using System.Collections;
using System.Diagnostics;
using NLog;
using NLog.Targets;

public static class LDebug
{
    private static bool _enabeld = true;
    private static ILogger m_NLogger= LogManager.GetCurrentClassLogger();
    private static int index = 0;
    private static void SetTarget()
    {
        if (LogManager.Configuration == null)
        {
            LogManager.LoadConfiguration(LitFramework.LitTool.AssetPathManager.Instance.GetStreamAssetDataPath("NLog.config", false));
        }
    }
    public static bool Enable
    {
        get { return _enabeld; }
        set
        {
            _enabeld = value;
            UnityEngine.Debug.unityLogger.logEnabled = value;
        }
    }

    [Conditional("NLOG")]
    public static void Log(object content)
    {
        if ( !Enable ) return;
        SetTarget();
        if (content is string)
        {
            Log((string)content);
        }
        else
        {
            m_NLogger.Info(content);
        }
    }

    [Conditional("NLOG")]
    public static void LogWarning( object content )
    {
        if ( !Enable ) return;
        SetTarget();
        if (content is string)
        {
            LogWarning((string)content);
        }
        else
        {
            m_NLogger.Warn(content);
        }
    }

    [Conditional("NLOG")]
    public static void LogError( object content )
    {
        if ( !Enable ) return;
        SetTarget();
        if (content is string)
        {
            LogError((string)content);
        }
        else
        {
            m_NLogger.Error(content);
        }
    }

    [Conditional("NLOG")]
    public static void LogFormat( string content, params object[] args )
    {
        if ( !Enable ) return;
        SetTarget();
        m_NLogger.Info(string.Format(content, args));
    }

    [Conditional("NLOG")]
    public static void LogWarningFormat( string content, params object[] args )
    {
        if ( !Enable ) return;
        SetTarget();
        m_NLogger.Warn(string.Format(content, args));
    }
   

    [Conditional("NLOG")]
    public static void LogErrorFormat( string content, params object[] args )
    {
        if ( !Enable ) return;
        SetTarget();
        m_NLogger.Error(new System.Diagnostics.StackTrace().ToString(),string.Format(content, args));
    }

    [Conditional("NLOG")]
    public static void Log( string content, LogColor color = LogColor.green )
    {
        if ( !Enable ) return;
        SetTarget();
        m_NLogger.Info(string.Format("<color={0}>{1}</color>", color, content));
    }

    [Conditional("NLOG")]
    public static void LogWarning( string content, LogColor color = LogColor.green )
    {
        if ( !Enable ) return;
        SetTarget();
        m_NLogger.Warn(string.Format("<color={0}>{1}</color>", color, content));
    }

    [Conditional("NLOG")]
    public static void LogError( string content, LogColor color = LogColor.green )
    {
        if ( !Enable ) return;
        SetTarget();
        m_NLogger.Error(string.Format("<color={0}>{1}</color>", color, content));
    }

    [Conditional("NLOG")]
    public static void LogForEach( ICollection contens, LogColor color = LogColor.green )
    {
        if ( !Enable ) return;
        SetTarget();
        foreach ( var item in contens )
        {
            m_NLogger.Error(string.Format("<color={0}>{1}</color>", color, item.ToString()));
        }
    }
}

#endif
