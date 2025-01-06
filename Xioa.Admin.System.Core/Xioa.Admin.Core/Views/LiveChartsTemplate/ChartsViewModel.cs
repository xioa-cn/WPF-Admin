using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;

namespace Xioa.Admin.Core.Views.LiveChartsTemplate;

/// <summary>
/// @author Xioa
/// @date  2024年12月2日
/// </summary>
public partial class ChartsViewModel : Xioa.Admin.Core.Services.ViewModels.ViewModelBase
{
    [ObservableProperty] private object? _Content;
    [ObservableProperty] private string? _SearchContent;

    public ChartsViewModel()
    {
        Search();
        ChangeContent("MyChartsTitle");
    }
    [RelayCommand]
    public void Search()
    {
        Titles.Clear();
        if (string.IsNullOrWhiteSpace(SearchContent))
        {
            foreach (var item in _Titles)
            {
                Titles.Add(item);
            }
        }
        else
        {
            foreach (var item in _Titles)
            {
                if (item.ToLower().Contains(SearchContent.ToLower()))
                    Titles.Add(item);
            }
        }
        SearchContent = "";
    }
    public ObservableCollection<string> Titles { get; set; } = new ObservableCollection<string>();
    private string[] _Titles = new string[]
    {
        "MyChartsTitle",
        "WindDirection",
        "AutomaticUpdates",
        "SpecifyXY",
        "ScrollableCharts",
        "VisualElements",
        "BasicPolar",
        "PolarCoordinates",
        "RadialArea",
        "RealTime",
        "MultipleThreads",
        "MultipleThreads2",
        "Crosshairs",
        "BasicPie",
        "Pushout",
        "Doughnut",
        "OutsideLabels",
        "Custom",
        "SvgLabels",
        "NightingaleRose",
        "RadialGradients",
        "BarsBackground",
        "ColumnWidth",
        "DynamicVisibility",
        "DelayedAnimations",
        "BasicGauge",
        "DegreesGauge",
        "MultipleGauge",
        "SlimGauge",
        "UpdatesGauges",
        "AngularGauge",
        "WorldHeatMap",
    };

    [RelayCommand]
    public void ChangeContent(string view)
    {
        var type = GetTypeByStringName(view + "View");
        if (type is null)
        {
            Growl.Error("未找到页面");
            return;
        }

        var cont = Activator.CreateInstance(type);
        if (cont is UserControl control)
        {
            var viewModelType = GetTypeByStringName(view + "ViewModel");
            if (viewModelType is null)
            {
                Growl.Error("未找到ViewModel");
                return;
            }
            control.DataContext = Activator.CreateInstance(viewModelType);
            this.Content = control;
        }

    }


    public static Type? GetTypeByStringName(string typeName)
    {
        Type? type = null;
        Assembly[] assemblyArray = AppDomain.CurrentDomain.GetAssemblies();
        int assemblyArrayLength = assemblyArray.Length;
        for (int i = 0; i < assemblyArrayLength; ++i)
        {
            type = assemblyArray[i].GetType(typeName);
            if (type != null)
            {
                return type;
            }
        }

        for (int i = 0; (i < assemblyArrayLength); ++i)
        {
            Type?[] typeArray = assemblyArray[i].GetTypes();
            int typeArrayLength = typeArray.Length;
            for (int j = 0; j < typeArrayLength; ++j)
            {
                if (typeArray[j]!.Name.Equals(typeName))
                {
                    return typeArray[j];
                }
            }
        }

        return type;
    }
}