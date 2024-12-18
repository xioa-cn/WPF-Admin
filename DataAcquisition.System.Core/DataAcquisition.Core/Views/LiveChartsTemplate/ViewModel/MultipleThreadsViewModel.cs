using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
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
public partial class MultipleThreadsViewModel
{
    private readonly Random _r = new();
    private readonly int _delay = 100;
    private readonly ObservableCollection<int> _values;
    private int _current;

    public ISeries[] Series { get; set; }

    public object Sync { get; } = new object();

    public bool IsReading { get; set; } = true;

    public MultipleThreadsViewModel()
    {
        var items = new List<int>();
        for (var i = 0; i < 1500; i++)
        {
            _current += _r.Next(-9, 10);
            items.Add(_current);
        }

        _values = new ObservableCollection<int>(items);
        
        Series = new LineSeries<int>[] {
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

        //（lock 关键字）执行语句块，然后释放该锁。持有锁时，持有锁的线程可以再次获取和释放锁。
        // 任何其他线程都将被阻止获取锁，并等待锁被释放。lock 语句确保在任何时候最多只有一个线程执行其主体。
        while (IsReading)
        {
            await Task.Delay(_delay);

            _current = Interlocked.Add(ref _current, _r.Next(-9, 10));

            lock (Sync)
            {
                _values.Add(_current);
                _values.RemoveAt(0);
                //Debug.WriteLine(DateTime.Now.ToString("U")); // 400条数据以上每秒
            }
        }
    }
}