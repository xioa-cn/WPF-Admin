using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;
using HandyControl.Controls;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace Xioa.Admin.Core.Views.GetColorUtil.Component;

public partial class GetColors : UserControl
{
    public ICommand CopyColorCommand { get; set; }
    private DispatcherTimer timer;

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("gdi32.dll")]
    private static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    public GetColors()
    {
        InitializeComponent();
        InitializeTimer();

        this.Loaded += (s, e) =>
        {
            this.Focus();
            Keyboard.Focus(this);
        };

        this.Focusable = true;
        this.IsTabStop = true;

        this.PreviewKeyDown += GetColors_KeyDown;

        this.PreviewKeyDown += (s, e) =>
        {
            if (e.Key == Key.System)
            {
                e.Handled = true;
            }
        };

        this.LostFocus += GetColors_LostFocus;
        this.GotFocus += GetColors_GotFocus;
        soueceColor.ItemsSource = colors;
        CopyColorCommand = new RelayCommand<string>(CopyColor);

        // 添加 CheckBox 状态改变事件处理
        //GetForuse.Checked += GetForuse_CheckedChanged;
        GetForuse.Unchecked += GetForuse_UnCheckedChanged;
    }

    private void GetForuse_UnCheckedChanged(object sender, RoutedEventArgs e)
    {
        if (JustNowGotFocus)
            Dispatcher.InvokeAsync(async () =>
            {
                await Task.Delay(50);
                GetForuse.IsChecked = true;
            });
    }

    private void GetForuse_CheckedChanged(object sender, RoutedEventArgs e)
    {

    }
    private bool JustNowGotFocus = false;
    private void GetColors_GotFocus(object sender, RoutedEventArgs e)
    {
        JustNowGotFocus = true;
        var result = this.Focusable;
        Dispatcher.InvokeAsync(async () =>
        {
            await Task.Delay(100);
            GetForuse.IsChecked = true;
        });
    }

    private void GetColors_LostFocus(object sender, RoutedEventArgs e)
    {
        JustNowGotFocus = false;
        var result = this.Focusable;
        GetForuse.IsChecked = false;
    }



    private void CopyColor(string? obj)
    {
        if (obj == null) return;
        Clipboard.SetText(obj);
    }

    public ObservableCollection<string> colors { get; set; } = new ObservableCollection<string>();

    private void InitializeTimer()
    {
        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(50);
        timer.Tick += Timer_Tick;
        timer.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        POINT point;
        GetCursorPos(out point);

        IntPtr desk = GetDC(IntPtr.Zero);
        uint pixel = GetPixel(desk, point.X, point.Y);
        ReleaseDC(IntPtr.Zero, desk);

        byte r = (byte)(pixel & 0x000000FF);
        byte g = (byte)((pixel & 0x0000FF00) >> 8);
        byte b = (byte)((pixel & 0x00FF0000) >> 16);

        Color color = Color.FromRgb(r, g, b);
        string hexColor = $"#{color.R:X2}{color.G:X2}{color.B:X2}";

        ColorPreview.Fill = new SolidColorBrush(color);
        ColorCode.Text = hexColor;
    }

    private void GetColors_KeyDown(object sender, KeyEventArgs e)
    {       
        Debug.WriteLine($"KeyDown triggered: {e.Key}, Modifiers: {Keyboard.Modifiers}, SystemKey: {e.SystemKey}");

        // 检查是否是 Alt+C
        if ((e.Key == Key.System && e.SystemKey == Key.C) ||
            (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt))
        {
            Clipboard.SetText(ColorCode.Text);
            colors.Insert(0, ColorCode.Text);
            e.Handled = true;
        }
    }

    // 添加获取焦点的公共方法
    public void SetFocus()
    {
        this.Focus();
        Keyboard.Focus(this);
    }
}