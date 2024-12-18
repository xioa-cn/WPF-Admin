using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.Defaults;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore.Measure;
using System;
using System.Threading.Tasks;
using System.Threading;
using DataAcquisition.Core.Views.DataVsualization.Model;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace DataAcquisition.Core.Views.DataVsualization;

/// <summary>
/// @author Xioa
/// @date  2024年11月29日
/// </summary>
public partial class DataVisualizationViewModel : ObservableObject
{
    public partial class DataWeek : ObservableObject
    {
        [ObservableProperty] private string? _week;
        [ObservableProperty] private bool? _showWeek = false;

        public DataWeek()
        {
        }

        public DataWeek(string week)
        {
            this.Week = week;
        }
    }

    public ObservableCollection<DataWeek> Weeks { get; set; } = new ObservableCollection<DataWeek>()
    {
        new DataWeek("未来七日") { ShowWeek = true },
        new DataWeek("近七日"),
        new DataWeek("近一月"),
        new DataWeek("近三月"),
        new DataWeek("近半年"),
        new DataWeek("近一年"),
    };

    public List<PieSeries<double>> Series { get; set; } = new List<PieSeries<double>>();
    private PieSeries<double> _series01;
    private PieSeries<double> _series02;

    public DataVisualizationViewModel()
    {
        _series01 = new PieSeries<double>()
        {
            Name = "Gitee",
            InnerRadius = 60,
            Fill = new SolidColorPaint(SKColors.MediumPurple),
            Values = new double[1] { 0 },
            DataLabelsSize = 80,
            DataLabelsFormatter = point => point.Context.Series.Name + "：" + point.Coordinate.PrimaryValue,
        };

        _series02 = new PieSeries<double>()
        {
            Name = "GitHub",
            InnerRadius = 60,
            Fill = new SolidColorPaint(SKColors.OrangeRed),
            Values = new double[1] { 0 },
            DataLabelsSize = 80,
            DataLabelsFormatter = point => point.Context.Series.Name + "：" + point.Coordinate.PrimaryValue,
        };


        Series.Add(_series01);
        Series.Add(_series02);
        
        this.ChartSeries.Add(
            new LineSeries<double, VariableSVGPathGeometry>()
            {
                GeometrySize = 20,
                GeometrySvg = SVGPoints.Star,
                Fill = new SolidColorPaint(SKColors.Transparent),
                Name = "From",
                DataLabelsFormatter =
                    point =>
                        point.Context.Series.Name + "："
                                                  + point.Coordinate.PrimaryValue,
                
            });
        ChartSeries[0].Values = new double[6]
        {
            1, 4, 6, 3, 5, 2
        };
        TextPaint2 = new SolidColorPaint()
            {
                Color = SKColors.IndianRed,
                SKTypeface = SKFontManager.Default.MatchCharacter('汉')
            };
          
        ChartSeries[0].XToolTipLabelFormatter = point =>
            $"来源";
        Change(Weeks[0]);
    }

    [ObservableProperty] private string? _github;
    [ObservableProperty] private string? _gitee;
    [ObservableProperty]
    private IPaint<LiveChartsCore.SkiaSharpView.Drawing.SkiaSharpDrawingContext> textPaint2;
    public Axis[] XAxes { get; set; } =
    {
        new Axis
        {
            Name = "X轴",
            NamePaint = new SolidColorPaint { Color = SKColors.Red },
            // 对命名标签或静态标签使用labels属性
            Labels = new string[]
            {
                "From1", "From2", "From3"
                ,"From4", "From5", "From6"
            }, 
            LabelsRotation = 15
        }
    };
    public Axis[] YAxes { get; set; } =
    {
        new Axis
        {
            Name = "Y轴",
            NamePaint = new SolidColorPaint { Color = SKColors.Red },
            // 对命名标签或静态标签使用labels属性
            LabelsRotation = 5
        }
    };
    public List<LineSeries<double, VariableSVGPathGeometry>> ChartSeries { get; set; } =
        new List<LineSeries<double, VariableSVGPathGeometry>>();

    [ObservableProperty] private VisualizationModel _visualizationModel = new();


    [RelayCommand]
    public void Change(DataWeek? dataWeek)
    {
        Random r = new Random();

        var downloadee = r.Next(1000, 9999);
        var downloadhub = r.Next(1000, 9999);
        this.VisualizationModel.Total = r.Next(1000, 9999) + "W";
        this.VisualizationModel.DownloadsEE =  downloadee+ "W";
        this.VisualizationModel.DownloadsHub = downloadhub + "W";
        this.VisualizationModel.ToDayVisit = r.Next(5000, 9999).ToString();
        this.VisualizationModel.YesToDayVisit = r.Next(10000, 50000).ToString();

        _series01.Values = new double[1] { downloadee };
        _series02.Values = new double[1] { downloadhub };

        ChartSeries[0].Values = new double[6]
        {
            r.Next(100, 500),
            r.Next(100, 500),
            r.Next(100, 500),
            r.Next(100, 500),
            r.Next(100, 500),
            r.Next(100, 500)
        };

    }
}