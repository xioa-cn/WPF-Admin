using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAcquisition.Core.Views.WeldingMonitor.Model;

namespace DataAcquisition.Core.Views.WeldingMonitor.ViewModel
{
    /// <summary>
    /// 焊接监控视图模型
    /// 负责处理焊接数据的实时监控、数据展示和报警管理
    /// </summary>
    public partial class WeldingMonitorViewModel : ObservableObject
    {
        // 用于定时更新数据的计时器，间隔100ms
        private readonly DispatcherTimer _timer = new()
        {
            Interval = TimeSpan.FromMilliseconds(300)
        };

        // 用于生成随机数据的随机数生成器
        private readonly Random _random = new Random();

        // 历史数据点的最大数量
        private const int MaxHistoryPoints = 200;

        #region Observable Properties

        /// <summary>
        /// 电压历史数据集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<double> _voltageHistory = new();

        /// <summary>
        /// 电流历史数据集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<double> _currentHistory = new();

        /// <summary>
        /// 当前电压值
        /// </summary>
        [ObservableProperty]
        private double _currentVoltage;

        /// <summary>
        /// 当前电流值
        /// </summary>
        [ObservableProperty]
        private double _currentAmperage;

        /// <summary>
        /// 焊丝消耗量
        /// </summary>
        [ObservableProperty]
        private double _wireConsumption;

        /// <summary>
        /// 保护气体消耗量
        /// </summary>
        [ObservableProperty]
        private double _gasConsumption;

        /// <summary>
        /// 报警消息集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<AlarmMessage> _alarmMessages = new();

        /// <summary>
        /// 趋势图数据系列
        /// </summary>
        [ObservableProperty]
        private ISeries[] _trendSeries;

        /// <summary>
        /// X轴配置
        /// </summary>
        [ObservableProperty]
        private Axis[] _xAxes;

        /// <summary>
        /// Y轴配置
        /// </summary>
        [ObservableProperty]
        private Axis[] _yAxes;

        [ObservableProperty]
        private int totalCount;

        [ObservableProperty]
        private int okCount;

        [ObservableProperty]
        private int ngCount;

        [ObservableProperty]
        private string currentResult = "NG";

        #endregion

        /// <summary>
        /// 初始化焊接监控视图模型
        /// </summary>
        public WeldingMonitorViewModel()
        {
            InitializeData();
            InitializeCharts();
            
        }


        public void Start()
        {
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        public void End()
        {
            _timer.Tick -= Timer_Tick;
            _timer.Stop();
        }
        
        /// <summary>
        /// 初始化历史数据
        /// 生成500个初始数据点用于图表显示
        /// </summary>
        private void InitializeData()
        {
            // 初始化历史数据
            for (int i = 0; i < MaxHistoryPoints; i++)
            {
                VoltageHistory.Add(22 + _random.NextDouble() * 3);  // 电压范围：22-25V
                CurrentHistory.Add(180 + _random.NextDouble() * 40); // 电流范围：180-220A
            }
        }

        /// <summary>
        /// 初始化图表配置
        /// 设置趋势图的样式、轴和数据系列
        /// </summary>
        private void InitializeCharts()
        {
            // 配置数据系列
            TrendSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = VoltageHistory,
                    Name = "电压",
                    Stroke = new SolidColorPaint(SKColors.Cyan, 2),
                    GeometrySize = 0
                },
                new LineSeries<double>
                {
                    Values = CurrentHistory,
                    Name = "电流",
                    Stroke = new SolidColorPaint(SKColors.Gold, 2),
                    GeometrySize = 0,
                    ScalesYAt = 1  // 使用第二个Y轴
                }
            };

            // 配置X轴
            XAxes = new Axis[]
            {
                new Axis
                {
                    SeparatorsPaint = new SolidColorPaint(SKColors.White.WithAlpha(20)),
                    LabelsPaint = new SolidColorPaint(SKColors.White.WithAlpha(180)),
                    TextSize = 12
                }
            };

            // 配置Y轴
            YAxes = new Axis[]
            {
                new Axis  // 电压轴
                {
                    Name = "电压 (V)",
                    NamePaint = new SolidColorPaint(SKColors.Cyan),
                    MinLimit = 15,
                    MaxLimit = 30,
                    LabelsPaint = new SolidColorPaint(SKColors.White.WithAlpha(180)),
                    TextSize = 12,
                    Position = LiveChartsCore.Measure.AxisPosition.Start,
                    ForceStepToMin = true,
                    MinStep = 1
                },
                new Axis  // 电流轴
                {
                    Name = "电流 (A)",
                    NamePaint = new SolidColorPaint(SKColors.Gold),
                    MinLimit = 150,
                    MaxLimit = 250,
                    LabelsPaint = new SolidColorPaint(SKColors.White.WithAlpha(180)),
                    TextSize = 12,
                    Position = LiveChartsCore.Measure.AxisPosition.End,
                    ForceStepToMin = true,
                    MinStep = 10
                }
            };
        }

        /// <summary>
        /// 定时器触发事件处理
        /// 更新实时数据、图表和检查报警
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateRealTimeData();
            UpdateChartData();
            CheckAndGenerateAlarms();
        }

        /// <summary>
        /// 更新实时数据
        /// 生成新的电压、电流和消耗量数据
        /// </summary>
        private void UpdateRealTimeData()
        {
            CurrentVoltage = 22 + _random.NextDouble() * 3;    // 电压范围：22-25V
            CurrentAmperage = 180 + _random.NextDouble() * 40; // 电流范围：180-220A
            WireConsumption += 0.01 + _random.NextDouble() * 0.02;  // 焊丝消耗增量：0.01-0.03
            GasConsumption += 0.05 + _random.NextDouble() * 0.1;   // 气体消耗增量：0.05-0.15
        }

        /// <summary>
        /// 更新图表数据
        /// 移除最旧的数据点并添加新的数据点
        /// </summary>
        private void UpdateChartData()
        {
            VoltageHistory.RemoveAt(0);
            VoltageHistory.Add(CurrentVoltage);
            CurrentHistory.RemoveAt(0);
            CurrentHistory.Add(CurrentAmperage);
        }

        /// <summary>
        /// 检查并生成报警信息
        /// 有2%的概率生成随机报警
        /// </summary>
        private void CheckAndGenerateAlarms()
        {
            if (_random.NextDouble() < 0.02)  // 2%的报警概率
            {
                var messages = new[]
                {
                    "电压波动超出范围",
                    "电流波动超出范围",
                    "焊丝送进速度异常",
                    "保护气体流量偏低",
                    "焊接速度异常"
                };
                var colors = new[]
                {
                    "#20ff4444",  // 红色（降低透明度）
                    "#20ffd700",  // 金色（降低透明度）
                    "#2000ff88",  // 绿色（降低透明度）
                    "#20ff69b4",  // 粉色（降低透明度）
                    "#204169e1"   // 蓝色（降低透明度）
                };

                var index = _random.Next(messages.Length);
                AlarmMessages.Insert(0, new AlarmMessage
                {
                    Message = messages[index],
                    Time = DateTime.Now.ToString("HH:mm:ss"),
                    SeverityColor = colors[index]
                });

                // 保持最多显示10条报警信息
                while (AlarmMessages.Count > 500)
                {
                    AlarmMessages.RemoveAt(AlarmMessages.Count - 1);
                }
            }
        }
    }

   
}
