using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DataAcquisition.Core.Views.WeldingMonitor.Converters
{
    public class ResultColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string result)
            {
                return result.ToUpper() switch
                {
                    "OK" => new SolidColorBrush(Color.FromRgb(0, 255, 136)), // #00ff88
                    "NG" => new SolidColorBrush(Color.FromRgb(255, 68, 68)), // #ff4444
                    _ => new SolidColorBrush(Color.FromRgb(224, 255, 255))   // #e0ffffff
                };
            }
            return new SolidColorBrush(Color.FromRgb(224, 255, 255));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 