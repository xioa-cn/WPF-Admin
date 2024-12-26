using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Xioa.Admin.Core.Services.Logging;

/// <summary>
/// 日志服务实现类，提供按级别分类的日志记录功能
/// 日志按照 Logs/日期/级别/序号.log 的方式组织
/// </summary>
public class LogService : ILogService
{
    /// <summary>
    /// 单例实例
    /// </summary>
    public static readonly LogService Instance = new LogService();

    /// <summary>
    /// 日志根目录
    /// </summary>
    private readonly string _baseLogDirectory;

    /// <summary>
    /// 用于线程同步的锁对象
    /// </summary>
    private readonly object _lockObj = new object();

    /// <summary>
    /// 单个日志文件的最大大小（MB）
    /// </summary>
    private readonly int _maxFileSizeInMB;

    /// <summary>
    /// 每个级别最多保留的日志文件数
    /// </summary>
    private readonly int _maxLogFiles;

    /// <summary>
    /// 私有构造函数，确保单例模式
    /// </summary>
    /// <param name="logDirectory">日志根目录</param>
    /// <param name="maxFileSizeInMB">单个文件最大大小（MB）</param>
    /// <param name="maxLogFiles">每个级别最多保留的文件数</param>
    private LogService(string logDirectory = "XLogs", int maxFileSizeInMB = 100, int maxLogFiles = 5)
    {
        _baseLogDirectory = logDirectory;
        _maxFileSizeInMB = maxFileSizeInMB;
        _maxLogFiles = maxLogFiles;

        Directory.CreateDirectory(_baseLogDirectory);
    }

    /// <summary>
    /// 获取日志文件路径
    /// </summary>
    /// <param name="level">日志级别</param>
    /// <returns>可用的日志文件路径</returns>
    /// <remarks>
    /// 1. 按日期和级别创建目录
    /// 2. 检查文件大小，必要时创建新文件
    /// 3. 维护文件数量，超出限制时删除最旧的文件
    /// </remarks>
    private string GetLogFilePath(LogLevel level)
    {
        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        string levelDirectory = Path.Combine(_baseLogDirectory, currentDate, level.ToString());

        // 确保目录存在
        if (!Directory.Exists(levelDirectory))
        {
            Directory.CreateDirectory(levelDirectory);
        }

        // 获取当前日志文件列表
        var logFiles = Directory.GetFiles(levelDirectory, "*.log")
            .OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f)))
            .ToList();

        // 如果没有文件，创建一个文件
        if (!logFiles.Any())
        {
            return Path.Combine(levelDirectory, "1.log");
        }

        // 检查当前文件大小
        string currentFile = logFiles.Last();
        if (new FileInfo(currentFile).Length >= _maxFileSizeInMB * 1024 * 1024)
        {
            int nextNumber = logFiles.Count + 1;
            if (nextNumber > _maxLogFiles)
            {
                // 超出文件数限制，删除最旧的并重命名其他文件
                File.Delete(logFiles.First());
                for (int i = 1; i < logFiles.Count; i++)
                {
                    File.Move(logFiles[i], Path.Combine(levelDirectory, $"{i}.log"));
                }

                nextNumber = logFiles.Count;
            }

            return Path.Combine(levelDirectory, $"{nextNumber}.log");
        }

        return currentFile;
    }

    /// <summary>
    /// 写入日志条目
    /// </summary>
    /// <param name="entry">日志条目</param>
    /// <remarks>
    /// 使用锁确保线程安全
    /// 写入失败时尝试写入Windows事件日志
    /// </remarks>
    private void WriteLogEntry(LogEntry entry)
    {
        var logMessage = FormatLogEntry(entry);

        lock (_lockObj)
        {
            try
            {
                string logFilePath = GetLogFilePath(entry.Level);
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                try
                {
                    System.Diagnostics.EventLog.WriteEntry("Xioa.Admin",
                        $"Failed to write log: {ex.Message}\nOriginal log: {logMessage}",
                        System.Diagnostics.EventLogEntryType.Error);
                }
                catch
                {
                }
            }
        }
    }

    /// <summary>
    /// 格式化日志条目
    /// </summary>
    /// <param name="entry">日志条目</param>
    /// <returns>格式化后的日志文本</returns>
    private string FormatLogEntry(LogEntry entry)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{entry.Level}] {entry.Message}");

        if (!string.IsNullOrEmpty(entry.UserName))
            sb.AppendLine($"User: {entry.UserName}");

        if (entry.Exception != null)
        {
            sb.AppendLine($"Exception: {entry.Exception.GetType().Name}");
            sb.AppendLine($"Message: {entry.Exception.Message}");
            sb.AppendLine($"StackTrace: {entry.Exception.StackTrace}");
        }

        sb.AppendLine($"Source:{entry.Source}");
        if (!string.IsNullOrEmpty(entry.AdditionalInfo))
            sb.AppendLine($"Additional Info: {entry.AdditionalInfo}");

        return sb.ToString();
    }

    public void Log(LogLevel level, string message, Exception? exception = null, string source = "")
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);
            var callerFrame = stackTrace.GetFrame(1); // 跳过当前方法
            var callerMethod = callerFrame?.GetMethod();
            var callerType = callerMethod?.DeclaringType;
            source = $"{callerType?.Name}.{callerMethod?.Name} " +
                     $"({Path.GetFileName(callerFrame?.GetFileName())}:{callerFrame?.GetFileLineNumber()})";
        }

        var logEntry = new LogEntry
        {
            Timestamp = DateTime.Now,
            Level = level,
            Message = message,
            // 格式化调用源信息：类名.方法名 (文件名:行号)
            Source = source,
            UserName = Environment.UserName,
            Exception = exception,
            AdditionalInfo = exception?.StackTrace
        };

        WriteLogEntry(logEntry);
    }

    public void LogDebug(string message)
    {
        var stackTrace = new System.Diagnostics.StackTrace(true);
        var callerFrame = stackTrace.GetFrame(1); // 跳过当前方法
        var callerMethod = callerFrame?.GetMethod();
        var callerType = callerMethod?.DeclaringType;
        var source = $"{callerType?.Name}.{callerMethod?.Name} " +
                     $"({Path.GetFileName(callerFrame?.GetFileName())}:{callerFrame?.GetFileLineNumber()})";
        Log(LogLevel.Debug, message, null, source);
    }

    public void LogInfo(string message)
    {
        var stackTrace = new System.Diagnostics.StackTrace(true);
        var callerFrame = stackTrace.GetFrame(1); // 跳过当前方法
        var callerMethod = callerFrame?.GetMethod();
        var callerType = callerMethod?.DeclaringType;
        var source = $"{callerType?.Name}.{callerMethod?.Name} " +
                     $"({Path.GetFileName(callerFrame?.GetFileName())}:{callerFrame?.GetFileLineNumber()})";
        Log(LogLevel.Info, message, null, source);
    }

    public void LogWarning(string message)
    {
        var stackTrace = new System.Diagnostics.StackTrace(true);
        var callerFrame = stackTrace.GetFrame(1); // 跳过当前方法
        var callerMethod = callerFrame?.GetMethod();
        var callerType = callerMethod?.DeclaringType;
        var source = $"{callerType?.Name}.{callerMethod?.Name} " +
                     $"({Path.GetFileName(callerFrame?.GetFileName())}:{callerFrame?.GetFileLineNumber()})";
        Log(LogLevel.Warning, message, null, source);
    }

    public void LogError(string message, Exception ex = null)
    {
        var stackTrace = new System.Diagnostics.StackTrace(true);
        var callerFrame = stackTrace.GetFrame(1); // 跳过当前方法
        var callerMethod = callerFrame?.GetMethod();
        var callerType = callerMethod?.DeclaringType;
        var source = $"{callerType?.Name}.{callerMethod?.Name} " +
                     $"({Path.GetFileName(callerFrame?.GetFileName())}:{callerFrame?.GetFileLineNumber()})";
        Log(LogLevel.Error, message, ex, source);
    }

    public void LogFatal(string message, Exception ex = null)
    {
        var stackTrace = new System.Diagnostics.StackTrace(true);
        var callerFrame = stackTrace.GetFrame(1); // 跳过当前方法
        var callerMethod = callerFrame?.GetMethod();
        var callerType = callerMethod?.DeclaringType;
        var source = $"{callerType?.Name}.{callerMethod?.Name} " +
                     $"({Path.GetFileName(callerFrame?.GetFileName())}:{callerFrame?.GetFileLineNumber()})";
        Log(LogLevel.Fatal, message, ex, source);
    }

    public void LogUserAction(string userName, string action, string details)
    {
        var stackTrace = new System.Diagnostics.StackTrace(true);
        var callerFrame = stackTrace.GetFrame(1); // 跳过当前方法
        var callerMethod = callerFrame?.GetMethod();
        var callerType = callerMethod?.DeclaringType;
        var source = $"{callerType?.Name}.{callerMethod?.Name} " +
                     $"({Path.GetFileName(callerFrame?.GetFileName())}:{callerFrame?.GetFileLineNumber()})";
        var message = $"User Action: {action} - {details}";
        Log(LogLevel.Info, message, null, source);
    }
}