using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.Drawing;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Painting;

namespace DataAcquisition.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月2日
/// </summary>
public partial class MyChartsTitleViewModel : ObservableObject
{
    LineSeries<double, VariableSVGPathGeometry> series1 = new LineSeries<double, VariableSVGPathGeometry>
    {
        Values = new double[] { 2, 1, 3, 5, 3, 4, 6 },
        Fill = null,

        GeometrySvg = SVGPoints.Star,
        GeometrySize = 20
    };
    LineSeries<double, VariableSVGPathGeometry> series2 = new LineSeries<double, VariableSVGPathGeometry>
    {
        Values = new double[] { 6, 4, 2, 1, 5, 3, 2 },
        Fill = null,

        GeometrySvg = SVGPoints.Star,
        GeometrySize = 20
    };

    public ISeries[] Series { get; set; } = new LineSeries<double, VariableSVGPathGeometry>[2];

    public MyChartsTitleViewModel()
    {
        Series[0] = series1;
        Series[1] = series2;
    }

    public LabelVisual Title { get; set; } =
        new LabelVisual
        {
            Text = "My chart title",
            TextSize = 25,
            Padding = new LiveChartsCore.Drawing.Padding(15)
        };

    [RelayCommand]
    public void ChangeLineSmoothness()
    {
        if (series1.LineSmoothness == 0)
        {
            series1.LineSmoothness = 1;
            series2.LineSmoothness = 1;
        }
        else
        {
            series1.LineSmoothness = 0;
            series2.LineSmoothness = 0;
        }
    }

    SolidColorPaint color = new SolidColorPaint(SKColors.CornflowerBlue);
    [RelayCommand]
    public void Fill()
    {
        if (series1.Fill == color)
        {
            series1.Fill = new SolidColorPaint(SKColors.Transparent);
            series2.Fill = new SolidColorPaint(SKColors.Transparent);
        }
        else
        {
            series1.Fill = color;
            series2.Fill = new SolidColorPaint(SKColors.OrangeRed);
        }
    }
}