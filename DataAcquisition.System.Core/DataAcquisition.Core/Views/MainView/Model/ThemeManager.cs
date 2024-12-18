using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DataAcquisition.Core.Views.MainView.Model;

public partial class ThemeManager : ObservableObject
{
    private static ThemeManager? _instance;
    public static ThemeManager Instance => _instance ??= new ThemeManager();

    [ObservableProperty] private bool _isDarkTheme;


    public ThemeManager()
    {
        // 默认使用深色主题
        IsDarkTheme = false;
        ApplyTheme();
    }

    partial void OnIsDarkThemeChanged(bool value)
    {
        ApplyTheme();
    }

    /***
     * 新建页面时 配合主题使用
     *
     * 在每个页面的根元素（Page、UserControl等）添加：Background="{DynamicResource Background.Brush}"
     *
     * 对于所有文本元素（TextBlock、TextBox等）使用
     * Foreground="{DynamicResource Text.Primary.Brush}"  <!-- 主要文本 -->
     * Foreground="{DynamicResource Text.Secondary.Brush}"  <!-- 次要文本 -->
     *
     * 对于所有边框和分隔线使用：
     * BorderBrush="{DynamicResource Border.Brush}"
     *
     * 对于面板和容器使用：
     * Background="{DynamicResource Background.Brush}"  <!-- 主背景 -->
     * Background="{DynamicResource Surface.Brush}"  <!-- 次要背景 -->
     *
     * 对于按钮和交互元素使用：
     * Background="{DynamicResource Primary.Brush}"  <!-- 主要按钮 -->
     * Background="{DynamicResource Secondary.Brush}"  <!-- 次要按钮 -->
     *
     * 对于输入控件（TextBox、ComboBox等）使用：
     * Background="{DynamicResource Background.Brush}"
     * Foreground="{DynamicResource Text.Primary.Brush}"
     * BorderBrush="{DynamicResource Border.Brush}"
     *
     */


    private static ResourceDictionary _hcLTheme = new ResourceDictionary()
        { Source = new Uri("pack://application:,,,/HandyControl;component/Themes/SkinDefault.xaml", UriKind.Absolute) };

    private static ResourceDictionary _hcDTheme = new ResourceDictionary()
        { Source = new Uri("pack://application:,,,/HandyControl;component/Themes/SkinDark.xaml", UriKind.Absolute) };


    private static ResourceDictionary? _currentLTheme =
        new ResourceDictionary
        {
            Source = new Uri(
                "pack://application:,,,/DataAcquisition.Core;component/Views/MainView/Themes/GreenTheme.xaml",
                UriKind.Absolute)
        };

    private static ResourceDictionary? _currentDTheme =
        new ResourceDictionary
        {
            Source = new Uri(
                "pack://application:,,,/DataAcquisition.Core;component/Views/MainView/Themes/DarkGreenTheme.xaml",
                UriKind.Absolute)
        };

    private void ApplyTheme()
    {
        var app = Application.Current;
        if (app == null) return;

        // 移除当前主题（如果存在）
        if (_currentTheme != null)
        {
            Dispatcher.CurrentDispatcher.Invoke(() => { app.Resources.MergedDictionaries.Remove(_currentTheme); });
            _currentTheme = null;
        }


        // 移除当前主题（如果存在）
        if (!IsDarkTheme && _currentLTheme != null)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                app.Resources.MergedDictionaries.Add(_currentDTheme);
                app.Resources.MergedDictionaries.Remove(_currentLTheme);
            });
        }

        if (IsDarkTheme && _currentDTheme != null)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                app.Resources.MergedDictionaries.Remove(_currentDTheme);
                app.Resources.MergedDictionaries.Add(_currentLTheme);
            });
        }

        if (!IsDarkTheme && _hcLTheme != null)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                app.Resources.MergedDictionaries.Remove(_hcLTheme);
                app.Resources.MergedDictionaries.Add(_hcDTheme);
            });
        }

        if (IsDarkTheme && _hcDTheme != null)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                app.Resources.MergedDictionaries.Remove(_hcDTheme);
                app.Resources.MergedDictionaries.Add(_hcLTheme);
            });
        }

        // Dispatcher.CurrentDispatcher.Invoke(() =>
        // {
        //     //app.Resources.MergedDictionaries.Add(_hcTheme);
        //     // 更新窗口背景色
        //     // if (Application.Current.MainWindow != null)
        //     // {
        //     //     var border =
        //     //         Application.Current.MainWindow.Template.FindName("MainBorder",
        //     //             Application.Current.MainWindow) as Border;
        //     //     if (border != null)
        //     //     {
        //     //         border.Background = (SolidColorBrush)Application.Current.Resources["Background.Brush"];
        //     //     }
        //     // }
        // });
    }

    private static ResourceDictionary? _currentTheme;

    public static void UseTheme(string content)
    {
        var app = Application.Current;
        if (app == null) return;

        // 移除当前主题（如果存在）
        if (_currentTheme != null)
        {
            app.Resources.MergedDictionaries.Remove(_currentTheme);
        }

        var themePath =
            $"pack://application:,,,/DataAcquisition.Core;component/Views/MainView/Themes/{content}Theme.xaml";
        _currentTheme = new ResourceDictionary { Source = new Uri(themePath, UriKind.Absolute) };
        app.Resources.MergedDictionaries.Add(_currentTheme);
        // 更新窗口背景色
        if (Application.Current.MainWindow != null)
        {
            var border =
                Application.Current.MainWindow.Template.FindName("MainBorder",
                    Application.Current.MainWindow) as Border;
            if (border != null)
            {
                border.Background = (SolidColorBrush)Application.Current.Resources["Background.Brush"];
            }
        }
    }
}