using System.Net.Http.Headers;
using System.Text;
using Xioa.Admin.Request.Tools.Model;

namespace Xioa.Admin.Request.Tools.NetAxios;

public static class NAxiosProgress
{
    // 单文件上传带进度
    public static async Task<T?> UploadWithProgressAsync<T>(
        this NAxios axios,
        string url,
        Stream fileStream,
        string fileName,
        IProgress<double> progress,
        string? contentType = null,
        Dictionary<string, string>? formData = null,
        CancellationToken cancellationToken = default,
        string apiFileName = "file",
        int bufferSize = 81920)
    {
        var content = new MultipartFormDataContent();

        // 创建带进度报告的流内容
        var progressContent = new ProgressStreamContent(
            fileStream,
            bufferSize,
            progress
        );

        if (!string.IsNullOrEmpty(contentType))
        {
            progressContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        }
        else
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var mimeType = axios.GetMimeType(extension);
            progressContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        }

        content.Add(progressContent, apiFileName, fileName);

        // 添加额外的表单数据
        if (formData != null)
        {
            foreach (var (key, value) in formData)
            {
                content.Add(new StringContent(value, Encoding.UTF8), key);
            }
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, 
            axios.BuildUrl(url));
        request.Content = content;
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await axios.SendRequestFileAsync(request, cancellationToken);
        return await NAxios.DeserializeResponseAsync<T>(response);
    }

    // 多文件上传带进度
    public static async Task<T?> UploadWithProgressAsync<T>(
        this NAxios axios,
        string url,
        IEnumerable<FileUploadContent> files,
        IProgress<double> progress,
        Dictionary<string, string>? formData = null,
        CancellationToken cancellationToken = default,
        string apiFileName = "files",
        int bufferSize = 81920)
    {
        var fileList = files.ToList();
        var totalSize = fileList.Sum(f => f.FileSize);
        var currentTotalUploaded = 0L;
        var progressLock = new object();

        using var content = new MultipartFormDataContent();
        var progressContents = new List<ProgressStreamContent>();

        try
        {
            // 为每个文件创建进度报告
            for (var i = 0; i < fileList.Count; i++)
            {
                var file = fileList[i];
                var currentFileUploaded = 0L;

                var fileProgress = new Progress<double>(percentage =>
                {
                    lock (progressLock)
                    {
                        // 计算当前文件已上传的字节数
                        var newFileUploaded = (long)(file.FileSize * (percentage / 100.0));
                        // 计算文件上传的增量
                        var delta = newFileUploaded - currentFileUploaded;
                        currentFileUploaded = newFileUploaded;
                        
                        // 更新总进度
                        currentTotalUploaded += delta;
                        var totalProgress = (currentTotalUploaded * 100.0) / totalSize;
                        
                        // 确保进度不超过100%
                        progress.Report(Math.Min(Math.Max(0, totalProgress), 100));
                    }
                });

                // 重置文件流位置
                if (file.FileStream.Position != 0)
                {
                    file.FileStream.Position = 0;
                }

                var progressContent = new ProgressStreamContent(
                    file.FileStream,
                    bufferSize,
                    fileProgress
                );
                progressContents.Add(progressContent);

                if (!string.IsNullOrEmpty(file.ContentType))
                {
                    progressContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                }
                else
                {
                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    var mimeType = axios.GetMimeType(extension);
                    progressContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                }

                content.Add(progressContent, apiFileName, file.FileName);
            }

            // 添加额外的表单数据
            if (formData != null)
            {
                foreach (var (key, value) in formData)
                {
                    content.Add(new StringContent(value, Encoding.UTF8), key);
                }
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, axios.BuildUrl(url));
            request.Content = content;
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await axios.SendRequestFileAsync(request, cancellationToken);
            
            return await NAxios.DeserializeResponseAsync<T>(response);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to upload files: {ex.Message}", ex);
        }
        finally
        {
            foreach (var progressContent in progressContents)
            {
                progressContent.Dispose();
            }
        }
    }
}