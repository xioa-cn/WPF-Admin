using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Xioa.Admin.Core.Views.VsuaButton.Converter
{
    public class ButtonToPolygonPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
             //< !--Points = "20,0 0,20 0,55 196,55 196,0"-- >
            if (value is Point point)
            {
                IEnumerable<System.Windows.Point> points = new List<System.Windows.Point>()
                {
                    new System.Windows.Point(20,0),
                    new System.Windows.Point(0,20),
                    new System.Windows.Point(0,point.X-5),
                    new System.Windows.Point(point.Y-8,point.X-5),
                    new System.Windows.Point(point.Y-8,0),
                };

                return new PointCollection(points);
            }
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
