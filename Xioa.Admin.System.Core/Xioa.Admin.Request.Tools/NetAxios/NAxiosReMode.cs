using System.Net.Sockets;
using System.Text;

namespace Xioa.Admin.Request.Tools.NetAxios;

public partial class NAxios {
    /// <summary>
    /// 克隆HTTP请求消息
    /// </summary>
    /// <param name="request">原始请求</param>
    /// <returns>克隆的请求</returns>
    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri)
        {
            Version = request.Version
        };

        // 复制请求头
        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        // 复制请求内容
        if (request.Content == null) return clone;
        {
            var content = await request.Content.ReadAsStringAsync();
            clone.Content = new StringContent(content, Encoding.UTF8, request.Content.Headers.ContentType?.MediaType);
            
            // 复制内容头
            foreach (var header in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return clone;
    }
    
    /// <summary>
    /// 判断是否需要重试请求
    /// </summary>
    /// <param name="exception">捕获的异常</param>
    /// <param name="retryCount">当前重试次数</param>
    /// <returns>是否需要重试</returns>
    private bool ShouldRetry(Exception exception, int retryCount)
    {
        // 如果超过最大重试次数，不再重试
        if (retryCount >= _config.RetryCount)
        {
            return false;
        }

        // 如果配置了自定义重试条件，使用自定义条件
        if (_config.RetryCondition != null)
        {
            return _config.RetryCondition(exception, retryCount);
        }

        // 默认重试条件
        return exception switch
        {
            HttpRequestException httpException => 
                // 5xx 服务器错误
                httpException.StatusCode >= System.Net.HttpStatusCode.InternalServerError ||
                // 408 请求超时
                httpException.StatusCode == System.Net.HttpStatusCode.RequestTimeout ||
                // 429 太多请求
                httpException.StatusCode == System.Net.HttpStatusCode.TooManyRequests,
            TaskCanceledException => true, // 超时
            SocketException => true,       // 网络连接错误
            IOException => true,           // IO错误（通常是网络相关）
            _ => false                     // 其他错误不重试
        };
    }
}