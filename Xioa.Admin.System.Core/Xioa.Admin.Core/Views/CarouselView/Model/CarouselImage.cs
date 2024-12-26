using CommunityToolkit.Mvvm.ComponentModel;

namespace Xioa.Admin.Core.Views.CarouselView.Model;

/// <summary>
/// @author Xioa
/// @date  2024年12月7日
/// </summary>
public partial class CarouselImage : ObservableObject
{
    [ObservableProperty]
    private string _imageSource = string.Empty;
    
    [ObservableProperty]
    private bool _isActive;
}