using System.Windows;
using System.Windows.Controls;
using DataAcquisition.Core.Views.WeldingMonitor.ViewModel;

namespace DataAcquisition.Core.Views.WeldingMonitor;

public partial class WeldingPage : Page
{
    public WeldingPage()
    {
        DataContext = new WeldingMonitorViewModel();
        InitializeComponent();
        this.Loaded += Loaded_Event;
        this.Unloaded += Unloaded_Event;
    }

    private void Unloaded_Event(object sender, RoutedEventArgs e)
    {
        if (this.DataContext is WeldingMonitorViewModel value)
        {
            value.End();
        }
    }

    private void Loaded_Event(object sender, RoutedEventArgs e)
    {
        if (this.DataContext is WeldingMonitorViewModel value)
        {
            value.Start();
        }
    }
}