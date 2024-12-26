using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Xioa.Admin.Service.Web.Api.Services.NlogService.Service;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var (fileName, lineNumber, methodName) = GetExceptionDetails(ex);
            
            // 构建详细的错误日志信息
            var errorMessage = 
                $"[异常信息]"
                + $"\n请求路径: {context.Request.Path}"
                + $"\n异常消息: {ex.Message}"
                + $"\n文件名: {fileName}"
                + $"\n行号: {lineNumber}"
                + $"\n方法名: {methodName}"
                + $"\n堆栈跟踪: {ex.StackTrace}"
                ;
            
            // 改用 LogError 记录异常
            _logger.LogError(errorMessage);
            await HandleExceptionAsync(context, ex, fileName, lineNumber, methodName);
        }
    }

    private (string fileName, string lineNumber, string methodName) GetExceptionDetails(Exception ex)
    {
        var fileName = "未知文件";
        var lineNumber = "0";
        var methodName = "未知方法";

        if (ex.StackTrace != null)
        {
            // 获取第一行堆栈信息（最接近异常发生点）
            var stackFrames = ex.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var frame in stackFrames)
            {
                // 匹配包含文件信息的堆栈行
                var match = Regex.Match(frame, @"at (.*?) in (.*?):line (\d+)");
                if (match.Success)
                {
                    methodName = match.Groups[1].Value.Trim();
                    fileName = Path.GetFileName(match.Groups[2].Value.Trim());
                    lineNumber = match.Groups[3].Value.Trim();
                    break;
                }
            }
        }

        return (fileName, lineNumber, methodName);
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, 
        string fileName, string lineNumber, string methodName)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var result = JsonSerializer.Serialize(new
        {
            success = false,
            code = context.Response.StatusCode,
            message = "服务器发生错误",
            error = new
            {
                message = exception.Message,
                fileName = fileName,
                lineNumber = lineNumber,
                methodName = methodName,
                stackTrace = exception.StackTrace
            }
        }, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

        await context.Response.WriteAsync(result);
    }
}
