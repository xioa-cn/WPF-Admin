using System.Windows.Controls;

namespace Xioa.Admin.Core.Views.NotificationView;

public partial class NotifyIconView : UserControl
{
    public NotifyIconView()
    {
        this.DataContext = new NotifyIconViewModel();
        InitializeComponent();
    }
}