using System.Text;
using System.Text.Json;

namespace Xioa.Admin.Service.Web.Api.Services.NlogService.Service;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;
    private readonly JsonSerializerOptions? _jsonOptions;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        
        // // 输出logger的类别名称
        // var loggerType = logger.GetType().GetGenericArguments()[0].FullName;
        // Console.WriteLine($"Logger category name: {loggerType}");
        
        _jsonOptions = new JsonSerializerOptions 
        { 
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.Now;
        var requestInfo = await FormatRequest(context.Request);
        
        // 记录请求信息
        var requestLog = $"[{context.Request.Method}请求]\n{JsonSerializer.Serialize(requestInfo, _jsonOptions)}";
        _logger.LogInformation(requestLog);

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
            var responseInfo = await FormatResponse(context.Response);
            var duration = (DateTime.Now - startTime).TotalMilliseconds;

            var responseLog = 
                $"[响应信息]"
                + $"\n请求路径: {context.Request.Path}"
                + $"\n耗时: {duration:F2}ms"
                + $"\n状态码: {context.Response.StatusCode}"
                + $"\n响应内容: \n{JsonSerializer.Serialize(responseInfo, _jsonOptions)}"
                ;

            _logger.LogInformation(responseLog);
        }
        finally
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private async Task<Dictionary<string, object?>> FormatRequest(HttpRequest request)
    {
        var requestInfo = new Dictionary<string, object?>();

        // 基本信息
        requestInfo["IP"] = request.Headers["X-Forwarded-For"].FirstOrDefault() 
            ?? request.HttpContext.Connection.RemoteIpAddress?.ToString() 
            ?? "unknown";
        requestInfo["Path"] = request.Path.ToString();
        requestInfo["Protocol"] = request.Protocol;

        // Headers
        var headers = new Dictionary<string, string>();
        foreach (var header in request.Headers)
        {
            headers[header.Key] = header.Value.ToString();
        }
        requestInfo["Headers"] = headers;

        // Query参数
        if (request.QueryString.HasValue)
        {
            var queryParams = new Dictionary<string, string>();
            foreach (var query in request.Query)
            {
                queryParams[query.Key] = query.Value.ToString();
            }
            requestInfo["QueryParams"] = queryParams;
        }

        // Body参数
        if (request.Method != "GET" && request.ContentLength > 0)
        {
            request.EnableBuffering();
            var bodyStr = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Position = 0;

            try
            {
                var jsonDoc = JsonDocument.Parse(bodyStr);
                requestInfo["Body"] = JsonSerializer.Deserialize<Dictionary<string, object>>(bodyStr);
            }
            catch
            {
                requestInfo["Body"] = bodyStr;
            }
        }

        return requestInfo;
    }

    private async Task<object> FormatResponse(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        var responseInfo = new Dictionary<string, object?>();

        // Headers
        var headers = new Dictionary<string, string>();
        foreach (var header in response.Headers)
        {
            headers[header.Key] = header.Value.ToString();
        }
        responseInfo["Headers"] = headers;

        // Body
        if (!string.IsNullOrEmpty(responseText))
        {
            try
            {
                var jsonDoc = JsonDocument.Parse(responseText);
                responseInfo["Body"] = JsonSerializer.Deserialize<Dictionary<string, object>>(responseText);
            }
            catch
            {
                responseInfo["Body"] = responseText;
            }
        }

        return responseInfo;
    }
}