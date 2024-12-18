using System.Windows.Controls;

namespace DataAcquisition.Core.Views.LiveChartsTemplate.Component;

public partial class VisualElementsView : UserControl
{
    public VisualElementsView()
    {
        InitializeComponent();
    }

    private void Open_Popup(object sender, System.Windows.RoutedEventArgs e)
    {
        this.Pop.IsOpen = true;
    }
}