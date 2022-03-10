#region <<版本信息>>
/*======================================
* 项目名称 ：Assets.Scripts.LitCore.LitTool.Log
* 项目描述 ：
* 类 名 称 ：ILog
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.LitCore.LitTool.Log
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2022/3/9 16:22:56
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2022. All rights reserved.
*******************************************************************
======================================*/
#endregion

using System.Collections;

internal interface ILog
{
    bool Enable { get; set; }

    void Log(object content);

    void LogWarning(object content);

    void LogError(object content);

    void LogFormat(string content, params object[] args);

    void LogWarningFormat(string content, params object[] args);


    void LogErrorFormat(string content, params object[] args);

    void Log(string content, LogColor color = LogColor.green);

    void LogWarning(string content, LogColor color = LogColor.yellow);

    void LogError(string content, LogColor color = LogColor.red);

    void LogForEach(ICollection contens, LogColor color = LogColor.green);
}
