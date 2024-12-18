using System.Windows.Controls;

namespace DataAcquisition.Core.Views.ExcelView;

public partial class ExcelPage : Page
{
    public ExcelPage()
    {
        this.DataContext = new ExcelViewModel();
        InitializeComponent();
    }
}