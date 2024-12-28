using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Xioa.Admin.Core.Views.MainView;
using Xioa.Admin.Core.WindowManager;
using Xioa.Admin.Model.Model.Login;
using HandyControl.Controls;
using Xioa.Admin.Core.Services.Tokens.Http;

namespace Xioa.Admin.Core.Views.LoginView;

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
        window.IsEnabled = false;

        try
        {
            await LoginRequestService.Login();

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
        catch (System.Exception ex)
        {
            MessageBox.Show(ex.Message);

        }
        window.IsEnabled = true;
    }
}