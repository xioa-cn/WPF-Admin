namespace Xioa.Admin.Request.Tools.NetAxios;

public class NAxiosConfig
{
    public string? BaseUrl { get; set; }
    public int Timeout { get; set; } = 10000; // 默认10秒
    public Dictionary<string, string> Headers { get; set; } = new();
    // 重试配置
    public int RetryCount { get; set; } = 3;  // 重试次数
    public int RetryDelay { get; set; } = 1000;  // 重试延迟(毫秒)
    public Func<Exception, int, bool>? RetryCondition { get; set; }  // 重试条件
}