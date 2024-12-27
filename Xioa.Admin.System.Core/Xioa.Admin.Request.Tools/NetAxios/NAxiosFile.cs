using System.Net.Http.Headers;
using System.Text;
using Xioa.Admin.Request.Tools.Model;

namespace Xioa.Admin.Request.Tools.NetAxios;

public partial class NAxios : IAxios {
    public async Task<T?> UploadAsync<T>(
        string url, 
        Stream fileStream, 
        string fileName, 
        string? contentType = null,
        Dictionary<string, string>? formData = null, 
        CancellationToken cancellationToken = default)
    {
        var content = new MultipartFormDataContent();

        // 添加文件
        var fileContent = new StreamContent(fileStream);
        if (!string.IsNullOrEmpty(contentType))
        {
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        }
        content.Add(fileContent, "files", fileName);

        // 添加其他表单字段
        if (formData != null)
        {
            foreach (var (key, value) in formData)
            {
                content.Add(new StringContent(value), key);
            }
        }

        var finalUrl = BuildUrl(url);
        var request = new HttpRequestMessage(HttpMethod.Post, finalUrl);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
        request.Content = content;

        var response = await SendRequestAsync(request, cancellationToken);
        return await DeserializeResponseAsync<T>(response);
    }

    public async Task<T?> UploadAsync<T>(string url, IEnumerable<FileUploadContent> files,
        Dictionary<string, string>? formData = null,
        CancellationToken cancellationToken = default) {
        var content = new MultipartFormDataContent();

        // 添加所有文件
        foreach (var file in files)
        {
            var fileContent = new StreamContent(file.FileStream);
            if (!string.IsNullOrEmpty(file.ContentType))
            {
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            }

            content.Add(fileContent, "files", file.FileName);
        }

        // 添加额外的表单数据
        if (formData != null)
        {
            foreach (var (key, value) in formData)
            {
                content.Add(new StringContent(value), key);
            }
        }

        var finalUrl = BuildUrl(url);
        var request = new HttpRequestMessage(HttpMethod.Post, finalUrl) {
            Content = content
        };

        var response = await SendRequestAsync(request, cancellationToken);
        return await DeserializeResponseAsync<T>(response);
    }
}