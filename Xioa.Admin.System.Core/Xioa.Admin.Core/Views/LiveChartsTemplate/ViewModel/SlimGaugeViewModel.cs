using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace Xioa.Admin.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月3日
/// </summary>
public partial class SlimGaugeViewModel:Xioa.Admin.Core.Services.ViewModels.ViewModelBase
{
    public IEnumerable<ISeries> Series { get; set; } =
        GaugeGenerator.BuildSolidGauge(
            new GaugeItem(50, series => SetStyle("Vanessa", series)),
            new GaugeItem(80, series => SetStyle("Charles", series)),
            new GaugeItem(100, series => SetStyle("Ana", series)),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.Fill = null;
            }));

    public static void SetStyle(string name, PieSeries<ObservableValue> series)
    {
        series.Name = name;
        series.DataLabelsSize = 20;
        series.DataLabelsPosition = PolarLabelsPosition.End;
        series.DataLabelsFormatter =
            point => series.Name + ":" + point.Coordinate.PrimaryValue.ToString();
        series.InnerRadius = 20;
        series.MaxRadialColumnWidth = 5;
    }
}