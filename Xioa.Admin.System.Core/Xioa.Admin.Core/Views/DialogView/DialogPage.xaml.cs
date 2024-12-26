using System.Windows;
using HandyControl.Controls;
using System.Windows.Controls;
using Xioa.Admin.Core.Views.DialogView.Model;

namespace Xioa.Admin.Core.Views.DialogView;

public partial class DialogPage : System.Windows.Controls.Page
{
    public DialogPage()
    {
        this.DataContext = new DialogViewModel();
        InitializeComponent();
    }

    private async void Show_Dialog(object sender, RoutedEventArgs e)
    {
        Dialog.Show("Dialog",MessageToken.DialogPageToken);
    }
}