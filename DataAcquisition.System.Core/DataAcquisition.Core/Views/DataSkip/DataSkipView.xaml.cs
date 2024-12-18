using System.Windows.Controls;

namespace DataAcquisition.Core.Views.DataSkip;

public partial class DataSkipView : Page
{
    public DataSkipView()
    {
        this.DataContext = new DataSkipViewModel();
        InitializeComponent();
    }

    private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        e.Row.Header = (e.Row.GetIndex() + 1).ToString();
    }
}