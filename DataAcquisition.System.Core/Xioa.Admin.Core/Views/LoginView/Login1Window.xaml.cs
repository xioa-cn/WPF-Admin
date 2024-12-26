using HandyControl.Controls;
using System.Diagnostics;
using System.Windows;

namespace Xioa.Admin.Core.Views.LoginView;

public partial class Login1Window : System.Windows.Window
{
    public Login1Window()
    {
        this.DataContext = new LoginViewModel();
        InitializeComponent();
    }

    private void CloseWindow_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
        Growl.Info("点击了按钮");
    }
}