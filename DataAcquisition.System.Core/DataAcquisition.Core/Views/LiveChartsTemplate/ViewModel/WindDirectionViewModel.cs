using DataAcquisition.Core.Views.LiveChartsTemplate.Model;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace DataAcquisition.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月2日
/// </summary>
public partial class WindDirectionViewModel
{
    public ISeries[]? Series { get; set; }
    private LineSeries<DataPoint, ArrowGeometry>? _lineSeries;

    public WindDirectionViewModel()
    {
        _lineSeries = new LineSeries<DataPoint, ArrowGeometry>(Fetch());
       
        _lineSeries.GeometrySize = 50;
        _lineSeries.GeometryStroke = null;
        _lineSeries.GeometryFill = new SolidColorPaint(SKColors.MediumVioletRed);
        _lineSeries.Fill = null;
        _lineSeries.Mapping = (dataPoint, index) => new(index, dataPoint.Value);
        Series = new ISeries[] { _lineSeries };
    }

    public DataPoint[] Fetch()
    {
        return new DataPoint[]
        {
            new DataPoint { Value = 4, Rotation = 0 },
            new DataPoint { Value = 6, Rotation = 20 },
            new DataPoint { Value = 8, Rotation = 90 },
            new DataPoint { Value = 2, Rotation = 176 },
            new DataPoint { Value = 7, Rotation = 55 },
            new DataPoint { Value = 9, Rotation = 226 },
            new DataPoint { Value = 3, Rotation = 320 }
        };
    }
}