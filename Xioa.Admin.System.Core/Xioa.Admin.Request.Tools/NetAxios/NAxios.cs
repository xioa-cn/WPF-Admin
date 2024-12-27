using System.Text;
using System.Text.Json;

namespace Xioa.Admin.Request.Tools.NetAxios;

public partial class NAxios : IAxios {
    private static readonly JsonSerializerOptions JsonOptions = new() {
        PropertyNameCaseInsensitive = true, // 不区分大小写
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // 驼峰命名
        WriteIndented = false // 不需要缩进，减少数据大小
    };

    private readonly HttpClient _httpClient;
    private readonly NAxiosConfig _config;
    private readonly List<Func<HttpRequestMessage, Task<HttpRequestMessage>>> _requestInterceptors = new();
    private readonly List<Func<HttpResponseMessage, Task<HttpResponseMessage>>> _responseInterceptors = new();

    public NAxios(NAxiosConfig? config = null) {
        _config = config ?? new NAxiosConfig();
        _httpClient = new HttpClient();

        if (!string.IsNullOrEmpty(_config.BaseUrl))
        {
            _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        }

        _httpClient.Timeout = TimeSpan.FromMilliseconds(_config.Timeout);

        // 设置默认请求头
        foreach (var header in _config.Headers)
        {
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }

    #region 私有辅助方法

    /// <summary>
    /// 构建完整的URL
    /// </summary>
    /// <param name="url">相对或绝对URL</param>
    /// <returns>完整的URL</returns>
    public string BuildUrl(string url) {
        if (Uri.TryCreate(url, UriKind.Absolute, out _))
        {
            return url;
        }

        url = url.TrimStart('/');

        if (!string.IsNullOrEmpty(_config.BaseUrl))
        {
            var baseUrl = _config.BaseUrl.TrimEnd('/');
            return $"{baseUrl}/{url}";
        }

        return url;
    }

    /// <summary>
    /// 将对象参数转换为URL查询字符串并附加到URL
    /// </summary>
    /// <param name="url">原始URL</param>
    /// <param name="parameters">查询参数对象</param>
    /// <returns>带有查询参数的完整URL</returns>
    private static string AppendQueryParameters(string url, object? parameters) {
        if (parameters == null) return url;

        var queryString = string.Join("&", parameters.GetType()
            .GetProperties()
            .Select(p =>
                $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(p.GetValue(parameters)?.ToString() ?? string.Empty)}"));

        return url.Contains("?") ? $"{url}&{queryString}" : $"{url}?{queryString}";
    }

    /// <summary>
    /// 创建JSON格式的HTTP内容
    /// </summary>
    /// <param name="data">要序列化的数据对象</param>
    /// <returns>JSON格式的StringContent</returns>
    private static StringContent CreateJsonContent(object? data) {
        var json = data == null ? "{}" : JsonSerializer.Serialize(data, JsonOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    /// <summary>
    /// 将HTTP响应内容反序列化为指定类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="response">HTTP响应消息</param>
    /// <returns>反序列化后的对象</returns>
    public static async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response) {
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, JsonOptions);
    }

    /// <summary>
    /// 应用请求拦截器链
    /// </summary>
    /// <param name="request">原始请求</param>
    /// <returns>处理后的请求</returns>
    private async Task<HttpRequestMessage> ApplyRequestInterceptors(HttpRequestMessage request) {
        var currentRequest = request;
        foreach (var interceptor in _requestInterceptors)
        {
            currentRequest = await interceptor(currentRequest);
        }

        return currentRequest;
    }

    /// <summary>
    /// 应用响应拦截器链
    /// </summary>
    /// <param name="response">原始响应</param>
    /// <returns>处理后的响应</returns>
    private async Task<HttpResponseMessage> ApplyResponseInterceptors(HttpResponseMessage response) {
        var currentResponse = response;
        foreach (var interceptor in _responseInterceptors)
        {
            currentResponse = await interceptor(currentResponse);
        }

        return currentResponse;
    }

    /// <summary>
    /// 发送HTTP请求，并应用拦截器和重试机制
    /// </summary>
    /// <param name="request">要发送的请求</param>
    /// <param name="cancellationToken"></param>
    /// <returns>HTTP响应消息</returns>
    public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        var retryCount = 0;
        while (true)
        {
            try
            {
                // 检查是否请求已被取消
                cancellationToken.ThrowIfCancellationRequested();

                var clonedRequest = await CloneRequestAsync(request);
                var interceptedRequest = await ApplyRequestInterceptors(clonedRequest);
                
                // 发送请求时传入取消令牌
                var response = await _httpClient.SendAsync(interceptedRequest, cancellationToken);
                return await ApplyResponseInterceptors(response);
            }
            catch (OperationCanceledException)
            {
                // 请求被取消，直接抛出异常
                throw;
            }
            catch (Exception ex)
            {
                retryCount++;
                
                if (!ShouldRetry(ex, retryCount))
                {
                    throw;
                }

                await Task.Delay(_config.RetryDelay, cancellationToken);
            }
        }
    }

   

    #endregion

    #region IAxios 实现

    public async Task<T?> GetAsync<T>(string url, object? parameters = null, CancellationToken cancellationToken = default)
    {
        var response = await GetAsync(url, parameters, cancellationToken);
        return await DeserializeResponseAsync<T>(response);
    }

    public async Task<HttpResponseMessage> GetAsync(string url, object? parameters = null, CancellationToken cancellationToken = default)
    {
        var finalUrl = BuildUrl(AppendQueryParameters(url, parameters));
        var request = new HttpRequestMessage(HttpMethod.Get, finalUrl);
        return await SendRequestAsync(request, cancellationToken);
    }

    public async Task<T?> PostAsync<T>(string url, object? data = null, CancellationToken cancellationToken = default)
    {
        var response = await PostAsync(url, data, cancellationToken);
        return await DeserializeResponseAsync<T>(response);
    }

    public async Task<HttpResponseMessage> PostAsync(string url, object? data = null, CancellationToken cancellationToken = default)
    {
        var finalUrl = BuildUrl(url);
        var request = new HttpRequestMessage(HttpMethod.Post, finalUrl)
        {
            Content = CreateJsonContent(data)
        };
        return await SendRequestAsync(request, cancellationToken);
    }

    public async Task<T?> PutAsync<T>(string url, object? data = null, CancellationToken cancellationToken = default)
    {
        var response = await PutAsync(url, data, cancellationToken);
        return await DeserializeResponseAsync<T>(response);
    }

    public async Task<HttpResponseMessage> PutAsync(string url, object? data = null, CancellationToken cancellationToken = default)
    {
        var finalUrl = BuildUrl(url);
        var request = new HttpRequestMessage(HttpMethod.Put, finalUrl)
        {
            Content = CreateJsonContent(data)
        };
        return await SendRequestAsync(request, cancellationToken);
    }

    public async Task<T?> DeleteAsync<T>(string url, object? parameters = null, CancellationToken cancellationToken = default)
    {
        var response = await DeleteAsync(url, parameters, cancellationToken);
        return await DeserializeResponseAsync<T>(response);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string url, object? parameters = null, CancellationToken cancellationToken = default)
    {
        var finalUrl = BuildUrl(AppendQueryParameters(url, parameters));
        var request = new HttpRequestMessage(HttpMethod.Delete, finalUrl);
        return await SendRequestAsync(request, cancellationToken);
    }

    #endregion

    #region 拦截器

    public void AddRequestInterceptor(Func<HttpRequestMessage, Task<HttpRequestMessage>> interceptor) {
        _requestInterceptors.Add(interceptor);
    }

    public void AddResponseInterceptor(Func<HttpResponseMessage, Task<HttpResponseMessage>> interceptor) {
        _responseInterceptors.Add(interceptor);
    }

    #endregion
}