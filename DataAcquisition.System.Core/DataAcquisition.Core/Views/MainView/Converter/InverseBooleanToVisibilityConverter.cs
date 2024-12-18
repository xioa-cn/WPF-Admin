using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DataAcquisition.Core.Views.MainView.Converter;

/// <summary>
/// @author Xioa
/// @date  2024Äê12ÔÂ6ÈÕ
/// </summary>
public class InverseBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            return visibility != Visibility.Visible;
        }
        return true;
    }
}