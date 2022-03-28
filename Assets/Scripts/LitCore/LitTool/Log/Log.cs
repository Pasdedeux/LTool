using System;
using System.Diagnostics;
using System.IO;
using System.Net;

#if NOT_UNITY
using NLog;
#endif

public static class Log
{
    public static ILog ILog { get; set; }

    private const int TraceLevel = 1;
    private const int DebugLevel = 2;
    private const int InfoLevel = 3;
    private const int WarningLevel = 4;

    public static bool Enable
    {
        get { return ILog.Enable; }
        set { ILog.Enable = value; }
    }

    private static bool CheckLogLevel(int level)
    {
        return Options.Instance.LogLevel <= level;
    }

    [Conditional("LOG")]
    public static void Trace(string msg, LogColor logColor = LogColor.green)
    {
        if (!Enable) return;
        if (!CheckLogLevel(DebugLevel))
        {
            return;
        }
        StackTrace st = new StackTrace(1, true);
        ILog.Trace($"{msg}\n{st}",logColor);
    }

    [Conditional("LOG")]
    public static void Debug(string msg, LogColor logColor = LogColor.green)
    {
        if (!Enable) return;
        if (!CheckLogLevel(DebugLevel))
        {
            return;
        }
        ILog.Debug(msg, logColor);
    }

    [Conditional("LOG")]
    public static void Info(string msg, LogColor logColor = LogColor.green)
    {
        if (!Enable) return;
        if (!CheckLogLevel(InfoLevel))
        {
            return;
        }
        ILog.Info(msg, logColor);
    }

    [Conditional("LOG")]
    public static void TraceInfo(string msg, LogColor logColor = LogColor.green)
    {
        if (!Enable) return;
        if (!CheckLogLevel(InfoLevel))
        {
            return;
        }
        StackTrace st = new StackTrace(1, true);
        ILog.Trace($"{msg}\n{st}", logColor);
    }

    [Conditional("LOG")]
    public static void Warning(string msg, LogColor logColor = LogColor.green)
    {
        if (!Enable) return;
        if (!CheckLogLevel(WarningLevel))
        {
            return;
        }

        ILog.Warning(msg, logColor);
    }

    [Conditional("LOG")]
    public static void Error(string msg, LogColor logColor = LogColor.green)
    {
        if (!Enable) return;
        StackTrace st = new StackTrace(1, true);
        ILog.Error($"{msg}\n{st}", logColor);
    }

    //[Conditional("LOG")]
    public static void Error(Exception e)
    {
        if (!Enable) return;
        if (e.Data.Contains("StackTrace"))
        {
            ILog.Error($"{e.Data["StackTrace"]}\n{e}");
            return;
        }
        string str = e.ToString();
        ILog.Error(str);
    }

    [Conditional("LOG")]
    public static void Trace(string message, params object[] args)
    {
        if (!Enable) return;
        if (!CheckLogLevel(TraceLevel))
        {
            return;
        }
        StackTrace st = new StackTrace(1, true);
        ILog.Trace($"{string.Format(message, args)}\n{st}");
    }

    [Conditional("LOG")]
    public static void Warning(string message, params object[] args)
    {
        if (!Enable) return;
        if (!CheckLogLevel(WarningLevel))
        {
            return;
        }
        ILog.Warning(string.Format(message, args));
    }

    [Conditional("LOG")]
    public static void Info(string message, params object[] args)
    {
        if (!Enable) return;
        if (!CheckLogLevel(InfoLevel))
        {
            return;
        }
        ILog.Info(string.Format(message, args));
    }

    [Conditional("LOG")]
    public static void Debug(string message, params object[] args)
    {
        if (!Enable) return;
        if (!CheckLogLevel(DebugLevel))
        {
            return;
        }
        ILog.Debug(string.Format(message, args));

    }

    [Conditional("LOG")]
    public static void Error(string message, params object[] args)
    {
        if (!Enable) return;
        StackTrace st = new StackTrace(1, true);
        string s = string.Format(message, args) + '\n' + st;
        ILog.Error(s);
    }

    [Conditional("LOG")]
    public static void Console(string message)
    {
        if (!Enable) return;
        if (Options.Instance.Console == 1)
        {
            System.Console.WriteLine(message);
        }
        ILog.Debug(message);
    }

    [Conditional("LOG")]
    public static void Console(string message, params object[] args)
    {
        if (!Enable) return;
        string s = string.Format(message, args);
        if (Options.Instance.Console == 1)
        {
            System.Console.WriteLine(s);
        }
        ILog.Debug(s);
    }
}

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