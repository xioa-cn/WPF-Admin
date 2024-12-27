using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using Xioa.Admin.Request.Tools.NetAxios;

namespace TestWebService;

public class NAxiosTest {
    [Fact]
    public async Task TestMethod1() {
        // 创建实例
        IAxios axios = new NAxios(new NAxiosConfig
        {
            BaseUrl = "https://localhost:7078/api/",
            RetryCount = 3,  // 最多重试3次
            RetryDelay = 1000,  // 每次重试间隔1秒
            RetryCondition = (exception, retryCount) =>
            {
                var res= exception switch
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
                return res;
            }
        });
        
        axios.AddRequestInterceptor(async (request) =>
        {
            // 添加认证头
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "your-token");
    
            // 添加自定义头
            request.Headers.Add("X-Custom-Header", "custom-value");
    
            // 可以修改请求URL
            if (!request.RequestUri.AbsolutePath.StartsWith("/api"))
            {
                var newUri = new Uri(request.RequestUri.AbsoluteUri.Replace(
                    request.RequestUri.AbsolutePath,
                    "/api" + request.RequestUri.AbsolutePath));
                request.RequestUri = newUri;
            }
    
            return request;
        });
        
        axios.AddResponseInterceptor(async (response) =>
        {
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // 处理未授权情况，例如刷新token
                // throw new UnauthorizedException();
            }
            
            if (response.Content != null)
            {
                var content = await response.Content.ReadAsStringAsync();
                // 处理响应内容...
                response.Content = new StringContent(content);
            }
    
            return response;
        });


        try
        {
            var response = await axios.GetAsync("/LoggerTest/TestLogger");
            
        }
        catch (Exception ex)
        {
            
        }
    }
    
    public class TestAxios {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}