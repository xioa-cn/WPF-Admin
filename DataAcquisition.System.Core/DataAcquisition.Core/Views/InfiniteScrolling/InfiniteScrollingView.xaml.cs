using System.Windows.Controls;

namespace DataAcquisition.Core.Views.InfiniteScrolling;

/// <summary>
/// @author Xioa
/// @date  2024-12-26 09:08:35
/// </summary>
public partial class InfiniteScrollingView : Page {
    public InfiniteScrollingView() {
        InitializeComponent();
    }

    private async void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e) {
        var scrollViewer = (ScrollViewer)sender;
        
        if (!(scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight)) return;
        if (this.DataContext == null || this.DataContext is not InfiniteScrollingViewModel viewModel) return;
      
        if (viewModel.CanMove)
        {
            await viewModel.MoveData();
        }
    }
}