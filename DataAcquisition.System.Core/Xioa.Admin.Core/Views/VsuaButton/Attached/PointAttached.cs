using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace Xioa.Admin.Core.Views.VsuaButton.Attached
{
    public static class PointAttached
    {
        public static readonly DependencyProperty PointProperty = DependencyProperty.RegisterAttached(
        "Point",
        typeof(Point),
        typeof(PointAttached),
        new PropertyMetadata(new Point(60,200)));

        public static Point GetPoint(DependencyObject obj)
        {
            return (Point)obj.GetValue(PointProperty);
        }

        public static void SetPoint(DependencyObject obj, Point value)
        {
            obj.SetValue(PointProperty, value);
        }

       
    }
}
