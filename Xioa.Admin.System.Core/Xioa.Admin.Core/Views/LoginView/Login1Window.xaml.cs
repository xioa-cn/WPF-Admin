using System;
using HandyControl.Controls;
using System.Diagnostics;
using System.Windows;
using Xioa.Admin.Core.Views.MainView;
using Xioa.Admin.Core.WindowManager;

namespace Xioa.Admin.Core.Views.LoginView;

public partial class Login1Window : System.Windows.Window {
    public Login1Window() {
        this.DataContext = new LoginViewModel();
        InitializeComponent();
        this.Closed += ClosedMethod;
    }

    private void ClosedMethod(object? sender, EventArgs e) {
        if (MainViewModel.LoginUser is not null)
        {
            
        }
        else
        {
            this.CloseWindowWithFade();
            Environment.Exit(0);
        }
    }

    private void CloseWindow_Click(object sender, RoutedEventArgs e) {
        this.Close();
    }

    private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) {
        // 使用默认浏览器打开链接
        Process.Start(new ProcessStartInfo {
            FileName = e.Uri.AbsoluteUri,
            UseShellExecute = true
        });

        // 标记事件已处理
        e.Handled = true;
    }
}