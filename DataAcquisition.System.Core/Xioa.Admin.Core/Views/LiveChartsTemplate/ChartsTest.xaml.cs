using HandyControl.Controls;
using System.Windows.Controls;
using System.Windows.Input;

namespace Xioa.Admin.Core.Views.LiveChartsTemplate;

public partial class ChartsTest : Page
{
    public ChartsTest()
    {
        this.DataContext = new ChartsViewModel();
        InitializeComponent();
    }

    private void ContentChange_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var btn = sender as Button;

        if (btn?.Tag is string str)
        {
            var viewModel = this.DataContext as ChartsViewModel;
            viewModel.ChangeContent(str);
        }

    }

    private void SearchBar_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {

            (this.DataContext as ChartsViewModel).SearchContent = (sender as SearchBar).Text;
            Keyboard.ClearFocus();
            (this.DataContext as ChartsViewModel).Search();
        }
    }
}