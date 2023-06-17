using System.Diagnostics.CodeAnalysis;

namespace Snek.Infrastructure;

public class FileLogger
{
    private string _path;
    private string _fileName;
    private LogLevel[] _logLevels;
    private readonly LogLevel[] _allLogLevels = new[] { LogLevel.Debug, LogLevel.Info, LogLevel.Error };

    public FileLogger(params LogLevel[] logLevels)
    {
        Initialize(logLevels);
    }

    public void Log(LogLevel logLevel, string message, object[] parameters)
    {
        if (!_logLevels.Contains(logLevel)) return;
        Directory.CreateDirectory(_path);

        var filePath = Path.Join(_path, _fileName);
        var logMessage = PrepareLogMessage(logLevel, message, parameters);

        File.AppendAllLines(filePath, new[] { logMessage });
    }

    public void LogDebug(string message, params object[] parameters)
        => Log(LogLevel.Debug, message, parameters);

    public void LogInfo(string message, params object[] parameters)
        => Log(LogLevel.Info, message, parameters);

    public void LogError(string message, params object[] parameters)
        => Log(LogLevel.Error, message, parameters);

    private static string PrepareLogMessage(LogLevel logLevel, string message, params object[] parameters)
        => $"{DateTime.Now:yyyy-MM-dd_HH:mm:ss.fffff} | {logLevel} | {message} | {string.Join(",", parameters.Select(p => p.ToString()).ToArray())}";

    [MemberNotNull(nameof(_path)), MemberNotNull(nameof(_fileName)), MemberNotNull(nameof(_logLevels))]
    public void Initialize(params LogLevel[] logLevels)
    {
        _logLevels = logLevels.Any() ? logLevels : _allLogLevels;

        _path = Path.Join(
            Directory.GetCurrentDirectory(),
            "Logs",
            DateTime.Now.ToString("yyyy-MM-dd"));

        _fileName = $"{DateTime.Now:HH:mm:ss}.txt";
    }
}