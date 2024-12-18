using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using DataAcquisition.Core.Views.CarouselView.Model;

namespace DataAcquisition.Core.Views.CarouselView;

/// <summary>
/// @author Xioa
/// @date  2024年12月7日
/// </summary>
public partial class CarouselViewModel : ObservableObject, IDisposable
{
    private DispatcherTimer? _timer;
    private bool _disposed;

    [ObservableProperty]
    private bool _isAutoPlaying;

    public ObservableCollection<CarouselImage> Images { get; } = new();

    public CarouselViewModel()
    {
        // 加载示例图片
        LoadSampleImages();
    }


    public void InitTimer()
    {
        // 初始化计时器
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += Timer_Tick;


    }


    private void LoadSampleImages()
    {
        try
        {
            // 测试用的本地图片
            var image1 = new CarouselImage
            {
                ImageSource = "/DataAcquisition.Core;component/Assets/Img/Carousel/sample1.jpg",
                IsActive = true
            };
            var image2 = new CarouselImage
            {
                ImageSource = "/DataAcquisition.Core;component/Assets/Img/Carousel/sample2.jpg"
            };
            var image3 = new CarouselImage
            {
                ImageSource = "/DataAcquisition.Core;component/Assets/Img/Carousel/sample3.jpg"
            };

            Images.Add(image1);
            Images.Add(image2);
            Images.Add(image3);

            // 打印日志以便调试
            System.Diagnostics.Debug.WriteLine($"Loaded image 1: {image1.ImageSource}");
            System.Diagnostics.Debug.WriteLine($"Loaded image 2: {image2.ImageSource}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading images: {ex.Message}");
        }
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        NextCommand.Execute(null);
    }

    [RelayCommand]
    private void ToggleAutoPlay()
    {
        IsAutoPlaying = !IsAutoPlaying;
        if (IsAutoPlaying)
            _timer.Start();
        else
            _timer.Stop();
    }

    [RelayCommand]
    private void Next()
    {
        int currentIndex = GetCurrentIndex();
        int nextIndex = (currentIndex + 1) % Images.Count;
        SetActiveImage(nextIndex);
    }

    [RelayCommand]
    private void Previous()
    {
        int currentIndex = GetCurrentIndex();
        int previousIndex = (currentIndex - 1 + Images.Count) % Images.Count;
        SetActiveImage(previousIndex);
    }

    [RelayCommand]
    private void SelectImage(CarouselImage image)
    {
        foreach (var item in Images)
        {
            item.IsActive = false;
        }
        image.IsActive = true;
    }

    private int GetCurrentIndex()
    {
        for (int i = 0; i < Images.Count; i++)
        {
            if (Images[i].IsActive)
                return i;
        }
        return 0;
    }

    private void SetActiveImage(int index)
    {
        foreach (var image in Images)
        {
            image.IsActive = false;
        }
        Images[index].IsActive = true;
    }

    public void Dispose()
    {
        IsAutoPlaying = false;
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {

        // 清理托管资源
        _timer.Stop();
        _timer.Tick -= Timer_Tick;
        _timer = null;
    }

    ~CarouselViewModel()
    {
        Dispose(false);
    }
}

