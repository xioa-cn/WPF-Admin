using System.Diagnostics;
using System.Net.Sockets;
using Xioa.Admin.Request.Tools.Helper;
using Xioa.Admin.Request.Tools.NetAxios;

namespace TestWebService;

public class NAxiosFile {
    [Fact]
    public async Task TestMethod1() {
        NAxios axios = new NAxios(new NAxiosConfig {
            BaseUrl = "https://localhost:7078/api/",
            RetryCount = 3, // 最多重试3次
            Headers = {
              ["accept"]= "*/*"
            },
            RetryDelay = 1000, // 每次重试间隔1秒
            RetryCondition = (exception, retryCount) =>
            {
                var res = exception switch {
                    HttpRequestException httpException =>
                        // 5xx 服务器错误
                        httpException.StatusCode >= System.Net.HttpStatusCode.InternalServerError ||
                        // 408 请求超时
                        httpException.StatusCode == System.Net.HttpStatusCode.RequestTimeout ||
                        // 429 太多请求
                        httpException.StatusCode == System.Net.HttpStatusCode.TooManyRequests,
                    TaskCanceledException => true, // 超时
                    SocketException => true, // 网络连接错误
                    IOException => true, // IO错误（通常是网络相关）
                    _ => false // 其他错误不重试
                };
                return res;
            }
        });
        var progress = new Progress<double>(progressPercentage =>
        {
            Debug.WriteLine($"Upload progress: {progressPercentage:P2}");
        });
        var filePath = "D:\\Test\\header.jpg";
        using var fileStream = File.OpenRead(filePath);
        var fileName = Path.GetFileName(filePath);

        try
        {
            
            var formData = new Dictionary<string, string>
            {
                { "description", "1" }
            };
           
            var response = await axios.UploadAsync<object>(
                "/File/upload?description=1",
                fileStream,
                fileName,
                FileHeader.GetContentType(fileName),
                formData

            );

            Console.WriteLine("Upload completed successfully!");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Upload was cancelled");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Upload failed: {ex.Message}");
        }
    }
}