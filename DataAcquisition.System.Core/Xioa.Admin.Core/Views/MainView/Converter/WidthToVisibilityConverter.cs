using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Xioa.Admin.Core.Views.MainView.Converter
{
    public class WidthToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double width && parameter is string targetWidth)
            {
                if (double.TryParse(targetWidth, out double target))
                {
                    return width == target ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 