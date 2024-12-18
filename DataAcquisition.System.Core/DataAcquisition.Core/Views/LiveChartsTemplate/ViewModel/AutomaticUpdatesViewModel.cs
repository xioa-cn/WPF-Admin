using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace DataAcquisition.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月2日
/// </summary>
public partial class AutomaticUpdatesViewModel
{
    private readonly Random _random = new();

  
    public ObservableCollection<ISeries> Series { get; set; }

   
    public ObservableCollection<ObservableValue> ObservableValues { get; set; }

    public AutomaticUpdatesViewModel()
    {
        ObservableValues = new ObservableCollection<ObservableValue>();
        ObservableValues.Add(new() { Value = 2 });
        ObservableValues.Add(new() { Value = 5 });
        ObservableValues.Add(new() { Value = 4 });
        Series = new ObservableCollection<ISeries>() { new LineSeries<ObservableValue>(ObservableValues) };
    }
    
    [RelayCommand]
    public void AddItem()
    {
        var randomValue = _random.Next(1, 10);

       
        ObservableValues.Add(new() { Value = randomValue });
    }

    [RelayCommand]
    public void RemoveItem()
    {
        if (ObservableValues.Count == 0) return;
        ObservableValues.RemoveAt(0);
    }

    [RelayCommand]
    public void UpdateItem()
    {
        var randomValue = _random.Next(1, 10);
        var lastItem = ObservableValues[ObservableValues.Count - 1];
        
        lastItem.Value = randomValue;
    }

    [RelayCommand]
    public void ReplaceItem()
    {
        var randomValue = _random.Next(1, 10);
        var randomIndex = _random.Next(0, ObservableValues.Count - 1);
        
        ObservableValues[randomIndex] = new(randomValue);
    }

    [RelayCommand]
    public void AddSeries()
    {
        var values = Enumerable.Range(0, 3)
            .Select(_ => _random.Next(0, 10))
            .ToArray();
        
        Series.Add(new LineSeries<int>(values));
    }

    [RelayCommand]
    public void RemoveSeries()
    {
        if (Series.Count == 1) return;
        
        Series.RemoveAt(Series.Count - 1);
    }
    
    
    
}