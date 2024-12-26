using System;

namespace Xioa.Admin.Core.Services.Logging;

/// <summary>
/// 日志服务接口，提供不同级别的日志记录功能
/// </summary>
public interface ILogService
{
    /// <summary>
    /// 记录指定级别的日志
    /// </summary>
    /// <param name="level">日志级别</param>
    /// <param name="message">日志消息</param>
    /// <param name="exception">异常信息（可选）</param>
    /// <param name="source">资源信息</param>
    public void Log(LogLevel level, string message, Exception? exception = null, string? source = "");

    /// <summary>
    /// 记录调试信息，用于开发和调试阶段
    /// </summary>
    /// <param name="message">调试信息</param>
    void LogDebug(string message);

    /// <summary>
    /// 记录一般信息，用于记录应用程序的正常运行状态
    /// </summary>
    /// <param name="message">信息内容</param>
    void LogInfo(string message);

    /// <summary>
    /// 记录警告信息，表示可能存在的问题，但不影响系统运行
    /// </summary>
    /// <param name="message">警告信息</param>
    void LogWarning(string message);

    /// <summary>
    /// 记录错误信息，表示发生了错误但系统可以继续运行
    /// </summary>
    /// <param name="message">错误信息</param>
    /// <param name="ex">异常对象（可选）</param>
    void LogError(string message, Exception ex = null);

    /// <summary>
    /// 记录致命错误，表示系统发生了严重错误可能需要终止运行
    /// </summary>
    /// <param name="message">错误信息</param>
    /// <param name="ex">异常对象（可选）</param>
    void LogFatal(string message, Exception ex = null);

    /// <summary>
    /// 记录用户操作日志
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="action">操作名称</param>
    /// <param name="details">操作详情</param>
    void LogUserAction(string userName, string action, string details);
} 