using System;
using System.Globalization;
using System.Windows.Data;

namespace Xioa.Admin.Core.Views.VsuaButton.Converter;

/// <summary>
/// @author Xioa
/// @date  2024Äê12ÔÂ20ÈÕ
/// </summary>
public class HalfValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double doubleValue)
        {
            return doubleValue / 2;
        }

        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

}