using System.Windows.Controls;

namespace DataAcquisition.Core.Views.DataSearch;

public partial class DataSearchView : Page
{
    public DataSearchView()
    {
        this.DataContext = new DataSearchViewModel();
        InitializeComponent();
    }
}