using System;
using System.Windows;
using System.Windows.Controls;

namespace DataAcquisition.Core.Views.CarouselView;

public partial class CarouselView : Page, IDisposable
{
    private readonly CarouselViewModel _viewModel;
    private bool _disposed;

    public CarouselView()
    {
        InitializeComponent();
        _viewModel = new CarouselViewModel();
        DataContext = _viewModel;

        Loaded += CarouselView_Loaded;
        Unloaded += CarouselView_Unloaded;
    }

    private void CarouselView_Loaded(object sender, RoutedEventArgs e)
    {
        if (_disposed)
        {
            _disposed = false;
        }
        _viewModel.InitTimer();
    }

    private void CarouselView_Unloaded(object sender, RoutedEventArgs e)
    {
        Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // 清理托管资源
                _viewModel.Dispose();
            }
            _disposed = true;
        }
    }

    ~CarouselView()
    {
        Dispose(false);
    }
}
