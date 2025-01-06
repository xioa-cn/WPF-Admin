using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Xioa.Admin.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月2日
/// </summary>
public partial class RealTimeViewModel:Xioa.Admin.Core.Services.ViewModels.ViewModelBase
{
    private readonly Random _random = new();
    private readonly List<DateTimePoint> _values = new();
    private readonly DateTimeAxis _customAxis;

    public ObservableCollection<ISeries> Series { get; set; }

    public Axis[] XAxes { get; set; }

    public object Sync { get; } = new object();

    public bool IsReading { get; set; } = true;

    public RealTimeViewModel()
    {
        Series =
        new ObservableCollection<ISeries> {
            new LineSeries<DateTimePoint>
            {
                Values = _values,
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null
            }
        };

        _customAxis = new DateTimeAxis(TimeSpan.FromSeconds(1), Formatter)
        {
            CustomSeparators = GetSeparators(),
            AnimationsSpeed = TimeSpan.FromMilliseconds(0),
            SeparatorsPaint = new SolidColorPaint(SKColors.Black.WithAlpha(100))
        };

        XAxes = new Axis[] { _customAxis };

        
    }

    public async Task ReadData()
    {

        while (IsReading)
        {
            await Task.Delay(500);

            lock (Sync)
            {
                _values.Add(new DateTimePoint(DateTime.Now, _random.Next(0, 10)));
                if (_values.Count > 100) _values.RemoveAt(0);

                _customAxis.CustomSeparators = GetSeparators();
            }
        }
    }

    private static double[] GetSeparators()
    {
        var now = DateTime.Now;

        return new double[]
        {
            now.AddSeconds(-25).Ticks,
            now.AddSeconds(-20).Ticks,
            now.AddSeconds(-15).Ticks,
            now.AddSeconds(-10).Ticks,
            now.AddSeconds(-5).Ticks,
            now.Ticks
        };
    }

    private static string Formatter(DateTime date)
    {
        var secsAgo = (DateTime.Now - date).TotalSeconds;

        return secsAgo < 1
            ? "now"
            : $"{secsAgo:N0}s ago";
    }
}