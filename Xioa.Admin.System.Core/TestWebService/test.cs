using System.Net.Sockets;
using Xioa.Admin.Request.Tools.NetAxios;

namespace TestWebService;

public class test
{
    [Fact]
    public async void Test1()
    {
        NAxios axios = new NAxios(new NAxiosConfig
        {
            BaseUrl = "http://localhost:5066",
            Timeout = 5000,
            RetryCount = 3,
            RetryDelay = 3000,
            RetryCondition = (exception, retryCount) =>
            {
                var res = exception switch
                {
                    HttpRequestException httpException =>
                        httpException.StatusCode >= System.Net.HttpStatusCode.InternalServerError ||
                        httpException.StatusCode == System.Net.HttpStatusCode.RequestTimeout ||
                        httpException.StatusCode == System.Net.HttpStatusCode.TooManyRequests,
                    TaskCanceledException => true,
                    SocketException => true,
                    IOException => true,
                    _ => false
                };
                return res;
            }
        });
        dynamic s = await axios.PostAsync<object>("/postTest",new {});
       
    }
}