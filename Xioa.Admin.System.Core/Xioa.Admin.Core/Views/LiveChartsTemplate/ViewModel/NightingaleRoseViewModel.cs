using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace Xioa.Admin.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月3日
/// </summary>
public partial class NightingaleRoseViewModel:Xioa.Admin.Core.Services.ViewModels.ViewModelBase
{
    public IEnumerable<ISeries> Series { get; set; }

    public NightingaleRoseViewModel()
    {
        var outer = 0;
        var data = new[] { 6, 5, 4, 3, 2 };
        Series = data.AsPieSeries((value, series) =>
        {
            series.InnerRadius = 50;
            series.OuterRadiusOffset = outer;
            outer += 50;
        });
    }
}