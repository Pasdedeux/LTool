using System.IO;

public class FileLogger : ILog
{
    private readonly StreamWriter stream;
    public bool Enable { get; set; } = true;

    public FileLogger(string path)
    {
        Enable = true;
        this.stream = new StreamWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite));
        this.stream.AutoFlush = true;
    }

    public void Trace(string message, LogColor logColor = LogColor.green)
    {
        if (!Enable) return;
        this.stream.WriteLine(message);
        this.stream.Flush();
    }

    public void Warning(string message, LogColor logColor = LogColor.green)
    {
        if (!Enable) return;
        this.stream.WriteLine(message);
        this.stream.Flush();
    }

    public void Info(string message, LogColor logColor = LogColor.green)
    {
        if (!Enable) return;
        this.stream.WriteLine(message);
        this.stream.Flush();
    }

    public void Debug(string message, LogColor logColor = LogColor.green)
    {
        if (!Enable) return;
        this.stream.WriteLine(message);
        this.stream.Flush();
    }

    public void Error(string message, LogColor logColor = LogColor.green)
    {
        if (!Enable) return;
        this.stream.WriteLine(message);
        this.stream.Flush();
    }

    public void Trace(string message, params object[] args)
    {
        if (!Enable) return;
        this.stream.WriteLine(message, args);
        this.stream.Flush();
    }

    public void Warning(string message, params object[] args)
    {
        if (!Enable) return;
        this.stream.WriteLine(message, args);
        this.stream.Flush();
    }

    public void Info(string message, params object[] args)
    {
        if (!Enable) return;
        this.stream.WriteLine(message, args);
        this.stream.Flush();
    }

    public void Debug(string message, params object[] args)
    {
        if (!Enable) return;
        this.stream.WriteLine(message, args);
        this.stream.Flush();
    }

    public void Error(string message, params object[] args)
    {
        if (!Enable) return;
        this.stream.WriteLine(message, args);
        this.stream.Flush();
    }

    public void Fatal(string message, params object[] args)
    {
        if (!Enable) return;
        this.stream.WriteLine(message, args);
        this.stream.Flush();
    }
}
