using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace Xioa.Admin.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月3日
/// </summary>
public partial class WorldHeatMapViewModel:Xioa.Admin.Core.Services.ViewModels.ViewModelBase
{
    private bool _isBrazilInChart = true;
    private readonly HeatLand _brazil;
    private readonly Random _r = new();

    public WorldHeatMapViewModel()
    {
        var lands = new HeatLand[]
        {
            // new() { Name = "bra", Value = 13 },
            // new() { Name = "mex", Value = 10 },
             new() { Name = "usa", Value = 15 },
            //  new() { Name = "can", Value = 8 },
            //  new() { Name = "ind", Value = 12 },
            //  new() { Name = "deu", Value = 13 },
            //  new() { Name= "jpn", Value = 15 },
             new() { Name = "chn", Value = 14 },
             // new() { Name = "rus", Value = 11 },
             // new() { Name = "fra", Value = 8 },
             // new() { Name = "esp", Value = 7 },
             // new() { Name = "kor", Value = 10 },
             // new() { Name = "zaf", Value = 12 },
             // new() { Name = "are", Value = 13 }
        };

        Series = new[] { new HeatLandSeries { Lands = lands } };

        _brazil = lands.First(x => x.Name == "chn");
        //DoRandomChanges();
    }
    public HeatLandSeries[] Series { get; set; }

    [RelayCommand]
    public void ToggleBrazil()
    {
        var lands = Series[0].Lands;
        if (lands is null) return;

        if (_isBrazilInChart)
        {
            Series[0].Lands = lands.Where(x => x != _brazil).ToArray();
            _isBrazilInChart = false;
            return;
        }

        Series[0].Lands = lands;
        Series[0].Lands.Add(_brazil);
        _isBrazilInChart = true;
    }
    public bool IsStart { get; set; } = false;
    public async void DoRandomChanges()
    {
        await Task.Delay(1000);

        while (IsStart)
        {
            foreach (var shape in Series[0].Lands ?? Enumerable.Empty<IWeigthedMapLand>())
            {
                shape.Value = _r.Next(-0, 20);
            }

            await Task.Delay(500);
        }
    }
}