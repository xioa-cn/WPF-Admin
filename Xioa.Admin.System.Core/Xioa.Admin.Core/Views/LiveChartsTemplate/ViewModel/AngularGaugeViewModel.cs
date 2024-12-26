using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;

namespace Xioa.Admin.Core.Views.LiveChartsTemplate.ViewModel;

public partial class AngularGaugeViewModel
{
    private readonly Random _random = new();

    public IEnumerable<ISeries> Series { get; set; }
    public IEnumerable<VisualElement<SkiaSharpDrawingContext>> VisualElements { get; set; }
    public NeedleVisual Needle { get; set; }

    public AngularGaugeViewModel()
    {
        var sectionsOuter = 130;
        var sectionsWidth = 20;

        Needle = new NeedleVisual
        {
            Value = 45
        };

        Series = GaugeGenerator.BuildAngularGaugeSections(
            new GaugeItem(60, s => SetStyle(sectionsOuter, sectionsWidth, s)),
            new GaugeItem(30, s => SetStyle(sectionsOuter, sectionsWidth, s)),
            new GaugeItem(10, s => SetStyle(sectionsOuter, sectionsWidth, s)));

        VisualElements = new VisualElement<SkiaSharpDrawingContext>[]
        {
            new AngularTicksVisual
            {
                Labeler = value => value.ToString("N1"),
                LabelsSize = 16,
                LabelsOuterOffset = 15,
                OuterOffset = 65,
                TicksLength = 20
            },
            Needle
        };
    }

    [RelayCommand]
    public void DoRandomChange()
    {
        Needle.Value = _random.Next(0, 100);
    }

    private static void SetStyle(
        double sectionsOuter, double sectionsWidth, PieSeries<ObservableValue> series)
    {
        series.OuterRadiusOffset = sectionsOuter;
        series.MaxRadialColumnWidth = sectionsWidth;
    }
}