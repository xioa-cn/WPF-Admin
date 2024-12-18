using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DataAcquisition.Core.Views.LiveChartsTemplate.ViewModel;

namespace DataAcquisition.Core.Views.LiveChartsTemplate.Component;

public partial class MultipleThreads2View : UserControl
{
    public MultipleThreads2View()
    {
        InitializeComponent();
    }
    
    private void InvokeOnUIThread(Action action)
    {
        Dispatcher.Invoke(action);
    }

    private void Loaded_Event(object sender, RoutedEventArgs e)
    {
        var readTasks = 10;
        (this.DataContext as MultipleThreads2ViewModel).Init(InvokeOnUIThread);
        (this.DataContext as MultipleThreads2ViewModel).IsReading = true;
        for (var i = 0; i < readTasks; i++)
        {
            _ = Task.Run((this.DataContext as MultipleThreads2ViewModel).ReadData);
        }
        ;
    }

    private void Unloaded_Event(object sender, RoutedEventArgs e)
    {
        (this.DataContext as MultipleThreads2ViewModel).IsReading = false;
    }
}