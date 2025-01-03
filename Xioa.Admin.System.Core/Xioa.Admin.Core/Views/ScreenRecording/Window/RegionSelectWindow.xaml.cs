using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Drawing;
using Xioa.Admin.Core.Views.ScreenRecording.Utils;

namespace Xioa.Admin.Core.Views.ScreenRecording.Window;

public partial class RegionSelectWindow :System.Windows. Window
{
    private System.Windows.Point startPoint;
    private bool isDrawing;
    private System.Drawing.Rectangle? selectedRegion;

    public RegionSelectWindow()
    {
        InitializeComponent();
        // 确保窗口覆盖所有屏幕
        this.Width = SystemParameters.VirtualScreenWidth;
        this.Height = SystemParameters.VirtualScreenHeight;
        this.Left = SystemParameters.VirtualScreenLeft;
        this.Top = SystemParameters.VirtualScreenTop;
    }

    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        startPoint = e.GetPosition(MainCanvas);
        isDrawing = true;
        
        // 重置选择框
        SelectionRect.Width = 0;
        SelectionRect.Height = 0;
        Canvas.SetLeft(SelectionRect, startPoint.X);
        Canvas.SetTop(SelectionRect, startPoint.Y);
        SelectionRect.Visibility = Visibility.Visible;
        SizeText.Visibility = Visibility.Visible;
        
        // 捕获鼠标
        Mouse.Capture(MainCanvas);
        e.Handled = true;
    }

    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (!isDrawing) return;

        var currentPoint = e.GetPosition(MainCanvas);
        
        // 计算选择框的位置和大小
        double width = Math.Abs(currentPoint.X - startPoint.X);
        double height = Math.Abs(currentPoint.Y - startPoint.Y);
        double left = Math.Min(startPoint.X, currentPoint.X);
        double top = Math.Min(startPoint.Y, currentPoint.Y);

        // 更新选择框
        SelectionRect.Width = width;
        SelectionRect.Height = height;
        Canvas.SetLeft(SelectionRect, left);
        Canvas.SetTop(SelectionRect, top);

        // 更新尺寸显示
        SizeText.Text = $"{(int)width} x {(int)height}";
        Canvas.SetLeft(SizeText, left);
        Canvas.SetTop(SizeText, top - 25);

        e.Handled = true;
    }

    private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!isDrawing) return;

        var currentPoint = e.GetPosition(MainCanvas);
        
        // 检查是否形成了有效的选择区域
        double width = Math.Abs(currentPoint.X - startPoint.X);
        double height = Math.Abs(currentPoint.Y - startPoint.Y);
        
        if (width > 10 && height > 10) // 确保选择区域足够大
        {
            double left = Math.Min(startPoint.X, currentPoint.X);
            double top = Math.Min(startPoint.Y, currentPoint.Y);

            // 创建选择区域
            selectedRegion = new System.Drawing.Rectangle(
                (int)left,
                (int)top,
                (int)width,
                (int)height
            );

            // 设置录制区域
            ScreenRecordHelper.SetRecordRegion(selectedRegion.Value);
            
            // 释放鼠标并关闭窗口
            Mouse.Capture(null);
            this.DialogResult = true;
            this.Close();
        }
        else
        {
            // 如果选择区域太小，重置选择
            SelectionRect.Visibility = Visibility.Collapsed;
            SizeText.Visibility = Visibility.Collapsed;
            isDrawing = false;
            Mouse.Capture(null);
        }

        e.Handled = true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            this.DialogResult = false;
            this.Close();
        }
        base.OnKeyDown(e);
    }
} 