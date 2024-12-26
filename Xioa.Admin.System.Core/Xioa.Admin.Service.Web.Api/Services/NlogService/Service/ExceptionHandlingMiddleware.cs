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
            _logger.LogError(ex, "Unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var stackTrace = exception.StackTrace;
        var controllerName = "";
        var line = "";

        // 尝试从堆栈跟踪中提取控制器名称和行号
        if (!string.IsNullOrEmpty(stackTrace))
        {
            var firstLine = stackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.None).FirstOrDefault();
            if (firstLine != null)
            {
                var regex = new Regex(@"at\s+(.*?)\s+in\s+(.*?):line\s+(\d+)");
                var match = regex.Match(firstLine);
                if (match.Success)
                {
                    controllerName = match.Groups[1].Value;  // 获取方法全名
                    line = match.Groups[3].Value;           // 获取行号
                }
            }
        }

        var result = JsonSerializer.Serialize(new
        {
            error = "An unexpected error occurred.",
            detail = exception.Message,
            controller = controllerName,
            line = line
        });

        await context.Response.WriteAsync(result);
    }
    
    // private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    // {
    //     context.Response.ContentType = "application/json";
    //     context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    //
    //     var result = JsonSerializer.Serialize(new { error = "An unexpected error occurred." });
    //     return context.Response.WriteAsync(result);
    // }
}