#if !NOT_UNITY
using System;

public class UnityLogger : ILog
{
    private bool _enabeld = true;
    public bool Enable
    {
        get { return _enabeld; }
        set
        {
            _enabeld = value;
            UnityEngine.Debug.unityLogger.logEnabled = value;
        }
    }

    public void Trace(string msg, LogColor logColor = LogColor.green)
    {
        UnityEngine.Debug.LogFormat("<color={0}>{1}</color>", logColor, msg);
    }

    public void Debug(string msg, LogColor logColor = LogColor.green)
    {
        UnityEngine.Debug.LogFormat("<color={0}>{1}</color>", logColor, msg);
    }

    public void Info(string msg, LogColor logColor = LogColor.green)
    {
        UnityEngine.Debug.LogFormat("<color={0}>{1}</color>", logColor, msg);
    }

    public void Warning(string msg, LogColor logColor = LogColor.green)
    {
        UnityEngine.Debug.LogWarningFormat("<color={0}>{1}</color>", logColor, msg);
    }

    public void Error(string msg, LogColor logColor = LogColor.red)
    {
        UnityEngine.Debug.LogErrorFormat("<color={0}>{1}</color>", logColor, msg);
    }

    public void Error(Exception e)
    {
        UnityEngine.Debug.LogException(e);
    }

    public void Trace(string message, params object[] args)
    {
        UnityEngine.Debug.LogFormat(message, args);
    }

    public void Warning(string message, params object[] args)
    {
        UnityEngine.Debug.LogWarningFormat(message, args);
    }

    public void Info(string message, params object[] args)
    {
        UnityEngine.Debug.LogFormat(message, args);
    }

    public void Debug(string message, params object[] args)
    {
        UnityEngine.Debug.LogFormat(message, args);
    }

    public void Error(string message, params object[] args)
    {
        UnityEngine.Debug.LogErrorFormat(message, args);
    }
}
#endif