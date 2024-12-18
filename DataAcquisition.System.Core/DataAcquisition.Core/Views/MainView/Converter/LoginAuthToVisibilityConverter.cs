using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using DataAcquisition.Core.Views.MainView.Model;
using DataAcquisition.Model.Model.Login;

namespace DataAcquisition.Core.Views.MainView.Converter;

public class LoginAuthToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (App.ViewAuthSwitch == ViewAuthSwitch.IsEnabled)
            return Visibility.Visible;

        if (value is LoginAuth requiredAuth && MainViewModel.LoginUser != null)
        {
            return (int)MainViewModel.LoginUser.LoginAuth >= (int)requiredAuth
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}