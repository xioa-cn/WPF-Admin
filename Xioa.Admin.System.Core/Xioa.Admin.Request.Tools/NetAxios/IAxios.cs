using Xioa.Admin.Request.Tools.Model;

namespace Xioa.Admin.Request.Tools.NetAxios;

public interface IAxios {
    #region 请求Client

    public HttpClient _httpClient { get; }

    #endregion

    #region GET请求

    Task<T?> GetAsync<T>(string url, object? parameters = null, Dictionary<string, string>? headers = null);
    Task<HttpResponseMessage> GetAsync(string url, object? parameters = null, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default);

    #endregion

    #region POST请求

    Task<T?> PostAsync<T>(string url, object? data = null, Dictionary<string, string>? headers = null);
    Task<HttpResponseMessage> PostAsync(string url, object? data = null, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default);

    #endregion

    #region PUT请求

    Task<T?> PutAsync<T>(string url, object? data = null, Dictionary<string, string>? headers = null);
    Task<HttpResponseMessage> PutAsync(string url, object? data = null, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default);

    #endregion

    #region DELETE请求

    Task<T?> DeleteAsync<T>(string url, Dictionary<string, string>? headers = null);
    Task<HttpResponseMessage> DeleteAsync(string url, object? parameters = null, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default);

    #endregion

    #region 拦截器

    void AddRequestInterceptor(Func<HttpRequestMessage, Task<HttpRequestMessage>> interceptor);
    void AddResponseInterceptor(Func<HttpResponseMessage, Task<HttpResponseMessage>> interceptor);

    #endregion
    
    
    #region 文件上传
    Task<T?> UploadAsync<T>(string url, Stream fileStream, string fileName, string? contentType = null, 
        Dictionary<string, string>? formData = null, CancellationToken cancellationToken = default,string apiFileName = "file");
    
    Task<T?> UploadAsync<T>(string url, IEnumerable<FileUploadContent> files, 
        Dictionary<string, string>? formData = null, CancellationToken cancellationToken = default,string apiFileName = "file");
    #endregion
}