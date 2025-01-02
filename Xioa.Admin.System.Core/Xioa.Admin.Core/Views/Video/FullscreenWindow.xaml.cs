using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Xioa.Admin.Core.Views.Video
{
    public partial class FullscreenWindow : Window
    {
        private bool isPlaying = false;
        private bool userIsDraggingSlider = false;
        private DateTime lastClickTime = DateTime.MinValue;
        private const double DOUBLE_CLICK_TIME = 300;
        private const double SEEK_SECONDS = 5.0;
        private bool isLooping = false;

        public  MediaElement mediaPlayerBase {
            get;
            set;
        }
        
        public bool IsPlaying => isPlaying;

        public FullscreenWindow()
        {
            InitializeComponent();
            mediaPlayerBase = this.mediaPlayer;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
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

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            isPlaying = false;
            playButton.Content = "播放";
        }

        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaPlayer != null)
            {
                mediaPlayer.Volume = e.NewValue;
            }
        }

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (isLooping)
            {
                mediaPlayer.Position = TimeSpan.Zero;
                mediaPlayer.Play();
                isPlaying = true;
                playButton.Content = "暂停";
            }
            else
            {
                mediaPlayer.Stop();
                isPlaying = false;
                playButton.Content = "播放";
            }
        }

        private void MediaPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            loadingPanel.Visibility = Visibility.Collapsed;
            MessageBox.Show($"视频加载失败: {e.ErrorException?.Message ?? "未知错误"}", "错误");
            this.Close();
        }

        private void MediaPlayer_BufferingStarted(object sender, RoutedEventArgs e)
        {
            loadingPanel.Visibility = Visibility.Visible;
        }

        private void MediaPlayer_BufferingEnded(object sender, RoutedEventArgs e)
        {
            loadingPanel.Visibility = Visibility.Collapsed;
        }

        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            loadingPanel.Visibility = Visibility.Collapsed;
        }

        private void MediaPlayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var clickTime = DateTime.Now;
            
            // 检查是否是双击
            if ((clickTime - lastClickTime).TotalMilliseconds < DOUBLE_CLICK_TIME)
            {
                this.Close();
                lastClickTime = DateTime.MinValue;
            }
            else
            {
                lastClickTime = clickTime;
            }
        }

        public void SetLoopState(bool looping)
        {
            isLooping = looping;
            loopButton.Opacity = isLooping ? 1.0 : 0.5;
        }

        private void LoopButton_Click(object sender, RoutedEventArgs e)
        {
            isLooping = !isLooping;
            loopButton.Opacity = isLooping ? 1.0 : 0.5;
        }
    }
} 