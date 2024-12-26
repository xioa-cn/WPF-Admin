using System;
using System.Globalization;
using System.Windows.Data;

namespace Xioa.Admin.Core.Views.VsuaButton.Converter;

public class ToUpperConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value ?? string.Empty).ToString().ToUpper();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}