using System.Windows;
using Xioa.Admin.Core.Views.ScreenRecording.ViewModel;
using System.Windows.Input;
using Xioa.Admin.Core.Views.ScreenRecording.Utils;

namespace Xioa.Admin.Core.Views.ScreenRecording.Window;

public partial class ScreenRecordingTimer : System.Windows.Window {
    public ScreenRecordingTimer() {
        InitializeComponent();
    }

    private void Close_Click(object sender, RoutedEventArgs e) {
        ScreenRecordingViewModel.ElfOpen = true;
        this.Close();
    }

    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            this.DragMove();
        }
    }

    private void SelectRegion_Click(object sender, RoutedEventArgs e)
    {
        var regionWindow = new RegionSelectWindow();
        regionWindow.ShowDialog();
    }

    private void ResetRegion_Click(object sender, RoutedEventArgs e)
    {
        ScreenRecordHelper.ClearRecordRegion();
    }
}