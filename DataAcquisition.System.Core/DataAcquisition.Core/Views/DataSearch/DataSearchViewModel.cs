using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DataAcquisition.Core.Views.DataSearch;

/// <summary>
/// @author Xioa
/// @date  2024年12月4日
/// </summary>
public partial class DataSearchViewModel : ObservableObject
{
    public ObservableCollection<object> ObjectCollection { get; set; } = new ObservableCollection<object>();


    [ObservableProperty] private DateTime _startTime = DateTime.Now;
    [ObservableProperty] private DateTime _endTime = DateTime.Now;
    [ObservableProperty] private string? _searchString;


    public DataSearchViewModel()
    {
        foreach (var item in Enumerable.Range(0,100))
        {
            ObjectCollection.Add(new { Id=item});
        }
    }


    [RelayCommand]
    private void SearchData()
    {
        Growl.Info(SearchString);
    }

    [RelayCommand]
    private void TimeSearchData()
    {
        Console.WriteLine(StartTime);
        Console.WriteLine(EndTime);
    }
}