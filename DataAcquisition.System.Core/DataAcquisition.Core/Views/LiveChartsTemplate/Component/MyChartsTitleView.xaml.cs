using System.Windows.Controls;

namespace DataAcquisition.Core.Views.LiveChartsTemplate.Component;

public partial class MyChartsTitleView : UserControl
{
    public MyChartsTitleView()
    {
        InitializeComponent();
    }

    private void Open_Popup(object sender, System.Windows.RoutedEventArgs e)
    {
        this.Pop.IsOpen = true;
    }
}