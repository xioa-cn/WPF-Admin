using DataAcquisition.Core.WindowManager;
using System;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using DataAcquisition.Core.Views.DialogView.Model;
using DataAcquisition.Core.Views.MainView.Model;
using DataAcquisition.Core.Views.NotificationView;
using DataAcquisition.Core.Views.NotificationView.Model;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace DataAcquisition.Core.Views.MainView;

public partial class MainWindow : System.Windows.Window {
    public MainWindow() {
        InitializeComponent();
        Dialog.Register(MessageToken.DialogPageToken, this);
        WeakReferenceMessenger.Default.Register<ChangeMainBorderSizeMessanger>(this, ChangeValue);
        this.Unloaded += (sender, args) =>
        {
            WeakReferenceMessenger.Default.Unregister<ChangeMainBorderSizeMessanger>(this);
        };
    }

    private void ChangeValue(object recipient, ChangeMainBorderSizeMessanger message) {
        ChangeNavBorder();
    }

    private void MiniSize_Click(object sender, RoutedEventArgs e) {
        this.WindowState = WindowState.Minimized;
    }

    private void MaxSize_Click(object sender, RoutedEventArgs e) {
        if (this.WindowState == WindowState.Normal)
        {
            this.WindowState = WindowState.Maximized;
        }

        else if (this.WindowState == WindowState.Maximized)
        {
            this.WindowState = WindowState.Normal;
        }
    }

    private NotifyIconView? _notifyIconView;

    private async void Close_Click(object sender, RoutedEventArgs e) {
        _notifyIconView ??= new NotifyIconView();

        var closeEnum = CloseEnum.None;
        var dialog = Dialog.Show(_notifyIconView, MessageToken.DialogPageToken);
        await dialog.Initialize<NotifyIconViewModel>(
            vm => { }).GetResultAsync<CloseEnum>().ContinueWith(re => { closeEnum = re.Result; });

        switch (closeEnum)
        {
            case CloseEnum.Close:
                this.CloseWindowWithFade();
                Environment.Exit(0);
                break;
            case CloseEnum.Notify:
                await Task.Delay(100);
                this.Visibility = Visibility.Hidden;
                break;
            case CloseEnum.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OpenOrCloseNaviBar(object sender, RoutedEventArgs e) {
        ChangeNavBorder();
    }


    private void ChangeNavBorder() {
        var width = naviGrid.Width;
        const double minWidth = 55.0;
        const double maxWidth = 200.0;

        DoubleAnimation doubleAnimation = new DoubleAnimation {
            Duration = new Duration(TimeSpan.FromSeconds(0.1)),
            From = width,
            To = Math.Abs(width - minWidth) < 0.01 ? maxWidth : minWidth
        };

        Icon_grid.Visibility = Math.Abs(width - minWidth) < 0.01 ? Visibility.Collapsed : Visibility.Visible;
        naviGrid.BeginAnimation(Border.WidthProperty, doubleAnimation);
    }
}