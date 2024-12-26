using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace Xioa.Admin.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月3日
/// </summary>
public partial class DynamicVisibilityViewModel
{
    public ISeries[] Series { get; set; } =
    {
        new ColumnSeries<double>
        {
            Values = new double[] { 2, 5, 4, 3 },
            IsVisible = true
        },
        new ColumnSeries<double>
        {
            Values = new double[] { 6, 3, 2, 8 },
            IsVisible = true
        },
        new ColumnSeries<double>
        {
            Values = new double[] { 4, 2, 8, 7 },
            IsVisible = true
        }
    };

    [RelayCommand]
    public void ToggleSeries0() =>
        Series[0].IsVisible = !Series[0].IsVisible;

    [RelayCommand]
    public void ToggleSeries1() =>
        Series[1].IsVisible = !Series[1].IsVisible;

    [RelayCommand]
    public void ToggleSeries2() =>
        Series[2].IsVisible = !Series[2].IsVisible;
}