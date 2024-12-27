using System;
using System.Globalization;
using System.Windows.Data;

namespace Xioa.Admin.Core.Views.XUserControls.Components.QQComboBox;

public class MultiParamsConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}