using System.Windows.Controls;

namespace Xioa.Admin.Core.Views.ExcelView;

public partial class ExcelPage : Page
{
    public ExcelPage()
    {
        this.DataContext = new ExcelViewModel();
        InitializeComponent();
    }
}