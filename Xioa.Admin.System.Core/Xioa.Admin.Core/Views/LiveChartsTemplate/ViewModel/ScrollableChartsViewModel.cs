using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Xioa.Admin.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月2日
/// </summary>
public partial class ScrollableChartsViewModel:Xioa.Admin.Core.Services.ViewModels.ViewModelBase
{
    private bool _isDown = false;
    private readonly ObservableCollection<ObservablePoint> _values = new ObservableCollection<ObservablePoint>();

    public ISeries[] Series { get; set; }
    public Axis[] ScrollableAxes { get; set; }
    public ISeries[] ScrollbarSeries { get; set; }
    public Axis[] InvisibleX { get; set; }
    public Axis[] InvisibleY { get; set; }
    public LiveChartsCore.Measure.Margin Margin { get; set; }
    public RectangularSection[] Thumbs { get; set; }

    public ScrollableChartsViewModel()
    {
        var trend = 1000;
        var r = new Random();

        for (var i = 0; i < 500; i++)
            _values.Add(new ObservablePoint( i,trend += r.Next(-20, 20)));

        Series = new ISeries[]{
            new LineSeries<ObservablePoint>
            {
                Values = _values,
                Name = "series",
                GeometryStroke = null,
                GeometryFill = null,
                DataPadding = new(0, 1),
                 YToolTipLabelFormatter = point=> "Y:"+point.Coordinate.PrimaryValue.ToString("0.00")  +" "
            + "X:"+point.Coordinate.SecondaryValue.ToString("0.00"),
            }
        };

        ScrollbarSeries = new ISeries[]{
            new LineSeries<ObservablePoint>
            {
                Values = _values,
                GeometryStroke = null,
                GeometryFill = null,
                DataPadding = new(0, 1)
            }
        };

        ScrollableAxes = new Axis[]
        {
            new Axis()};

        Thumbs = new RectangularSection[]
        {

            new RectangularSection
            {
                Fill = new SolidColorPaint(new SKColor(255, 205, 210, 100))
            }
            }
        ;

        InvisibleX = new Axis[]
        {
            new Axis { IsVisible = false }};
        InvisibleY = new Axis[] { new Axis { IsVisible = false } };


        var auto = LiveChartsCore.Measure.Margin.Auto;
        Margin = new(100, auto, 50, auto);
    }

    [RelayCommand]
    public void ChartUpdated(ChartCommandArgs args)
    {
        var cartesianChart = (ICartesianChartView<SkiaSharpDrawingContext>)args.Chart;

        var x = cartesianChart.XAxes.First();


        var thumb = Thumbs[0];

        thumb.Xi = x.MinLimit;
        thumb.Xj = x.MaxLimit;
    }

    [RelayCommand]
    public void PointerDown(PointerCommandArgs args)
    {
        _isDown = true;
    }

    [RelayCommand]
    public void PointerMove(PointerCommandArgs args)
    {
        if (!_isDown) return;

        var chart = (ICartesianChartView<SkiaSharpDrawingContext>)args.Chart;
        var positionInData = chart.ScalePixelsToData(args.PointerPosition);

        var thumb = Thumbs[0];
        var currentRange = thumb.Xj - thumb.Xi;

        // update the scroll bar thumb when the user is dragging the chart
        thumb.Xi = positionInData.X - currentRange / 2;
        thumb.Xj = positionInData.X + currentRange / 2;

        // update the chart visible range
        ScrollableAxes[0].MinLimit = thumb.Xi;
        ScrollableAxes[0].MaxLimit = thumb.Xj;
    }

    [RelayCommand]
    public void PointerUp(PointerCommandArgs args)
    {
        _isDown = false;
    }
}