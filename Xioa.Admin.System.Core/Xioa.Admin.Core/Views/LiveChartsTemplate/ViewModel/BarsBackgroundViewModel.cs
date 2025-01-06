using System;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Xioa.Admin.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月3日
/// </summary>
public partial class BarsBackgroundViewModel:Xioa.Admin.Core.Services.ViewModels.ViewModelBase
{
    public ISeries[] Series { get; set; } =
    {
        new ColumnSeries<double>
        {
            IsHoverable = false, // disables the series from the tooltips 
            Values = new Double[] { 10, 10, 10, 10, 10, 10, 10 },
            Stroke = null,
            Fill = new SolidColorPaint(new SKColor(30, 30, 30, 30)),
            IgnoresBarPosition = true
        },
        new ColumnSeries<double>
        {
            Values = new Double[] { 3, 10, 5, 3, 7, 3, 8 },
            Stroke = null,
            Fill = new SolidColorPaint(SKColors.CornflowerBlue),
            IgnoresBarPosition = true
        }
    };

    public ICartesianAxis[] YAxes { get; set; } = new[]
    {
        new Axis { MinLimit = 0, MaxLimit = 10 }
    };
}