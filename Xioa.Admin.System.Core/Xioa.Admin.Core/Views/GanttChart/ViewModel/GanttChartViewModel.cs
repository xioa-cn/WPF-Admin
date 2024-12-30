using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Linq;
using Xioa.Admin.Core.Views.ContentPage;
using GMap.NET.MapProviders;
using Xioa.Admin.Core.Views.GanttChart.Model;

namespace Xioa.Admin.Core.Views.GanttChart.ViewModel
{
    public partial class GanttChartViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<MonthInfo> _months;

        private ObservableCollection<GanttTask> _tasks;
        public ObservableCollection<GanttTask> Tasks
        {
            get => _tasks;
            set
            {
                if (_tasks != value)
                {
                    _tasks = value;
                    OnPropertyChanged();
                    //UpdateDateRange();

                    // 监听集合变化
                    if (_tasks != null)
                    {
                        _tasks.CollectionChanged += Tasks_CollectionChanged;
                    }
                }
            }
        }

        [ObservableProperty]
        private DateTime _startDate;

        [ObservableProperty]
        private DateTime _endDate;

        public GanttChartViewModel()
        {
            Tasks = new ObservableCollection<GanttTask>();
            GenerateSampleData();
            InitializeMonths();
        }

        private void Tasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //UpdateDateRange();
        }

        private void UpdateDateRange()
        {
            if (Tasks == null || !Tasks.Any())
            {
                StartDate = DateTime.Now;
                EndDate = DateTime.Now.AddMonths(1);
                return;
            }

            StartDate = Tasks.Min(t => t.StartDate);
            EndDate = Tasks.Max(t => t.EndDate);

            // 确保时间范围至少包含一个月
            if (EndDate < StartDate.AddMonths(1))
            {
                EndDate = StartDate.AddMonths(1);
            }

            InitializeMonths(); // 重新初始化月份列表
        }

        private void InitializeMonths()
        {
            Months = new ObservableCollection<MonthInfo>();

            if (Tasks == null || !Tasks.Any())
            {
                return;
            }

            // 获取最早和最晚日期，并调整到月初
            var timesStart = Tasks.Min(e => e.StartDate);
            var timesEnd = Tasks.Max(e => e.EndDate);

            // 调整开始时间到月初
            timesStart = new DateTime(timesStart.Year, timesStart.Month, 1);
            // 调整结束时间到月末的下个月初
            timesEnd = new DateTime(timesEnd.Year, timesEnd.Month, 1).AddMonths(1);

            var currentDate = timesStart;
            while (currentDate <= timesEnd)
            {
                Months.Add(new MonthInfo
                {
                    Date = currentDate,
                    Month = currentDate.ToString("yyyy年M月")
                });
                currentDate = currentDate.AddMonths(1);
            }

            // 使用月初作为基准日期计算任务条位置
            foreach (var item in Tasks)
            {
                item.SetMargin(timesStart);
            }
        }


        private void GenerateSampleData()
        {
            var monthWidth = 100.0;

            Tasks.Add(new GanttTask(
                "任务1",
                new DateTime(2020, 2, 22),
                new DateTime(2020, 9, 15),
                58,
                "#F5A623",
                monthWidth,
                TaskStatus.InProgress,
                "正在进行中，预计9月完成"
            )
            {
                TaskMen = new ObservableCollection<string> { "张三", "李四" }
            });

            Tasks.Add(new GanttTask(
                 "任务2",
                new DateTime(2020, 5, 22),
                new DateTime(2020, 9, 15),
                0,
                "#E74C3C",
                monthWidth,
                TaskStatus.Delayed,
                "由于技术原因延期"
            )
            {
                TaskMen = new ObservableCollection<string> { "王二" }
            });

            Tasks.Add(new GanttTask(
                 "任务3",
                new DateTime(2020, 3, 1),
                new DateTime(2021, 3, 2),
                100,
                "#2ECC71",
                monthWidth,
                TaskStatus.Completed,
                "按期完成"
            )
            {
                TaskMen = new ObservableCollection<string> { "张三" }
            });

            Tasks.Add(new GanttTask(
                 "任务4",
                new DateTime(2020, 8, 1),
                new DateTime(2021, 1, 1),
                0,
                "#95A5A6",
                monthWidth,
                TaskStatus.NotStarted,
                "等待前置任务完成"
            )
            {
                TaskMen = new ObservableCollection<string> { "李四" }
            });
        }
    }

    public class MonthInfo
    {
        public DateTime Date { get; set; }
        public string Month { get; set; }
    }

   

    
}
