using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace DataAcquisition.Core.WindowManager;

public static class WindowManager
{
    private const double AnimationDuration = 2000; // 动画持续时间(毫秒)

    public static void SwitchWindow(this Window currentWindow, Window newWindow)
    {
        // 设置新窗口的初始状态
        newWindow.Opacity = 0;
        newWindow.Show();
        newWindow.WindowState = currentWindow.WindowState;

        // 创建淡出动画
        var fadeOutAnimation = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(AnimationDuration),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
        };

        // 创建淡入动画
        var fadeInAnimation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(AnimationDuration),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
        };

        // 当淡出完成时关闭当前窗口
        fadeOutAnimation.Completed += (s, e) => { currentWindow.Close(); };

        // 开始动画
        currentWindow.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
        newWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
    }

    public static void ShowWindowWithFade(this Window window)
    {
        window.Opacity = 0;
        window.Show();

        var fadeInAnimation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(AnimationDuration),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
        };

        window.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
    }

    public static void CloseWindowWithFade(this Window window)
    {
        var fadeOutAnimation = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(AnimationDuration),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
        };
        
        fadeOutAnimation.Completed += (s, e) => window.Close();
        window.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
    }
}