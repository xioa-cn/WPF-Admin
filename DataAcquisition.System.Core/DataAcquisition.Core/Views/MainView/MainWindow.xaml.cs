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

namespace DataAcquisition.Core.Views.MainView;

public partial class MainWindow : System.Windows.Window
{
    public MainWindow()
    {
        InitializeComponent();
        Dialog.Register(MessageToken.DialogPageToken, this);
        WeakReferenceMessenger.Default.Register<ChangeMainBorderSizeMessanger>(this, ChangeValue);
        this.Unloaded += (sender, args) =>
        {
            WeakReferenceMessenger.Default.Unregister<ChangeMainBorderSizeMessanger>(this);
        };
    }

    private void ChangeValue(object recipient, ChangeMainBorderSizeMessanger message)
    {
        ChangeNavBorder();
    }

    private void MiniSize_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void MaxSize_Click(object sender, RoutedEventArgs e)
    {
        if (this.WindowState == WindowState.Normal)
        {
            this.WindowState = WindowState.Maximized;
        }

        else if (this.WindowState == WindowState.Maximized)
        {
            this.WindowState = WindowState.Normal;
        }
    }
    private NotifyIconView? _NotifyIconView;
    private async void Close_Click(object sender, RoutedEventArgs e)
    {
        if(_NotifyIconView is null)
        {
            _NotifyIconView = new NotifyIconView();
        }

        CloseEnum closeEnum = CloseEnum.None;
        var dialog = Dialog.Show(_NotifyIconView,MessageToken.DialogPageToken);
        await dialog.Initialize<NotifyIconViewModel>(
            vm => { }).GetResultAsync<CloseEnum>().ContinueWith(re => { closeEnum = re.Result; });

        if (closeEnum == CloseEnum.Close)
        {
            this.CloseWindowWithFade();
            Environment.Exit(0);
        }
        else if(closeEnum == CloseEnum.Notify)
        {
            await Task.Delay(100);
            this.Visibility = Visibility.Hidden;
        }
    }

    private void OpenOrCloseNaviBar(object sender, RoutedEventArgs e)
    {
        ChangeNavBorder();
    }


    private void ChangeNavBorder()
    {
        var width = naviGrid.Width;
        if (width == 55)
        {
            Icon_grid.Visibility = Visibility.Collapsed;
            naviGrid.Width = 200;
        }
        else
        {
            Icon_grid.Visibility = Visibility.Visible;
            naviGrid.Width = 55;
        }
    }
}