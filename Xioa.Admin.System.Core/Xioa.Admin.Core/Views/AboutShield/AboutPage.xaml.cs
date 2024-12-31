using System.Diagnostics;
using System.Windows.Controls;

namespace Xioa.Admin.Core.Views.AboutShield;

public partial class AboutPage : Page {
    public AboutPage() {
        InitializeComponent();
    }

    private void StackPanel_IsMouseDirectlyOverChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {

    }

    private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
        // 使用默认浏览器打开链接
        Process.Start(new ProcessStartInfo
        {
            FileName = e.Uri.AbsoluteUri,
            UseShellExecute = true
        });

        // 标记事件已处理
        e.Handled = true;
    }
}