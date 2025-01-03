using Microsoft.VisualBasic.Devices;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Xioa.Admin.Core.Views.ScreenRecording.Window;

namespace Xioa.Admin.Core.Views.ScreenRecording.Utils;

public class ScreenRecordHelper
{
    #region 模拟控制台信号需要使用的API

    [DllImport("kernel32.dll")]
    static extern bool GenerateConsoleCtrlEvent(int dwCtrlEvent, int dwProcessGroupId);

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleCtrlHandler(IntPtr handlerRoutine, bool add);

    [DllImport("kernel32.dll")]
    static extern bool AttachConsole(int dwProcessId);

    [DllImport("kernel32.dll")]
    static extern bool FreeConsole();

    #endregion

    //ffmpeg进程
    static Process ffmpegProcess = null;

    //ffmpeg.exe实体文件路径，建议把ffmpeg.exe及其配套放在自己的Debug目录下
    static string ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg", "ffmpeg.exe");
  
    private static Rectangle? recordRegion = null;
    private static RecordingBorderWindow borderWindow = null;

    // 设置录制区域
    public static void SetRecordRegion(Rectangle region)
    {
        recordRegion = region;
        Dispatcher.CurrentDispatcher.Invoke(() =>
        {
            ShowBorderWindow();
        });
     
    }

    // 清除录制区域
    public static void ClearRecordRegion()
    {
        recordRegion = null;
        HideBorderWindow();
    }

    private static void ShowBorderWindow()
    {
        if (recordRegion.HasValue)
        {
            if (borderWindow == null)
            {
                borderWindow = new RecordingBorderWindow();
            }
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                borderWindow.SetRegion(recordRegion.Value);
                borderWindow.Show();

            });
            
        }
    }

    private static void HideBorderWindow()
    {
        if (borderWindow != null)
        {
            borderWindow.Hide();
        }
    }

    /// <summary>
    /// 开始录制
    /// </summary>
    /// <param name="outFilePath">录屏文件保存路径</param>
    public static void Start(string outFilePath)
    {
        try
        {
            if (File.Exists(outFilePath))
            {
                File.Delete(outFilePath);
            }
            ffmpegProcess = new Process();

            var list = GetInDeviceListFull();
            if (list.Count == 0)
            {
                throw new Exception("No audio input device found");
            }

            string videoInput;
            if (recordRegion.HasValue)
            {
                var r = recordRegion.Value;
                if (r.Width <= 0 || r.Height <= 0)
                {
                    throw new Exception($"Invalid region size: {r.Width}x{r.Height}");
                }

                // 确保宽高是2的倍数
                int width = r.Width + (r.Width % 2);
                int height = r.Height + (r.Height % 2);
                
                videoInput = $"-f gdigrab -framerate 30 -offset_x {r.X} -offset_y {r.Y} " +
                           $"-video_size {width}x{height} -i desktop";
            }
            else
            {
                // 获取主屏幕分辨率
                var screenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                var screenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                
                // 确保宽高是2的倍数
                screenWidth += (screenWidth % 2);
                screenHeight += (screenHeight % 2);
                
                videoInput = $"-f gdigrab -framerate 30 -video_size {screenWidth}x{screenHeight} -i desktop";
            }

            // 使用更简单的编码参数
            string arguments = $"{videoInput} " +
                             $"-f dshow -i audio=\"{list[0]}\" " +
                             "-c:v libx264 " +
                             "-preset veryfast " +    // 改回veryfast
                             "-crf 23 " +             // 使用CRF模式而不是指定比特率
                             "-pix_fmt yuv420p " +
                             "-c:a aac " +
                             "-b:a 128k " +
                             "-y " +
                             $"\"{outFilePath}\"";

            Debug.WriteLine($"完整FFmpeg命令: {ffmpegPath} {arguments}");

            ProcessStartInfo startInfo = new ProcessStartInfo(ffmpegPath);
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = arguments;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardInput = true;

            ffmpegProcess.ErrorDataReceived += new DataReceivedEventHandler(Output);
            ffmpegProcess.StartInfo = startInfo;

            ffmpegProcess.Start();
            ffmpegProcess.BeginErrorReadLine();

            if (ffmpegProcess.HasExited)
            {
                throw new Exception("FFmpeg process exited immediately");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"录制启动失败: {ex.Message}");
            if (ex.InnerException != null)
            {
                Debug.WriteLine($"内部错误: {ex.InnerException.Message}");
            }
            throw;
        }
    }
    /// <summary>
    /// 获取声音输入设备完整名称
    /// </summary>
    /// <returns></returns>
    public static List<string> GetInDeviceListFull()
    {
        List<string> devices = new List<string>();
        var enumberator = new MMDeviceEnumerator();
        var deviceCollection = enumberator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.All);
        for (int waveInDevice = 0; waveInDevice < WaveIn.DeviceCount; waveInDevice++)
        {
            WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
            foreach (MMDevice device in deviceCollection)
            {
                try
                {
                    if (device.FriendlyName.StartsWith(deviceInfo.ProductName))
                    {
                        devices.Add(device.FriendlyName);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
        }
        return devices;
    }
    /// <summary>
    /// 停止录制
    /// </summary>
    public static void Stop()
    {
        // 停止录制时隐藏边框
        HideBorderWindow();
        if (ffmpegProcess is null) return;
        ffmpegProcess.StandardInput.WriteLine("q");
        ffmpegProcess.Close();
        ffmpegProcess.Dispose();
        ffmpegProcess = null;
    }

    /// <summary>
    /// 控制台输出信息
    /// </summary>
    private static void Output(object sender, DataReceivedEventArgs e)
    {
        if (!String.IsNullOrEmpty(e.Data))
        {
            Debug.WriteLine($"FFmpeg输出: {e.Data}");
            
            // 如果包含error字样，特别标注出来
            if (e.Data.Contains("error", StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine($"发现错误: {e.Data}");
            }
        }
    }
}