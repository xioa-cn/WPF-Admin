using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using static System.Net.WebRequestMethods;

namespace DataAcquisition.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月2日
/// </summary>
public class SpecifyXYViewModel
{
    public ISeries[] Series { get; set; } = new ISeries[]
    {
        new LineSeries<ObservablePoint>
        {
            Values = new ObservablePoint[]
            {
                new ObservablePoint(0, 4),
                new ObservablePoint(1, 3),
                new ObservablePoint(2, 10),
                new ObservablePoint(3, 5),
                null,
                new ObservablePoint(7, 8),
                new ObservablePoint(9, 6),
                new ObservablePoint(12, 12),
            },

            DataLabelsSize = 20,
            IsHoverable = true,
            Stroke = new SolidColorPaint(SKColors.Orange) {StrokeThickness=4},

            DataLabelsPaint = new SolidColorPaint(SKColors.Blue),
            DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Bottom,
            Name = "数据值",
            DataLabelsFormatter = point=> "Y:"+point.Coordinate.PrimaryValue.ToString("0.00")  +" "
            + "X:"+point.Coordinate.SecondaryValue.ToString("0.00"),//界面显示
            EnableNullSplitting = true, // 允许断流
            //XToolTipLabelFormatter = point=> "Y:"+point.Coordinate.PrimaryValue.ToString("0.00")  +" "
            //+ "X:"+point.Coordinate.SecondaryValue.ToString("0.00"),
            YToolTipLabelFormatter = point=> "Y:"+point.Coordinate.PrimaryValue.ToString("0.00")  +" "
            + "X:"+point.Coordinate.SecondaryValue.ToString("0.00"),//鼠标移入显示
        }
    };


}