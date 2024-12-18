using System;
using System.Globalization;
using System.Windows.Data;
using DataAcquisition.Core.Views.MainView.Model;
using DataAcquisition.Model.Model.Login;

namespace DataAcquisition.Core.Views.MainView.Converter;

/// <summary>
/// @author Xioa
/// @date  2024年12月6日
/// </summary>
public class LoginAuthToEnabledConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (App.ViewAuthSwitch == ViewAuthSwitch.Visibility)
            return true;
        
        if (value is LoginAuth requiredAuth && MainViewModel.LoginUser != null)
        {
            return (int)MainViewModel.LoginUser.LoginAuth >= (int)requiredAuth;
        }
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}