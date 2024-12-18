using System.Windows;
using HandyControl.Controls;
using System.Windows.Controls;
using DataAcquisition.Core.Views.DialogView.Model;

namespace DataAcquisition.Core.Views.DialogView;

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