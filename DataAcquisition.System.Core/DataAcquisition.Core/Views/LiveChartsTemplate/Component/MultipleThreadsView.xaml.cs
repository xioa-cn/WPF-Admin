using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DataAcquisition.Core.Views.LiveChartsTemplate.ViewModel;

namespace DataAcquisition.Core.Views.LiveChartsTemplate.Component;

public partial class MultipleThreadsView : UserControl
{
    public MultipleThreadsView()
    {
        InitializeComponent();
    }

    private void Loaded_Event(object sender, RoutedEventArgs e)
    {
        var readTasks = 10;
        (this.DataContext as MultipleThreadsViewModel).IsReading = true;
        for (var i = 0; i < readTasks; i++)
        {
            _ = Task.Run((this.DataContext as MultipleThreadsViewModel).ReadData);
        }
        ;
    }

    private void Unloaded_Event(object sender, RoutedEventArgs e)
    {
        (this.DataContext as MultipleThreadsViewModel).IsReading = false;
    }
}