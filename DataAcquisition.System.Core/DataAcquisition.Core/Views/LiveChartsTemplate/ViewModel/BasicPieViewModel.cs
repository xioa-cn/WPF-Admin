using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.VisualElements;

namespace DataAcquisition.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月3日
/// </summary>
public partial class BasicPieViewModel
{
    public IEnumerable<ISeries> Series { get; set; } =
        new[] { 2, 4, 1, 4, 3 }.AsPieSeries();

    
    public IEnumerable<PieSeries<int>> Series2 { get; set; } = new List<PieSeries<int>>()
    {
        new PieSeries<int> { Name="Base1", Values = new[] { 2 } },
        new PieSeries<int> { Name="Base2",Values = new[] { 4 } },
        new PieSeries<int> { Name="Base3",Values = new[] { 1 } },
        new PieSeries<int> { Name="Base4",Values = new[] { 4 } },
        new PieSeries<int> { Name="Base5",Values = new[] { 3 } },
    };

    public LabelVisual Title { get; set; } =
        new LabelVisual
        {
            Text = "My chart title",
            TextSize = 25,
            Padding = new LiveChartsCore.Drawing.Padding(15)
        };
}