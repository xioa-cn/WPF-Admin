using Microsoft.VisualBasic.Devices;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Xioa.Admin.Core.Views.ScreenRecording.Window;

namespace Xioa.Admin.Core.Views.ScreenRecording.Utils;

public class ScreenRecordHelper {
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
    static Process? _ffmpegProcess = null;

    //ffmpeg.exe实体文件路径，建议把ffmpeg.exe及其配套放在自己的Debug目录下
    static string _ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg", "ffmpeg.exe");

    private static Rectangle? _recordRegion = null;
    private static RecordingBorderWindow? _borderWindow = null;

    // 设置录制区域
    public static void SetRecordRegion(Rectangle region) {
        _recordRegion = region;
        Dispatcher.CurrentDispatcher.Invoke(ShowBorderWindow);
    }

    // 清除录制区域
    public static void ClearRecordRegion() {
        _recordRegion = null;
        HideBorderWindow();
    }

    private static void ShowBorderWindow() {
        if (_recordRegion.HasValue)
        {
            if (_borderWindow == null)
            {
                _borderWindow = new RecordingBorderWindow();
            }

            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                _borderWindow.SetRegion(_recordRegion.Value);
                _borderWindow.Show();
            });
        }
    }

    private static void HideBorderWindow() {
        if (_borderWindow != null)
        {
            _borderWindow.Hide();
        }
    }

    private static void FindDevices() {
        // 获取 wasapi 设备列表
        Process deviceListProcess = new Process {
            StartInfo = new ProcessStartInfo {
                FileName = _ffmpegPath,
                Arguments = "-list_devices true -f wasapi -i dummy",
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        string deviceList = string.Empty;
        deviceListProcess.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                deviceList += e.Data + Environment.NewLine;
            }
        };

        deviceListProcess.Start();
        deviceListProcess.BeginErrorReadLine();
        deviceListProcess.WaitForExit();

        // 查找默认音频输出设备
        var match = System.Text.RegularExpressions.Regex.Match(deviceList,
            @"(?<="")\{[0-9A-F\-\.]+\}\.\{[0-9A-F\-]+\}(?="")");
    }

    /// <summary>
    /// 开始录制
    /// </summary>
    /// <param name="outFilePath">录屏文件保存路径</param>
    public static void Start(string outFilePath) {
        try
        {
            if (File.Exists(outFilePath))
            {
                File.Delete(outFilePath);
            }

            _ffmpegProcess = new Process();

            var audioDevices = GetInDeviceListFull();
            string audioInput;
            if (audioDevices.Count == 0)
            {
                throw new Exception("No audio input device found");
            }

            // 查找虚拟音频捕获设备和麦克风
            // var stereoMix = audioDevices.FirstOrDefault(d =>
            //     d.Contains("Stereo Mix", StringComparison.OrdinalIgnoreCase) ||
            //     d.Contains("Virtual Audio", StringComparison.OrdinalIgnoreCase));
            //
            // var microphone = audioDevices.FirstOrDefault(d =>
            //     d.Contains("Microphone", StringComparison.OrdinalIgnoreCase));
            //
            // if (stereoMix != null && microphone != null)
            // {
            //     // 同时使用两个音频输入源
            //     audioInput = $"-f dshow -i audio=\"{stereoMix}\" " +
            //                  $"-f dshow -i audio=\"{microphone}\" " +
            //                  // 混音滤镜
            //                  "-filter_complex " +
            //                  "\"[1:a]volume=0.5[a1];" + // 降低麦克风音量
            //                  "[0:a][a1]amix=inputs=2:duration=first:dropout_transition=2[aout]\" " +
            //                  "-map 0:v -map \"[aout]\" ";
            // }
            // else if (stereoMix != null)
            // {
            //     // 只有系统声音
            //     audioInput = $"-f dshow -i audio=\"{stereoMix}\"";
            // }
            // else if (microphone != null)
            // {
            //     // 只有麦克风
            //     audioInput = $"-f dshow -i audio=\"{microphone}\"";
            // }
            // else
            // {
            var defaultAudio = GetDefaultAudioInputDevice();


            audioInput = $"-f dshow -i audio=\"{defaultAudio}\"";
            // else
            // {
            //     throw new NullReferenceException(nameof(audioDevices));
            // }
            // }

            //FindDevices();
            // var outputDevices = GetDefaultAudioDevice();
            var audioOutput = string.Empty;
            var h = audioDevices.FirstOrDefault(e => e.Contains("立体声混音"));
            if (h is not null)
            {
                audioOutput = string.Join(" ", // 音频输入 - 使用WASAPI捕获输出声音
                    "-f dshow",
                    $"-i audio=\"{h}\" ", // 正确的WASAPI格式
                    "-filter_complex [1:a]volume=0.8[a1];[2:a]volume=0.8[a2];[a1][a2]amix=inputs=2[a]",
                    "-map 0:v -map \"[a]\""
                ); // 设置为立体声
            }

            Debug.WriteLine("Audio输入:" + audioInput);
            Debug.WriteLine("Audio输入出:" + audioOutput);
            string videoInput;
            if (_recordRegion.HasValue)
            {
                var r = _recordRegion.Value;
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
            string arguments = string.Join(" ", $"{videoInput} ",
                $"{audioInput}",
                audioOutput,
                "-c:v libx264 ",
                "-preset veryfast ",
                "-crf 23 ", // 使用CRF模式而不是指定比特率
                "-pix_fmt yuv420p ",
                "-c:a aac ",
                "-b:a 128k ",
                "-y ",
                $"\"{outFilePath}\"").Trim();

            Debug.WriteLine($"完整ffmpeg命令: {_ffmpegPath} {arguments}");

            ProcessStartInfo startInfo = new ProcessStartInfo(_ffmpegPath);
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = arguments;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardInput = true;

            _ffmpegProcess.ErrorDataReceived += new DataReceivedEventHandler(Output);
            _ffmpegProcess.StartInfo = startInfo;

            _ffmpegProcess.Start();
            _ffmpegProcess.BeginErrorReadLine();

            if (_ffmpegProcess.HasExited)
            {
                throw new Exception("ffmpeg process exited immediately");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"录制启动失败: {ex.Message}");
            if (ex.InnerException != null)
            {
                Debug.WriteLine($"内部错误: {ex.InnerException.Message}");
            }

            // 下载 ffmpeg 复制到Debug路径下 才可执行 录屏 => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg", "ffmpeg.exe");
            throw;
        }
    }

    /// <summary>
    /// 获取默认音频输出设备ID（WASAPI格式）
    /// </summary>
    private static MMDevice? GetDefaultAudioDevice() {
        try
        {
            using var enumerator = new MMDeviceEnumerator();
            var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            if (device is null)
                return null;
            // 返回WASAPI格式的设备ID
            return device;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting default audio device: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 获取默认输出设备
    /// </summary>
    public static string GetDefaultAudioInputDevice() {
        using (var enumerator = new MMDeviceEnumerator())
        {
            var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
            return defaultDevice.FriendlyName;
        }
    }

    /// <summary>
    /// 获取所有输出设备
    /// </summary>
    public static List<AudioDeviceInfo> GetOutputDevices() {
        var devices = new List<AudioDeviceInfo>();

        try
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

                foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                {
                    devices.Add(new AudioDeviceInfo {
                        Id = device.ID,
                        Name = device.FriendlyName,
                        IsDefault = device.ID == defaultDevice.ID,
                        DataFlow = DataFlow.Render
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting output devices: {ex.Message}");
        }

        return devices;
    }

    /// <summary>
    /// 获取声音输入设备完整名称
    /// </summary>
    /// <returns></returns>
    public static List<string> GetInDeviceListFull() {
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
                    if (!device.FriendlyName.StartsWith(deviceInfo.ProductName)) continue;
                    devices.Add(device.FriendlyName);
                    break;
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
    public static void Stop() {
        ClearRecordRegion();
        // 停止录制时隐藏边框
        HideBorderWindow();
        if (_ffmpegProcess is null) return;
        _ffmpegProcess.StandardInput.WriteLine("q");
        _ffmpegProcess.Close();
        _ffmpegProcess.Dispose();
        _ffmpegProcess = null;
    }

    /// <summary>
    /// 控制台输出信息
    /// </summary>
    private static void Output(object sender, DataReceivedEventArgs e) {
        if (String.IsNullOrEmpty(e.Data)) return;
        Debug.WriteLine($"ffmpeg输出: {e.Data}");

        // error标注出来
        if (e.Data.Contains("error", StringComparison.OrdinalIgnoreCase))
        {
            Debug.WriteLine($"发现错误: {e.Data}");
        }
    }
}

public class AudioDeviceInfo {
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsDefault { get; set; }
    public DataFlow DataFlow { get; set; } // Render = 输出, Capture = 输入
}