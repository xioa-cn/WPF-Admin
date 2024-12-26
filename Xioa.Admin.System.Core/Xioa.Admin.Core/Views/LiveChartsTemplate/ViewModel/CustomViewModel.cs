using System;
using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Xioa.Admin.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月3日
/// </summary>
public class CustomViewModel
{
    public IEnumerable<ISeries> Series { get; set; }

    public CustomViewModel()
    {
        var outer = 0;
        var data = new[] { 6, 5, 4, 3 };
        
        Series = data.AsPieSeries((value, series) =>
        {
            series.OuterRadiusOffset = outer;
            outer += 50;

            series.DataLabelsPaint = new SolidColorPaint(SKColors.White)
            {
                SKTypeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
            };

            series.ToolTipLabelFormatter =
                point =>
                {
                    var pv = point.Coordinate.PrimaryValue;
                    var sv = point.StackedValue!;

                    var a = $"{pv}/{sv.Total}{Environment.NewLine}{sv.Share:P2}";
                    return a;
                };

            series.DataLabelsFormatter =
                point =>
                {
                    var pv = point.Coordinate.PrimaryValue;
                    var sv = point.StackedValue!;

                    var a = $"{pv}/{sv.Total}{Environment.NewLine}{sv.Share:P2}";
                    return a;
                };
        });
    }
}