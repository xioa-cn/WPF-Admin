using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace DataAcquisition.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月3日
/// </summary>
public partial class MultipleThreads2ViewModel
{
    private readonly Random _r = new();
    private readonly int _delay = 100;
    private readonly ObservableCollection<int> _values;
    private static int s_current;
    private Action<Action>? _dispatcherService;

    public ISeries[] Series { get; set; }

    public bool IsReading { get; set; } = true;

    public void Init(Action<Action>? dispatcherService)
    {
        _dispatcherService = dispatcherService;
    }

    public MultipleThreads2ViewModel()
    {
        var items = new List<int>();
        for (var i = 0; i < 1500; i++)
        {
            s_current += _r.Next(-9, 10);
            items.Add(s_current);
        }

        _values = new ObservableCollection<int>(items);


        Series = new[] {
            new LineSeries<int>
            {
                Values = _values,
                GeometryFill = null,
                GeometryStroke = null,
                LineSmoothness = 0,
                Stroke = new SolidColorPaint(SKColors.Blue, 1)
            }
        };


        _delay = 1;

    }


    public async Task ReadData()
    {
        await Task.Delay(1000);

        while (IsReading)
        {
            await Task.Delay(_delay);
            //if (_dispatcherService is not null)
            _dispatcherService!(() =>
            {
                s_current += _r.Next(-9, 10);
                _values.Add(s_current);
                _values.RemoveAt(0);
            });
            //Debug.WriteLine(DateTime.Now.ToString("U")); // 500条数据以上每秒
        }
    }
}