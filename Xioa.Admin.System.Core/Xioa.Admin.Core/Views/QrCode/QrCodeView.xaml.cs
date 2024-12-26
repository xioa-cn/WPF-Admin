using System.Windows.Controls;

namespace Xioa.Admin.Core.Views.QrCode;

public partial class QrCodeView : Page
{
    public QrCodeView()
    {
        this.DataContext = new QrCodeViewModel();
        InitializeComponent();
    }
}