using HarfBuzzSharp;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Animation;

namespace Xioa.Admin.Core.Views.ScreenRecording.Window;

public partial class RecordingBorderWindow :System.Windows. Window
{
    public RecordingBorderWindow()
    {
        InitializeComponent();

        // Æô¶¯±ß¿ò¶¯»­
        var storyboard = (Storyboard)FindResource("BorderAnimation");
        storyboard.Begin();
    }

    public void SetRegion(Rectangle region)
    {
        Dispatcher.Invoke(() =>
        {
            this.Left = region.Left;
            this.Top = region.Top;
            this.Width = region.Width;
            this.Height = region.Height;
        });
       
    }
} 