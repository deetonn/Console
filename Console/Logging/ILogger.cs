﻿namespace Console.Logging;

public interface ILogger
{
    void LogInfo(object self, string info);
    void LogError(object self, string err);
    void LogWarning(object self, string warn);
    void LogDebug(object self, string dbg);
}

public class FileLogger : ILogger
{
    private readonly string _logFile;

    public FileLogger()
    {
        _logFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Console\\saved\\log.txt";

        if (!File.Exists(_logFile))
        {
            File.Create(_logFile).Dispose();
        }
        else
        {
            File.Delete(_logFile);
            File.Create(_logFile).Dispose();
        }

        LogInfo(this, "Logger initialized!");
    }

    public void LogError(object self, string err)
    {
        LogToFile(self, $"[error]: {err}");
    }

    public void LogInfo(object self, string info)
    {
        LogToFile(self, $"[info]: {info}");
    }

    public void LogWarning(object self, string warn)
    {
        LogToFile(self, $"[warning]: {warn}");
    }

    public void LogDebug(object self, string dbg)
    {
#if DEBUG
        LogToFile(self, "[debug] " + dbg);
#endif
    }

    private void LogToFile(object self, string info)
    {
        var name = self.GetType().Name;
        try
        {
            File.AppendAllText(_logFile, $"[{name}]" + info + Environment.NewLine);
        }
        catch { }
    }
}

public static class StaticLoggerGlobalUseMe
{
    private static readonly ILogger _logger = new FileLogger();

    public static ILogger Logger() => _logger;
}
