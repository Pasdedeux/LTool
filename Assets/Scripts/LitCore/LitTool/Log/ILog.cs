public interface ILog
{
    bool Enable { get; set; }

    void Trace(string message, LogColor logColor = LogColor.green);
    void Info(string message, LogColor logColor = LogColor.green);
    void Warning(string message, LogColor logColor = LogColor.green);
    void Debug(string message, LogColor logColor = LogColor.green);
    void Error(string message, LogColor logColor = LogColor.green);


    void Trace(string message, params object[] args);
    void Warning(string message, params object[] args);
    void Info(string message, params object[] args);
    void Debug(string message, params object[] args);
    void Error(string message, params object[] args);
}