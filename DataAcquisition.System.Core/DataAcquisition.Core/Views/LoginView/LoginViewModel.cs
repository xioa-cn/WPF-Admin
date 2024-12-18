using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAcquisition.Core.Views.MainView;
using DataAcquisition.Core.WindowManager;
using DataAcquisition.Model.Model.Login;
using HandyControl.Controls;

namespace DataAcquisition.Core.Views.LoginView;

/// <summary>
/// @author Xioa
/// @date  2024年11月28日
/// </summary>
public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty] private string? _userName;
    [ObservableProperty] private string? _password;
    [ObservableProperty] private bool _rememberPassword;

    public LoginViewModel()
    {
        UserName = "xioa";
    }

    [RelayCommand]
    private async Task Login(System.Windows.Window window)
    {
        //TODO 请求后台验证用户信息

        MainViewModel.LoginUser = new LoginUser()
        {
            UserName = this.UserName,
            Password = this.Password,
            LoginAuth = LoginAuth.Admin
        };

        if (!App.MainWindowShow.IsVisible)
        {

            window.SwitchWindow(App.MainWindowShow);

            Growl.Success($"Login Success!! {UserName}");
            return;
        }

        window.Close();
        Growl.Success($"Login Success!! {UserName}");
        await Task.CompletedTask;
    }
}