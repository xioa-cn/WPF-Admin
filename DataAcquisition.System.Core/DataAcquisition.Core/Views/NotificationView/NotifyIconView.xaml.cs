using System.Windows.Controls;

namespace DataAcquisition.Core.Views.NotificationView;

public partial class NotifyIconView : UserControl
{
    public NotifyIconView()
    {
        this.DataContext = new NotifyIconViewModel();
        InitializeComponent();
    }
}