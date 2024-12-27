using System.Net.Http.Headers;
using System.Text;
using Xioa.Admin.Request.Tools.Model;

namespace Xioa.Admin.Request.Tools.NetAxios;

public partial class NAxios : IAxios
{
    public async Task<T?> UploadAsync<T>(
        string url,
        Stream fileStream,
        string fileName,
        string? contentType = null,
        Dictionary<string, string>? formData = null,
        CancellationToken cancellationToken = default,
        string apiFileName = "file")
    {
        using var content = new MultipartFormDataContent();

        // 添加文件
        using var fileContent = new StreamContent(fileStream);
        if (!string.IsNullOrEmpty(contentType))
        {
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        }
        else
        {
            // 根据文件扩展名推断内容类型
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var mimeType = GetMimeType(extension);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
        }


        content.Add(fileContent, apiFileName, fileName);

        // 添加其他表单字段
        if (formData != null)
        {
            foreach (var (key, value) in formData)
            {
                content.Add(new StringContent(value, Encoding.UTF8), key);
            }
        }

        var finalUrl = BuildUrl(url);
        using var request = new HttpRequestMessage(HttpMethod.Post, finalUrl);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = content;

        var response = await SendRequestFileAsync(request, cancellationToken);
        return await DeserializeResponseAsync<T>(response);
    }

    public async Task<T?> UploadAsync<T>(
        string url,
        IEnumerable<FileUploadContent> files,
        Dictionary<string, string>? formData = null,
        CancellationToken cancellationToken = default,
        string apiFileName = "files")
    {
        using var content = new MultipartFormDataContent();

        // 添加所有图片文件
        foreach (var file in files)
        {
            // 创建文件内容
            var fileContent = new ByteArrayContent(await GetBytesFromStreamAsync(file.FileStream));

            // 设置内容类型
            var contentType = file.ContentType;
            if (string.IsNullOrEmpty(contentType))
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                contentType = GetMimeType(extension);
            }

            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            // 为每个文件添加唯一的名称
            content.Add(fileContent, apiFileName, file.FileName);
        }

        // 添加额外的表单数据
        if (formData != null)
        {
            foreach (var (key, value) in formData)
            {
                content.Add(new StringContent(value, Encoding.UTF8), key);
            }
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, BuildUrl(url));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = content;

        var response = await SendRequestFileAsync(request, cancellationToken);
        return await DeserializeResponseAsync<T>(response);
    }

    // 添加 MIME 类型映射方法
    public string GetMimeType(string extension)
    {
        return extension switch
        {
            ".txt" => "text/plain",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".csv" => "text/csv",
            ".xml" => "application/xml",
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",
            ".7z" => "application/x-7z-compressed",
            _ => "application/octet-stream" // 默认二进制流
        };
    }

    public async Task<HttpResponseMessage> SendRequestFileAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _httpClient.SendAsync(request, cancellationToken);
        }
        catch (HttpRequestException ex) when (ex.InnerException is System.Security.Authentication
                                                  .AuthenticationException)
        {
            throw new HttpRequestException("SSL证书验证失败，请检查服务器证书配置或使用HTTPS", ex);
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"请求失败: {ex.Message}", ex);
        }
    }

    private async Task<byte[]> GetBytesFromStreamAsync(Stream stream)
    {
        if (stream is MemoryStream memoryStream)
        {
            return memoryStream.ToArray();
        }

        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return ms.ToArray();
    }
}