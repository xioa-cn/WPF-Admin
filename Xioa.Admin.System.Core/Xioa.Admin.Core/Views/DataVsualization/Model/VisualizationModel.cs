using CommunityToolkit.Mvvm.ComponentModel;

namespace Xioa.Admin.Core.Views.DataVsualization.Model;

/// <summary>
/// @author Xioa
/// @date  2024年12月2日
/// </summary>
public partial class VisualizationModel : ObservableObject
{
    [ObservableProperty]private string? _Total;
    [ObservableProperty]private string? _DownloadsEE;
    [ObservableProperty]private string? _DownloadsHub;
    [ObservableProperty]private string? _ToDayVisit;
    [ObservableProperty]private string? _YesToDayVisit;
}