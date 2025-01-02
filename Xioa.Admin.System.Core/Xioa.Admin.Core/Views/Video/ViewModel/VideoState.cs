using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Xioa.Admin.Core.Views.Video.ViewModel;

// 添加一个视频状态管理类
public class VideoState
{
    public static VideoState Instance { get; } = new VideoState();
    
    public Uri? CurrentSource { get; set; }
    public TimeSpan Position { get; set; }
    public bool IsPlaying { get; set; }
    public double Volume { get; set; }
    public bool HasState { get; set; }

    private VideoState() { }

    public void SaveState(MediaElement mediaPlayer, bool isPlaying)
    {
        if (mediaPlayer.Source != null)
        {
            CurrentSource = mediaPlayer.Source;
            Position = mediaPlayer.Position;
            IsPlaying = isPlaying;
            Volume = mediaPlayer.Volume;
            HasState = true;
        }
    }

    public void RestoreState(MediaElement mediaPlayer)
    {
        if (!HasState) return;

        try
        {
            mediaPlayer.Source = CurrentSource;
            mediaPlayer.Volume = Volume;

            // 使用 Dispatcher 延迟设置位置和播放状态
            mediaPlayer.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    mediaPlayer.Position = Position;
                    if (IsPlaying)
                    {
                        mediaPlayer.Play();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"恢复视频状态失败: {ex.Message}");
                }
            }), DispatcherPriority.Loaded);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"恢复视频源失败: {ex.Message}");
        }
    }

    public void Clear()
    {
        CurrentSource = null;
        Position = TimeSpan.Zero;
        IsPlaying = false;
        Volume = 1.0;
        HasState = false;
    }
}
