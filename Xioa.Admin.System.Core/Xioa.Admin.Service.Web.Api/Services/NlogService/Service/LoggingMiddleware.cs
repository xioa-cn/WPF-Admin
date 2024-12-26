namespace Xioa.Admin.Service.Web.Api.Services.NlogService.Service;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 记录请求信息
        _logger.LogInformation($"Incoming request: {context.Request.Method} {context.Request.Path}");

        // 获取请求IP地址
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        _logger.LogInformation($"Request from IP: {ipAddress}");

        // 记录请求参数（对于GET请求）
        if (context.Request.Method == "GET")
        {
            var queryParams = context.Request.QueryString.ToString();
            _logger.LogInformation($"Query parameters: {queryParams}");
        }

        // 拦截响应
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        // 记录响应内容
        _logger.LogInformation($"Response: {text}");

        await responseBody.CopyToAsync(originalBodyStream);
    }
}