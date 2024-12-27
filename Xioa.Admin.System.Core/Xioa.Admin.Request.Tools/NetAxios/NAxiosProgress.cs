using System.Net.Http.Headers;
using Xioa.Admin.Request.Tools.Model;

namespace Xioa.Admin.Request.Tools.NetAxios;

public static class NAxiosProgress {
    public static async Task<T?> UploadWithProgressAsync<T>(
        this NAxios axios,
        string url,
        Stream fileStream,
        string fileName,
        IProgress<double> progress,
        string? contentType = null,
        Dictionary<string, string>? formData = null,
        CancellationToken cancellationToken = default) {
        var content = new MultipartFormDataContent();

        // 创建带进度报告的流内容
        var progressContent = new ProgressStreamContent(
            fileStream,
            4096, // 缓冲区大小
            progress
        );

        if (!string.IsNullOrEmpty(contentType))
        {
            progressContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        }

        content.Add(progressContent, "file", fileName);

        // 添加额外的表单数据
        if (formData != null)
        {
            foreach (var (key, value) in formData)
            {
                content.Add(new StringContent(value), key);
            }
        }

        var finalUrl = axios.BuildUrl(url);
        var request = new HttpRequestMessage(HttpMethod.Post, finalUrl) {
            Content = content
        };

        var response = await axios.SendRequestAsync(request, cancellationToken);
        return await NAxios.DeserializeResponseAsync<T>(response);
    }
}