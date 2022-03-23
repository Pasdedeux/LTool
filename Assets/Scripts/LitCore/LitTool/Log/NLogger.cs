#if true//NOT_UNITY
using NLog;

public class NLogger : ILog
{
    private readonly Logger logger;
    public bool Enable { get; set; } = true;

    public NLogger(string name)
    {
        Enable = true;
        this.logger = LogManager.GetLogger(name);
    }

    public void Trace(string message, LogColor logColor = LogColor.white)
    {
        if (!Enable) return;
        this.logger.Trace(message);
    }

    public void Warning(string message, LogColor logColor = LogColor.white)
    {
        if (!Enable) return;
        this.logger.Warn(message);
    }

    public void Info(string message, LogColor logColor = LogColor.white)
    {
        if (!Enable) return;
        this.logger.Info(message);
    }

    public void Debug(string message, LogColor logColor = LogColor.white)
    {
        if (!Enable) return;
        this.logger.Debug(message);
    }

    public void Error(string message, LogColor logColor = LogColor.white)
    {
        if (!Enable) return;
        this.logger.Error(message);
    }

    public void Fatal(string message)
    {
        if (!Enable) return;
        this.logger.Fatal(message);
    }

    public void Trace(string message, params object[] args)
    {
        if (!Enable) return;
        this.logger.Trace(message, args);
    }

    public void Warning(string message, params object[] args)
    {
        if (!Enable) return;
        this.logger.Warn(message, args);
    }

    public void Info(string message, params object[] args)
    {
        if (!Enable) return;
        this.logger.Info(message, args);
    }

    public void Debug(string message, params object[] args)
    {
        if (!Enable) return;
        this.logger.Debug(message, args);
    }

    public void Error(string message, params object[] args)
    {
        if (!Enable) return;
        this.logger.Error(message, args);
    }

    public void Fatal(string message, params object[] args)
    {
        if (!Enable) return;
        this.logger.Fatal(message, args);
    }
}
#endif