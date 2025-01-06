using System;
using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Xioa.Admin.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月2日
/// </summary>
public partial class RadialAreaViewModel:Xioa.Admin.Core.Services.ViewModels.ViewModelBase
{
    private PolarLineSeries<int> value1 = new PolarLineSeries<int>
    { 
        Name = "Value1",
        Values = new int[] { 7, 7, 7, 7, 7 },
        LineSmoothness = 0,
        GeometrySize = 0,

        Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(90)),
    };

    private PolarLineSeries<int> value2 =
        new PolarLineSeries<int>
        {
            Name = "Value2",
            Values = new int[] { 2, 7, 5, 9, 7 },
            LineSmoothness = 1,
            GeometrySize = 0,
            Fill = new SolidColorPaint(SKColors.Red.WithAlpha(90))
        };

    public RadialAreaViewModel()
    {
        Series = new ISeries[]
        { 
            value1,value2
        };
        value2.ChartPointPointerHover += DataPoin2;
        value1.ChartPointPointerHover += DataPoin1;
    }

    private void DataPoin1(IChartView chart, ChartPoint<int, CircleGeometry, LabelGeometry>? point)
    {
        value1.ZIndex = 1;
        value2.ZIndex = 0;
    }

    private void DataPoin2(IChartView chart, ChartPoint<int, CircleGeometry, LabelGeometry>? point)
    {
        value1.ZIndex = 0;
        value2.ZIndex = 1;
    }

    public ISeries[] Series { get; set; } 

    public PolarAxis[] AngleAxes { get; set; } = new[]
    {
        new PolarAxis
        {
            LabelsRotation = LiveCharts.TangentAngle,
            Labels = new string[] { "数值A", "数值B", "数值C", "数值D", "数值E" },
            LabelsBackground = LvcColor.Empty
        }
    };

    public PolarAxis[] RadiusAxes { get; set; } = new[]
    {
        new PolarAxis
        {
            TextSize = 0,
            ForceStepToMin = true,
            MinStep = 1,
            SeparatorsPaint = new SolidColorPaint(SKColors.SkyBlue),
            //LabelsRotation = 0,
            //LabelsHorizontalAlignment = Align.End,
            //LabelsPadding = new Padding(20),
            //LabelsPaint = new SolidColorPaint { Color = SKColors.Transparent },
            //LabelsAngle = 1,
            ////Labels = new string[] { "", "", "", "", "" }
            //LabelsBackground  = LvcColor.FromArgb(0,0,0,0),
        }
    };
}