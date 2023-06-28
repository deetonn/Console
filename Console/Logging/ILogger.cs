
namespace Console.Logging;

public interface ILogger
{
    void LogInfo(object self, string info);
    void LogError(object self, string err);
    void LogWarning(object self, string warn);
}

public class FileLogger : ILogger
{
    private readonly string _logFile;

    public FileLogger()
    {
        _logFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\saved\\log.txt";

        if (!File.Exists(_logFile))
        {
            File.Create(_logFile).Dispose();
        }
        else
        {
            File.Delete(_logFile);
            File.Create(_logFile).Dispose();
        }
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

    private void LogToFile(object self, string info)
    {
        var name = self.GetType().Name;
        File.AppendAllText(_logFile, $"[{name}]" + info + Environment.NewLine);
    }
}

public static class StaticLoggerGlobalUseMe
{
    private static readonly ILogger _logger = new FileLogger();

    public static ILogger Logger() => _logger;
}
