using SkiaSharp;

namespace DataAcquisition.Core.Views.LiveChartsTemplate.Model;

public class BrowserShare
{
    public string Name { get; set; }
    public SKPath Svg { get; set; }
    public double Value { get; set; }
}