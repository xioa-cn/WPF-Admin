using System.Windows.Controls;
using Xioa.Admin.Model.Model.Login;

namespace Xioa.Admin.Core.Views.MainView.Utils;

/// <summary>
/// @author Xioa
/// @date  2024年12月7日
/// </summary>
public static class FrameHelper
{
    public static bool Navigate(this Frame frame, Page content, LoginAuth auth)
    {
        if (MainViewModel.LoginUser is not null && (int)auth > (int)MainViewModel.LoginUser?.LoginAuth)
        {
            content.IsEnabled = false;
        }

        return frame.Navigate(content);
    }
}