using System.Net.Sockets;
using Xioa.Admin.Request.Tools.Model;
using Xioa.Admin.Request.Tools.NetAxios;

namespace TestWebService;

public class NAxiosFilesProgress
{
    [Fact]
    public async Task UploadProgressTest()
    {
        NAxios axios = new NAxios(new NAxiosConfig
        {
            BaseUrl = "http://localhost:7078/api/",
            Timeout = 30000,
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
        var progress = new Progress<double>(percentage => { Console.WriteLine($"上传进度: {percentage:F2}"); });
         var fileStream = "E:\\Test\\116.png";
         var fileStream1 = "E:\\Test\\117.png";
        List<FileUploadContent> fileUploadContents = new List<FileUploadContent>()
        {
            new FileUploadContent
            {
                FileStream =File.OpenRead( fileStream),
                FileName = "1.zip",
               
            },
            new FileUploadContent
            {
                FileStream = File.OpenRead( fileStream1),
                FileName = "2.zip",
               
            },
        };
       
        var result = await axios.UploadWithProgressAsync<object>(
            "/File/upload/multiple",
            fileUploadContents,
            progress,
            formData: new Dictionary<string, string>()
            {
                { "description", "测试文件" }
            },
            apiFileName: "files"
        );
    }
}