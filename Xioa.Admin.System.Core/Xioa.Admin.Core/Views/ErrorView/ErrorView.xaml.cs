using System.Windows;
using System.Windows.Controls;

namespace Xioa.Admin.Core.Views.ErrorView;

public partial class ErrorView : Page
{
    public ErrorView()
    {
        this.DataContext = new ErrorViewModel();
        InitializeComponent();
    }

    private void ContentChange_Click(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;

        if (btn?.Tag is not string str) return;
        var viewModel = this.DataContext as ErrorViewModel;
        viewModel?.ChangeContent(str);
    }
}