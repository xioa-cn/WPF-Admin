using System;

namespace Xioa.Admin.Core.Services.Logging;

/// <summary>
/// 日志条目类，用于存储单条日志的完整信息
/// </summary>
public class LogEntry
{
    /// <summary>
    /// 日志记录的时间戳
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 日志级别，用于区分日志的重要程度
    /// </summary>
    public LogLevel Level { get; set; }

    /// <summary>
    /// 日志消息内容
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 日志来源，通常包含类名、方法名和行号
    /// 格式：类名.方法名 (文件名:行号)
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// 记录日志的用户名
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 异常信息，当记录错误日志时使用
    /// </summary>
    public Exception Exception { get; set; }

    /// <summary>
    /// 附加信息，用于存储任何额外的上下文信息
    /// </summary>
    public string AdditionalInfo { get; set; }
} 