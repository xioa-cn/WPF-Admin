using Xioa.Admin.Core.Views.LiveChartsTemplate.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Xioa.Admin.Core.Views.LiveChartsTemplate.Component;

public partial class RealTimeView : UserControl
{
    public RealTimeView()
    {
        InitializeComponent();
        
    }



    private void Unloaded_event(object sender, RoutedEventArgs e)
    {
        var dc =
                this.DataContext as RealTimeViewModel;
        dc.IsReading = false;
    }

    private void Loaded_event(object sender, RoutedEventArgs e)
    {
        var dc =
               this.DataContext as RealTimeViewModel;
        dc.IsReading = true;
        dc. ReadData();
    }
}