using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace DataAcquisition.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月3日
/// </summary>
public partial class ColumnWidthViewModel
{
    public ISeries[] Series { get; set; } =
    {
        new ColumnSeries<double>
        {
            Values = new double[]
            {
                20, 50, 40, 20, 40, 30, 50, 20, 50, 40
            },

            // Defines the distance between every bars in the series
            Padding = 0,

            // Defines the max width a bar can have
            MaxBarWidth = double.MaxValue
        }
    };
}