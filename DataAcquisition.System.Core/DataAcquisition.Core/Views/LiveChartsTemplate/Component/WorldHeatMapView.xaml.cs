using DataAcquisition.Core.Views.LiveChartsTemplate.ViewModel;
using System.Windows.Controls;

namespace DataAcquisition.Core.Views.LiveChartsTemplate.Component;

public partial class WorldHeatMapView : UserControl
{
    public WorldHeatMapView()
    {
        InitializeComponent();
    }

    private void Unloaded_Event(object sender, System.Windows.RoutedEventArgs e)
    {
        (this.DataContext as WorldHeatMapViewModel).IsStart = false;
    }

    private void Loaded_Event(object sender, System.Windows.RoutedEventArgs e)
    {
        (this.DataContext as WorldHeatMapViewModel).IsStart = true;
        (this.DataContext as WorldHeatMapViewModel).DoRandomChanges();
    }
}