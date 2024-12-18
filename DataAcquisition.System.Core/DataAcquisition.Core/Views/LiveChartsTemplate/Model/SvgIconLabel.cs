using LiveChartsCore.Drawing;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using SkiaSharp;

namespace DataAcquisition.Core.Views.LiveChartsTemplate.Model;

/// <summary>
/// @author Xioa
/// @date  2024年12月3日
/// </summary>
public class SvgIconLabel: VariableSVGPathGeometry, ILabelGeometry<SkiaSharpDrawingContext>
{
    public string Name { get; set; } = string.Empty;

    public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
    {
        // lets draw the icon using the VariableSVGPathGeometry base class.
        base.OnDraw(context, paint);

        // and after that, lets draw the name of the browser using the SkiaSharp API.

        using var textPaint = new SKPaint();
        textPaint.Color = SKColors.WhiteSmoke;
        textPaint.Style = SKPaintStyle.Fill;
        textPaint.TextSize = 16;
        textPaint.FakeBoldText = true;
        textPaint.IsAntialias = true;

        context.Canvas.DrawText(Name, X, Y - 10, textPaint);
    }

    // All the folowing properties and ctor are required by the ILabelGeometry interface
    // in this case we will ignore them.

    public SvgIconLabel()
    {
        // just a workaround... probably will be imprived in a future version.
        _ = RegisterMotionProperty(new FloatMotionProperty(nameof(TextSize), 11));
    }

    public Padding Padding { get; set; } = new();
    public float LineHeight { get; set; }
    public Align VerticalAlign { get; set; }
    public Align HorizontalAlign { get; set; }
    public string Text { get; set; } = string.Empty;
    public float TextSize { get; set; }
    public float MaxWidth { get; set; }
    public LvcColor Background { get; set; }
}