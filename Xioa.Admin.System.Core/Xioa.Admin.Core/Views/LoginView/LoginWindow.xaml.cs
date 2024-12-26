using System;
using System.Windows;
using Xioa.Admin.Core.Views.MainView;
using Xioa.Admin.Core.WindowManager;

namespace Xioa.Admin.Core.Views.LoginView;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
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
    private void CloseWindow_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}