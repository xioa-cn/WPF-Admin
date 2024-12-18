using System.Collections.Generic;
using DataAcquisition.Core.Views.LiveChartsTemplate.Model;
using LiveChartsCore;
using LiveChartsCore.ConditionalDraw;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace DataAcquisition.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月3日
/// </summary>
public partial class SvgLabelsViewModel
{
    public IEnumerable<ISeries> Series { get; set; }

    public SvgLabelsViewModel()
    {
        BrowserShare[] data = new BrowserShare[] {
            new() { Name = "Chrome", Value = 65.72, Svg = Icons.Chrome },
            new() { Name = "Safari", Value = 18.22, Svg = Icons.Safari },
            new() { Name = "Edge", Value = 5.31, Svg = Icons.Edge }
        };
        
        Series = data.AsPieSeries<BrowserShare, DoughnutGeometry, SvgIconLabel>(
            (dataItem, series) =>
            {
                series.DataLabelsPaint = new SolidColorPaint(SKColors.WhiteSmoke);
                
                series
                    .OnPointMeasured(point =>
                    {
                        var svgLabel = point.Label!;

                        svgLabel.Path = dataItem.Svg;
                        svgLabel.Name = dataItem.Name;
                        svgLabel.Width = 30;
                        svgLabel.Height = 30;
                        svgLabel.TranslateTransform = new(-15, -15);
                    });
            });

       
        LiveCharts.Configure(config => config
            .HasMap<BrowserShare>((point, index) => new(index, point.Value)));
    }
}