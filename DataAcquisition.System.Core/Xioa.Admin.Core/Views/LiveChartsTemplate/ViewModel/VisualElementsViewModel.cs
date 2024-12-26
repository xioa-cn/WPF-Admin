using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;
using SkiaSharp;

namespace Xioa.Admin.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月2日
/// </summary>
public partial class VisualElementsViewModel
{
    public IEnumerable<ChartElement<SkiaSharpDrawingContext>> VisualElements { get; set; }

    public ISeries[] Series { get; set; }

    private void RectanglePointerDown(
        VisualElement<SkiaSharpDrawingContext> visual,
        VisualElementEventArgs<SkiaSharpDrawingContext> visualElementsArgs)
    {
        //var rectangleVisual = (GeometryVisual<RectangleGeometry>)visual;

        //// toggle the width of the rectangle
        //rectangleVisual.Width = rectangleVisual is { Width: 4 } ? 2 : 4;
    }

    GeometryVisual<RectangleGeometry> rectangleVisual;
    SolidColorPaint tran = new SolidColorPaint(SKColors.Transparent) { ZIndex = -10 };
    SolidColorPaint red = new SolidColorPaint(SKColors.Firebrick) { ZIndex = -10 };
    SolidColorPaint green = new SolidColorPaint(SKColors.PaleGreen) { ZIndex = -10 };

    public VisualElementsViewModel()
    {
        var visuals = new List<ChartElement<SkiaSharpDrawingContext>>();

        rectangleVisual = new GeometryVisual<RectangleGeometry>
        {
            X = 2,
            Y = 6,
            LocationUnit = MeasureUnit.ChartValues,
            Width = 2,
            Height = 2,
            SizeUnit = MeasureUnit.ChartValues,
            Fill = tran,
            Stroke = new SolidColorPaint(SKColors.Red)
            {
                ZIndex = -10,
                StrokeThickness = 3f,
            },
            Label = "框1",
            LabelPaint = new SolidColorPaint(SKColors.Black) { ZIndex = -11 },
            LabelSize = 12
        };
        // 让rectangleVisual的四条边框有不同的颜色
        //rectangleVisual.Stroke = new SolidColorPaint(SKColors.Red) { ZIndex = -10, StrokeThickness = 3f };
        //rectangleVisual.Stroke = new SolidColorPaint(SKColors.Green) { ZIndex = -10, StrokeThickness = 3f };
        //rectangleVisual.Stroke = new SolidColorPaint(SKColors.Blue) { ZIndex = -10, StrokeThickness = 3f };
        //rectangleVisual.Stroke = new SolidColorPaint(SKColors.Yellow) { ZIndex = -10, StrokeThickness = 3f };
        
        // listen for the pointer down event
        rectangleVisual.PointerDown += RectanglePointerDown;
        visuals.Add(rectangleVisual);

        //var labelVisual = new LabelVisual
        //{
        //    Text = "What happened here?",
        //    X = 3,
        //    Y = 2,
        //    TextSize = 5,
        //    Paint = new SolidColorPaint(SKColors.Red) { ZIndex = 11 },
        //    BackgroundColor = new LvcColor(55, 71, 79),
        //    Padding = new Padding(1),
        //    LocationUnit = MeasureUnit.ChartValues,
        //    Translate = new LvcPoint(0, -35)
        //};
        //visuals.Add(labelVisual);

        //var svgVisiual = new SVGVisual
        //{
        //    Path = SKPath.ParseSvgPathData(SVGPoints.Star),
        //    X = 80,
        //    Y = 80,
        //    LocationUnit = MeasureUnit.Pixels,
        //    Width = 100,
        //    Height = 100,
        //    SizeUnit = MeasureUnit.Pixels,
        //    Fill = new SolidColorPaint(new SKColor(251, 192, 45, 50)) { ZIndex = 10 },
        //    Stroke = new SolidColorPaint(new SKColor(251, 192, 45)) { ZIndex = 10, StrokeThickness = 1.5f },
        //    Label = "This one is fixed",
        //    LabelPaint = new SolidColorPaint(SKColors.Black) { ZIndex = 11 },
        //    LabelSize = 10
        //};

        //visuals.Add(svgVisiual);

        VisualElements = visuals;
        Series = new ISeries[]
        {
            new LineSeries<int>
            {
                GeometrySize = 13,
                Values = new int[7]
                {
                    4,
                    6,
                    8,
                    2,
                    7,
                    9,
                    3,
                },
            }
        };
    }

    [RelayCommand]
    public void ChangeValue(string color)
    {
        if (color == "tran")
        {
            rectangleVisual.Fill = tran;
        }
        else if (color == "red")
        {
            rectangleVisual.Fill = red;
        }
        else if (color == "green")
        {
            rectangleVisual.Fill = green;
        }
    }
}