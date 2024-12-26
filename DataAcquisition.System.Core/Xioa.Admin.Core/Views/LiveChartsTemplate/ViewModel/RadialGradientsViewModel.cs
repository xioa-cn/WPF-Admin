using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Xioa.Admin.Core.Views.LiveChartsTemplate.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月3日
/// </summary>
public partial class RadialGradientsViewModel
{
    private static readonly SKColor[] s_colors =new SKColor[] {
        new SKColor(179, 229, 252),
        new SKColor(1, 87, 155),
        SKColors.Black, 
        SKColors.GreenYellow, 
        SKColors.Aqua, 
        // ...

        // you can add as many colors as you require to build the gradient
        // by default all the distance between each color is equal
        // use the colorPos parameter in the constructor of the RadialGradientPaint class
        // to specify the distance between each color
    };

    public ISeries[] Series { get; set; } = new [] {
        new PieSeries<int>
        {
            Name = "Maria",
            Values = new int[] {7},
            Stroke = null,
            Fill = new RadialGradientPaint(s_colors),
            Pushout = 10,
            OuterRadiusOffset = 20
        },
        new PieSeries<int>
        {
            Name = "Charles",
            Values = new int[] {3},
            Stroke = null,
            Fill = new RadialGradientPaint(new SKColor(255, 205, 210), new SKColor(183, 28, 28))
        }
    };
}