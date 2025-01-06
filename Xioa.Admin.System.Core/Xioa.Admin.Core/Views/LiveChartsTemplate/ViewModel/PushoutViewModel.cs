using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace Xioa.Admin.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月3日
/// </summary>
public class PushoutViewModel:Xioa.Admin.Core.Services.ViewModels.ViewModelBase
{
    public IEnumerable<ISeries> Series { get; set; } =
        new[] { 6, 5, 4, 6, 2 }.AsPieSeries((value, series) =>
        {
           
            if (value != 6) return;

            series.Pushout = 30;
        });
}