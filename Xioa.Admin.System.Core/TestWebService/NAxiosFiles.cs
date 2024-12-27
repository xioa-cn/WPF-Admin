using System.Diagnostics;
using System.Net.Sockets;
using Xioa.Admin.Request.Tools.Model;
using Xioa.Admin.Request.Tools.NetAxios;

namespace TestWebService;

public class NAxiosFiles
{
    [Fact]
    public async Task TestMethod1()
    {
        NAxios axios = new NAxios(new NAxiosConfig
        {
            BaseUrl = "http://localhost:7078/api/",
            RetryCount = 3, // 最多重试3次
            Headers =
            {
                ["accept"] = "*/*"
            },
            RetryDelay = 1000, // 每次重试间隔1秒
            RetryCondition = (exception, retryCount) =>
            {
                var res = exception switch
                {
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
        }, false);
       
        var filePath = "E:\\Test\\116.png";
        IEnumerable<FileStream> fileStreams = new[]
        {
            File.OpenRead(filePath), File.OpenRead(filePath)
        };
        var file1 = new FileUploadContent
        {
            FileStream = File.OpenRead(filePath),
            FileName = "116.png",
        };
        var file2 = new FileUploadContent
        {
            FileStream = File.OpenRead(filePath),
            FileName = "116.png"
        };
        // 多文件上传
        var files = new List<FileUploadContent>
        {
            file1, //file2
        };


        try
        {
            // 多文件上传

            var result = await axios.UploadAsync<object>(
                "/File/upload/multiple",
                files,
                formData: new Dictionary<string, string>
                {
                    { "description", "测试文件" }
                },apiFileName:"files"
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