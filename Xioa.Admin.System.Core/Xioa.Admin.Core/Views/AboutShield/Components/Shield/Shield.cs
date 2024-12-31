using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Xioa.Admin.Core.Views.AboutShield.Components.Shield
{
    public class Shield : ButtonBase
    {
        static Shield()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Shield), 
                new FrameworkPropertyMetadata(typeof(Shield)));
        }

        #region 依赖属性

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(Shield), new PropertyMetadata(null));

        public static readonly DependencyProperty SubjectProperty =
            DependencyProperty.Register("Subject", typeof(string), typeof(Shield), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(Shield), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty SubjectBackgroundProperty =
            DependencyProperty.Register("SubjectBackground", typeof(Brush), typeof(Shield), 
                new PropertyMetadata(Brushes.DarkGray));

        public static readonly DependencyProperty StatusBackgroundProperty =
            DependencyProperty.Register("StatusBackground", typeof(Brush), typeof(Shield), 
                new PropertyMetadata(Brushes.LightGray));

        #endregion

        #region 属性

        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string Subject
        {
            get => (string)GetValue(SubjectProperty);
            set => SetValue(SubjectProperty, value);
        }

        public string Status
        {
            get => (string)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public Brush SubjectBackground
        {
            get => (Brush)GetValue(SubjectBackgroundProperty);
            set => SetValue(SubjectBackgroundProperty, value);
        }

        public Brush StatusBackground
        {
            get => (Brush)GetValue(StatusBackgroundProperty);
            set => SetValue(StatusBackgroundProperty, value);
        }

        #endregion
    }
}