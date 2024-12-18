using System;
using System.Globalization;
using System.Windows.Data;

namespace DataAcquisition.Core.Views.MainView.Converter;

public class BoolToAngleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isChecked)
        {
            return isChecked ? 180 : 0;
        }
        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}