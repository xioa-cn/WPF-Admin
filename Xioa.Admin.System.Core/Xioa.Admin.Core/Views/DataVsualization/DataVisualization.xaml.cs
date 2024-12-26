using System.Windows;
using System.Windows.Controls;

namespace Xioa.Admin.Core.Views.DataVsualization;

public partial class DataVisualization : Page
{
    public DataVisualization()
    {
        this.DataContext = new DataVisualizationViewModel();
        InitializeComponent();
    }

    private void Changed_Click(object sender, RoutedEventArgs e)
    {
        if (this.DataContext is not DataVisualizationViewModel value) return;
        var obj = sender as RadioButton;
        var data = obj?.Tag as DataVisualizationViewModel.DataWeek;

        value.Change(data);
    }
}