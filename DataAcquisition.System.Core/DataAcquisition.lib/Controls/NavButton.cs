using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace DataAcquisition.lib.Controls
{
    public class NavButton : ContentControl
    {
        public ObservableCollection<object> ItemSource
        {
            get { return (ObservableCollection<object>)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register(nameof(ItemSource), typeof(ObservableCollection<object>), typeof(NavButton));
    }
}
