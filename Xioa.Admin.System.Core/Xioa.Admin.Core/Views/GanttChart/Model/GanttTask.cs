using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Xioa.Admin.Core.Views.GanttChart.Model
{
    public partial class GanttTask : ObservableObject
    {
        public ObservableCollection<string> TaskMen { get; set; }
        [ObservableProperty]
        private string? _name;

        [ObservableProperty]
        private DateTime _startDate;

        [ObservableProperty]
        private DateTime _endDate;

        [ObservableProperty]
        private double _progress;

        [ObservableProperty]
        private Brush _color;

        [ObservableProperty]
        private Thickness _margin;

        [ObservableProperty]
        private decimal _width;

        [ObservableProperty]
        private TaskStatus _status;

        [ObservableProperty]
        private string _statusDescription;

        public string DateRange => $"{StartDate:M.d}-{EndDate:M.d}";

        public string StatusText => GetStatusText();

        private string GetStatusText()
        {
            return Status switch
            {
                TaskStatus.NotStarted => "未开始",
                TaskStatus.InProgress => "进行中",
                TaskStatus.Delayed => "已延期",
                TaskStatus.Completed => "已完成",
                _ => string.Empty
            };
        }
        private decimal _monthWidth;
        public GanttTask(
            string name,
            DateTime startDate,
            DateTime endDate,
            double progress,
            string color,
            double monthWidth,
            TaskStatus status = TaskStatus.NotStarted,
            string statusDescription = "")
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            Progress = progress;
            Status = status;
            StatusDescription = statusDescription;

            Color = new SolidColorBrush(GetColorByStatus(status, color));

            var daysInMonth = (decimal)DateTime.DaysInMonth(startDate.Year, startDate.Month);
            _monthWidth = (decimal)monthWidth;
            decimal duration = 0;
            if (startDate.Year == endDate.Year)
            {
                duration = ((decimal)endDate.Month - (decimal)startDate.Month) >= (decimal)0 ?
             (decimal)(endDate.Month - startDate.Month - 1) : (decimal)0
              ;
            }
            else
            {
                duration = 12 - startDate.Month + endDate.Month - 1;
            }


            var startDayLength = (30 - startDate.Day) == -1 ? 1 : 30 - startDate.Day;
            var endDayLength = endDate.Day;
            var dayLength = (decimal)(startDayLength + endDayLength) / (decimal)30;
            var len = (duration + dayLength) == (decimal)0 ? (decimal)0.01 : (duration + dayLength);
            Width = len * _monthWidth;
        }

        public void SetMargin(DateTime baseDate)
        {
            // 计算月份差
            var monthsDiff = (StartDate.Year - baseDate.Year) * 12 + StartDate.Month - baseDate.Month;

            // 计算当月天数偏移比例（使用decimal确保不会丢失小数）
            var daysInMonth = DateTime.DaysInMonth(StartDate.Year, StartDate.Month);
            var dayOffset = (decimal)((StartDate.Day - 1) / (decimal)30);

            // 计算总偏移量
            var startOffset = (monthsDiff + dayOffset) * _monthWidth;

            Margin = new Thickness((double)startOffset, 0, 0, 0);
        }

        private Color GetColorByStatus(TaskStatus status, string defaultColor)
        {
            if (!string.IsNullOrEmpty(defaultColor))
            {
                return (Color)ColorConverter.ConvertFromString(defaultColor);
            }

            return status switch
            {
                TaskStatus.NotStarted => (Color)ColorConverter.ConvertFromString("#95A5A6"),  // 灰色
                TaskStatus.InProgress => (Color)ColorConverter.ConvertFromString("#3498DB"),  // 蓝色
                TaskStatus.Delayed => (Color)ColorConverter.ConvertFromString("#E74C3C"),     // 红色
                TaskStatus.Completed => (Color)ColorConverter.ConvertFromString("#2ECC71"),   // 绿色
                _ => Colors.Gray
            };
        }
    }
}
