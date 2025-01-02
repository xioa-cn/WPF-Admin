using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Xioa.Admin.Core.Views.Video.ViewModel;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Linq;

namespace Xioa.Admin.Core.Views.Video;


public partial class VideoPage : Page {
    private DispatcherTimer timer;
    private bool isPlaying = false;
    private bool userIsDraggingSlider { get; set; } = false;
    private const double SEEK_SECONDS = 5.0; // 快进/后退的秒数
    private bool isFullscreen = false;
    private FullscreenWindow? fullscreenWindow = null;
    private Grid? originalParent = null;
    private Grid? mainGrid = null;
    private DateTime lastClickTime = DateTime.MinValue;
    private const double DOUBLE_CLICK_TIME = 300; // 双击时间阈值（毫秒）
    private TimeSpan? pendingPosition = null;  // 添加这个字段来存储待恢复的位置
    private bool isLooping { get; set; } = false;  // 添加循环播放标志

    public VideoPage() {
        InitializeComponent();
        InitializeTimer();
        this.Loaded += VideoPage_Loaded;
        this.Unloaded += VideoPage_Unloaded;
        
        // 保存主Grid的引用
        mainGrid = (Grid)Content;
        
        timelineSlider.PreviewMouseDown += TimelineSlider_PreviewMouseDown;
        timelineSlider.PreviewMouseUp += TimelineSlider_PreviewMouseUp;
    }

    private void VideoPage_Loaded(object sender, RoutedEventArgs e)
    {
        // 当页面加载时恢复状态
        VideoState.Instance.RestoreState(mediaPlayer);
    }

    private void VideoPage_Unloaded(object sender, RoutedEventArgs e)
    {
        // 当页面卸载时保存状态
        VideoState.Instance.SaveState(mediaPlayer, isPlaying);
        
        if (timer != null)
        {
            timer.Stop();
            timer = null;
        }
    }

    private void TimelineSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        userIsDraggingSlider = true;
    }

    private void TimelineSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        userIsDraggingSlider = false;
        if (mediaPlayer.Source != null)
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(timelineSlider.Value);
            UpdateTimeDisplay();
        }
    }
    private void InitializeTimer() {
        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += Timer_Tick;
    }

    private void Timer_Tick(object? sender, EventArgs e) {
        if (!userIsDraggingSlider && mediaPlayer.Source != null)
        {
            timelineSlider.Value = mediaPlayer.Position.TotalSeconds;
            UpdateTimeDisplay();
        }
    }

    private void OpenButton_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "视频文件|*.mp4;*.avi;*.wmv;*.mkv|所有文件|*.*"
        };

        // 添加一个输入URL的对话框
        var result = MessageBox.Show("是否打开网络视频？", "选择", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.Yes)
        {
            // 弹出输入框
            var dialog = new InputDialog("请输入视频URL");
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var url = dialog.ResponseText;
                    mediaPlayer.Source = new Uri(url);
                    mediaPlayer.Play();
                    isPlaying = true;
                    playButton.Content = "暂停";
                    timer.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"无法加载视频: {ex.Message}", "错误");
                }
            }
        }
        else
        {
            // 打开本地文件
            if (openFileDialog.ShowDialog() == true)
            {
                mediaPlayer.Source = new Uri(openFileDialog.FileName);
                mediaPlayer.Play();
                isPlaying = true;
                playButton.Content = "暂停";
                timer.Start();
            }
        }
    }

    private void PlayButton_Click(object sender, RoutedEventArgs e) {
        if (mediaPlayer.Source == null) return;

        if (isPlaying)
        {
            mediaPlayer.Pause();
            playButton.Content = "播放";
        }
        else
        {
            mediaPlayer.Play();
            playButton.Content = "暂停";
        }

        isPlaying = !isPlaying;
    }

    private void PauseButton_Click(object sender, RoutedEventArgs e) {
        mediaPlayer.Pause();
        isPlaying = false;
        playButton.Content = "播放";
    }

    private void StopButton_Click(object sender, RoutedEventArgs e) {
        mediaPlayer.Stop();
        isPlaying = false;
        playButton.Content = "播放";
    }
    
    private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e) {
        if (isLooping)
        {
            // 循环播放时，重新开始播放
            mediaPlayer.Position = TimeSpan.Zero;
            mediaPlayer.Play();
            isPlaying = true;
            playButton.Content = "暂停";
        }
        else
        {
            // 非循环播放时，停止播放
            mediaPlayer.Stop();
            isPlaying = false;
            playButton.Content = "播放";
            timer.Stop();
        }
    }

    private void TimelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
        if (userIsDraggingSlider && mediaPlayer.Source != null)
        {
            // 实时更新时间显示，但不更新视频位置
            var newPosition = TimeSpan.FromSeconds(e.NewValue);
            timeDisplay.Text = $"{newPosition:hh\\:mm\\:ss} / {mediaPlayer.NaturalDuration.TimeSpan:hh\\:mm\\:ss}";
        }
    }

    private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
        if (mediaPlayer != null)
        {
            mediaPlayer.Volume = e.NewValue;
        }
    }
    private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
    {
        if (mediaPlayer.NaturalDuration.HasTimeSpan)
        {
            timelineSlider.Minimum = 0;
            timelineSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            timelineSlider.SmallChange = 1;
            timelineSlider.LargeChange = Math.Min(10, mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds / 10);
            
            // 如果有待恢复的位置，在这里恢复
            if (pendingPosition.HasValue)
            {
                mediaPlayer.Position = pendingPosition.Value;
                pendingPosition = null;
            }
            
            UpdateTimeDisplay();
        }
    }
    private void UpdateTimeDisplay() {
        if (mediaPlayer.NaturalDuration.HasTimeSpan)
        {
            var currentPosition = mediaPlayer.Position;
            var totalDuration = mediaPlayer.NaturalDuration.TimeSpan;
            timeDisplay.Text = $"{currentPosition:hh\\:mm\\:ss} / {totalDuration:hh\\:mm\\:ss}";
        }
    }

    private void ForwardButton_Click(object sender, RoutedEventArgs e)
    {
        if (mediaPlayer.Source != null)
        {
            var newPosition = mediaPlayer.Position + TimeSpan.FromSeconds(SEEK_SECONDS);
            // 确保不超过视频总长度
            if (newPosition <= mediaPlayer.NaturalDuration.TimeSpan)
            {
                mediaPlayer.Position = newPosition;
            }
            else
            {
                mediaPlayer.Position = mediaPlayer.NaturalDuration.TimeSpan;
            }
            UpdateTimeDisplay();
        }
    }

    private void RewindButton_Click(object sender, RoutedEventArgs e)
    {
        if (mediaPlayer.Source != null)
        {
            var newPosition = mediaPlayer.Position - TimeSpan.FromSeconds(SEEK_SECONDS);
            // 确保不小于0
            if (newPosition >= TimeSpan.Zero)
            {
                mediaPlayer.Position = newPosition;
            }
            else
            {
                mediaPlayer.Position = TimeSpan.Zero;
            }
            UpdateTimeDisplay();
        }
    }
  
    private void MediaPlayer_BufferingStarted(object sender, RoutedEventArgs e)
    {
        // 当视频开始缓冲时显示 loading 面板
        loadingPanel.Visibility = Visibility.Visible;
    }

    private void MediaPlayer_BufferingEnded(object sender, RoutedEventArgs e)
    {
        // 当视频缓冲结束时隐藏 loading 面板
        loadingPanel.Visibility = Visibility.Collapsed;
    }

    private void MediaPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
    {
        // 当视频加载失败时
        loadingPanel.Visibility = Visibility.Collapsed;  // 隐藏 loading
        
        // 显示错误信息
        string errorMessage = e.ErrorException?.Message ?? "未知错误";
        MessageBox.Show($"视频加载失败: {errorMessage}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        
        // 重置播放器状态
        mediaPlayer.Stop();
        isPlaying = false;
        playButton.Content = "播放";
        timer.Stop();
    }

    private void FullscreenButton_Click(object sender, RoutedEventArgs e)
    {
        if (!isFullscreen)
        {
            // 进入全屏
            EnterFullscreen();
        }
        else
        {
            // 退出全屏
            ExitFullscreen();
        }
    }

    private void EnterFullscreen()
    {
        try
        {
            // 保存当前视频状态
            var currentPosition = mediaPlayer.Position;
            var currentSource = mediaPlayer.Source;
            var currentVolume = mediaPlayer.Volume;

            // 创建全屏窗口
            fullscreenWindow = new FullscreenWindow();
            
            // 设置全屏窗口的媒体播放器状态
            fullscreenWindow.mediaPlayer.Source = currentSource;
            fullscreenWindow.mediaPlayer.Volume = currentVolume;
            
            // 显示全屏窗口
            fullscreenWindow.Show();
            
            // 设置位置和播放状态
            fullscreenWindow.mediaPlayer.Position = currentPosition;
            if (isPlaying)
            {
                fullscreenWindow.mediaPlayer.Play();
                fullscreenWindow.playButton.Content = "暂停";
            }

            // 同步音量滑块
            fullscreenWindow.volumeSlider.Value = currentVolume;

            // 同步循环播放状态
            fullscreenWindow.loopButton.Opacity = isLooping ? 1.0 : 0.5;
            fullscreenWindow.SetLoopState(isLooping);

            // 更新状态
            isFullscreen = true;
            fullscreenButton.Content = "⧉";

            // 注册事件处理
            fullscreenWindow.Closed += FullscreenWindow_Closed;
            
            // 暂停原窗口的播放
            mediaPlayer.Pause();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            MessageBox.Show($"进入全屏失败: {ex.Message}", "错误");
        }
    }

    private void FullscreenWindow_Closed(object? sender, EventArgs e)
    {
        try
        {
            if (fullscreenWindow != null)
            {
                // 保存全屏窗口的状态
                var position = fullscreenWindow.mediaPlayer.Position;
                var wasPlaying = fullscreenWindow.IsPlaying;  // 使用窗口的播放状态

                // 恢复原窗口的播放状态
                mediaPlayer.Position = position;
                
                // 使用 Dispatcher 确保状态正确恢复
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (wasPlaying)
                    {
                        mediaPlayer.Play();
                        playButton.Content = "暂停";
                        isPlaying = true;
                    }
                    else
                    {
                        mediaPlayer.Pause();
                        playButton.Content = "播放";
                        isPlaying = false;
                    }
                }), DispatcherPriority.Loaded);

                fullscreenWindow = null;
                isFullscreen = false;
                fullscreenButton.Content = "⛶";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"退出全屏失败: {ex.Message}", "错误");
        }
    }

    private void ExitFullscreen()
    {
        if (fullscreenWindow != null)
        {
            fullscreenWindow.Close();
        }
    }

    // 双击视频区域切换全屏
    private void MediaPlayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var clickTime = DateTime.Now;
        
        // 检查是否是双击（两次点击间隔小于300毫秒）
        if ((clickTime - lastClickTime).TotalMilliseconds < DOUBLE_CLICK_TIME)
        {
            // 双击处理
            if (!isFullscreen)
            {
                EnterFullscreen();
            }
            else
            {
                ExitFullscreen();
            }
            
            // 重置点击时间
            lastClickTime = DateTime.MinValue;
        }
        else
        {
            // 记录第一次点击时间
            lastClickTime = clickTime;
        }
    }

    private void LoopButton_Click(object sender, RoutedEventArgs e)
    {
        isLooping = !isLooping;
        loopButton.Opacity = isLooping ? 1.0 : 0.5;  // 通过透明度显示状态
    }
}