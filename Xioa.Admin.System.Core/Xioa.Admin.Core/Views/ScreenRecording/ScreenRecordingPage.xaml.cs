using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Xioa.Admin.Core.Views.ScreenRecording.ViewModel;

namespace Xioa.Admin.Core.Views.ScreenRecording;

public partial class ScreenRecordingPage : Page {
    private Point startPoint;

    public ScreenRecordingPage() {
        InitializeComponent();
    }

    private void Border_MouseDown(object sender, MouseButtonEventArgs e) {
        // Start point of the rectangle
        startPoint = e.GetPosition(sender as Border);
        SelectionRectangle.Visibility = Visibility.Visible;
        SelectionRectangle.Width = 0;
        SelectionRectangle.Height = 0;
        Canvas.SetLeft(SelectionRectangle, startPoint.X);
        Canvas.SetTop(SelectionRectangle, startPoint.Y);
    }

    private void Border_MouseMove(object sender, MouseEventArgs e) {
        if (e.LeftButton == MouseButtonState.Pressed) {
            // Update the width and height of the rectangle as the mouse moves
            Point currentPoint = e.GetPosition(sender as Border);
            double x = Math.Min(currentPoint.X, startPoint.X);
            double y = Math.Min(currentPoint.Y, startPoint.Y);
            double width = Math.Max(currentPoint.X, startPoint.X) - x;
            double height = Math.Max(currentPoint.Y, startPoint.Y) - y;

            SelectionRectangle.Width = width;
            SelectionRectangle.Height = height;
            Canvas.SetLeft(SelectionRectangle, x);
            Canvas.SetTop(SelectionRectangle, y);
        }
    }

    private void Border_MouseUp(object sender, MouseButtonEventArgs e) {
        // Finalize the rectangle and set the region in the ViewModel
        if (DataContext is ScreenRecordingViewModel viewModel) {
            viewModel.RecordingRegion = new Rect(Canvas.GetLeft(SelectionRectangle), Canvas.GetTop(SelectionRectangle), SelectionRectangle.Width, SelectionRectangle.Height);
        }
        SelectionRectangle.Visibility = Visibility.Collapsed; // Hide rectangle after selection
    }
}