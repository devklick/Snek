using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Snek.Core.Infrastructure;

public class FileLogger
{
    private string _path;
    private string _fileName;
    private LogLevel[] _logLevels;
    private readonly LogLevel[] _allLogLevels = new[] { LogLevel.Debug, LogLevel.Info, LogLevel.Error };
    private readonly bool _enabled;

    public FileLogger(bool enabled, params LogLevel[] logLevels)
    {
        _enabled = enabled;
        Initialize(logLevels);
    }

    public void Log(LogLevel logLevel, string eventType, string message, object[] parameters)
    {
        if (!_enabled || !_logLevels.Contains(logLevel)) return;
        Directory.CreateDirectory(_path);

        var filePath = Path.Join(_path, _fileName);
        var logMessage = PrepareLogMessage(logLevel, eventType, message, parameters);

        File.AppendAllLines(filePath, new[] { logMessage });
    }

    public void LogDebug(string eventType, string message, params object[] parameters)
        => Log(LogLevel.Debug, eventType, message, parameters);

    public void LogInfo(string eventType, string message, params object[] parameters)
        => Log(LogLevel.Info, eventType, message, parameters);

    public void LogError(string eventType, string message, params object[] parameters)
        => Log(LogLevel.Error, eventType, message, parameters);

    private static string PrepareLogMessage(LogLevel logLevel, string eventType, string message, params object[] parameters)
        => $"{DateTime.Now:yyyy-MM-dd_HH:mm:ss.fffff} | {logLevel} | {eventType} | {message} | {string.Join(",", parameters.Select(p => p.ToString()).ToArray())}";

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