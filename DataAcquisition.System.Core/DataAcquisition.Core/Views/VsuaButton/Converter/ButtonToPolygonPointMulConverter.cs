using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DataAcquisition.Core.Views.VsuaButton.Converter
{
    public class ButtonToPolygonPointMulConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2 || !(values[0] is double width) || !(values[1] is double height))
                return null;

            // 计算斜切角的大小（可以调整这个值来改变斜切角度）
            double cutSize = height / 2;

            // 按照顺序生成六个点
            var points = new PointCollection
        {
            new Point(cutSize, 0),           // 左上角切点
          new Point(0,height),
          new Point(width -cutSize,height),
          new Point(width,0),
        };

            return points;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
