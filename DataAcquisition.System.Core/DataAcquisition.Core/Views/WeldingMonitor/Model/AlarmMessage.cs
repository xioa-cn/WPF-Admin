namespace DataAcquisition.Core.Views.WeldingMonitor.Model;

///<summary>
/// @author：XIOA (xioa.liu)
/// @date：2024-12-07
/// @belong-sln：DataAcquisition.System.Core 
/// @desc：AlarmMessage
///</summary>
/// <summary>
/// 报警消息类
/// 包含报警信息、时间和严重程度颜色
/// </summary>
public class AlarmMessage
{
    /// <summary>
    /// 报警消息内容
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 报警时间
    /// </summary>
    public string Time { get; set; }

    /// <summary>
    /// 报警严重程度对应的颜色
    /// </summary>
    public string SeverityColor { get; set; }
}