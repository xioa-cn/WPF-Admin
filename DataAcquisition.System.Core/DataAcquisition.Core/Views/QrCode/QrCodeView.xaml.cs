using System.Windows.Controls;

namespace DataAcquisition.Core.Views.QrCode;

public partial class QrCodeView : Page
{
    public QrCodeView()
    {
        this.DataContext = new QrCodeViewModel();
        InitializeComponent();
    }
}