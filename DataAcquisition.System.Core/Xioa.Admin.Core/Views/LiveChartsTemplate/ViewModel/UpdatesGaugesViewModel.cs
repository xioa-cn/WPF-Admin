using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace Xioa.Admin.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月3日
/// </summary>
public partial class UpdatesGaugesViewModel
{
    private readonly Random _random = new();

    public ObservableValue ObservableValue1 { get; set; }
    public ObservableValue ObservableValue2 { get; set; }
    public IEnumerable<ISeries> Series { get; set; }

    public UpdatesGaugesViewModel()
    {
        ObservableValue1 = new ObservableValue { Value = 50 };
        ObservableValue2 = new ObservableValue { Value = 80 };

        Series = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(ObservableValue1, series =>
            {
                series.Name = "North";
                series.DataLabelsPosition = PolarLabelsPosition.Start;
            }),
            new GaugeItem(ObservableValue2, series =>
            {
                series.Name = "South";
                series.DataLabelsPosition = PolarLabelsPosition.Start;
            }));
    }

    [RelayCommand]
    public void DoRandomChange()
    {
        // modifying the Value property updates and animates the chart automatically
        ObservableValue1.Value = _random.Next(0, 100);
        ObservableValue2.Value = _random.Next(0, 100);
    }
}