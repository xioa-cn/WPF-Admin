using System;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace DataAcquisition.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月3日
/// </summary>
public partial class CrosshairsViewModel
{
    public ISeries[] Series { get; set; } = new ISeries[] {
        new LineSeries<double> { Values = new double[]{200, 558, 458, 249, 457, 339, 587} },
        new LineSeries<double> { Values = new double[]{210, 400, 300, 350, 219, 323, 618} },
        };

    public ICartesianAxis[] XAxes { get; set; } = new Axis[] {
        new Axis
        {
            CrosshairLabelsBackground = SKColors.GreenYellow.AsLvcColor(),
            CrosshairLabelsPaint = new SolidColorPaint(SKColors.DarkRed),
            CrosshairPaint = new SolidColorPaint(SKColors.DarkOrange, 1),
            Labeler = value => value.ToString("N2")
        }
    };

    public ICartesianAxis[] YAxes { get; set; } = new Axis[] {
        new Axis
        {
            CrosshairLabelsBackground = SKColors.GreenYellow.AsLvcColor(),
            CrosshairLabelsPaint = new SolidColorPaint(SKColors.DarkRed),
            CrosshairPaint = new SolidColorPaint(SKColors.DarkOrange, 1),
            CrosshairSnapEnabled = true // snapping is also supported
        }
    };
}