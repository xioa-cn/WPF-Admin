using System;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;

namespace Xioa.Admin.Core.Views.LiveChartsTemplate.ViewModel;

public partial class BasicPolarViewModel
{
    public ISeries[] Series { get; set; } = new [] {
        new PolarLineSeries<double>
        {
            Values = new double[] {16,15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1},
            DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30)),
            GeometrySize = 15,
            DataLabelsSize = 8,
            DataLabelsPosition = PolarLabelsPosition.Middle,
            DataLabelsRotation = LiveCharts.CotangentAngle,
            IsClosed = true
        }
    };

    public PolarAxis[] RadialAxes { get; set; } =  new []{
        new PolarAxis
        {
            LabelsAngle = -60,
            MaxLimit = 30, // null to let the chart autoscale (defualt is null) 
        }
    };

    public PolarAxis[] AngleAxes { get; set; } = new []{
        new PolarAxis
        {
            LabelsRotation = LiveCharts.TangentAngle,
            //IsInverted = true // enables counter clockwise draw. 
        }
    };

    public LabelVisual Title { get; set; } =
        new LabelVisual
        {
            Text = "My chart title",
            TextSize = 25,
            Padding = new LiveChartsCore.Drawing.Padding(15),
            Paint = new SolidColorPaint(SKColors.DarkSlateGray)
        };
}