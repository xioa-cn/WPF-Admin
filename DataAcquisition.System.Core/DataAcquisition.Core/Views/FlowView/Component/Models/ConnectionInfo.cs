using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace DataAcquisition.Core.Views.FlowView.Component.Models;

public class ConnectionInfo
{
    public FrameworkElement Source { get; set; }
    public FrameworkElement Target { get; set; }
    public Line Line { get; set; }
    public Polygon Arrow { get; set; }
    public ConnectionPointInfo SourcePoint { get; set; }
    public ConnectionPointInfo TargetPoint { get; set; }
}

public class ConnectionPointInfo
{
    public FrameworkElement Element { get; set; }
    public string Position { get; set; }
    public Point GetPosition()
    {
        var element = Element.Parent as FrameworkElement;
        if (element == null) return new Point();

        var left = Canvas.GetLeft(element);
        var top = Canvas.GetTop(element);
        var width = element.ActualWidth;
        var height = element.ActualHeight;

        var pointSize = Element.ActualWidth / 2;

        switch (Position)
        {
            case "Top":
                return new Point(left + width / 2, top + pointSize);
            case "Right":
                return new Point(left + width - pointSize, top + height / 2);
            case "Bottom":
                return new Point(left + width / 2, top + height - pointSize);
            case "Left":
                return new Point(left + pointSize, top + height / 2);
            default:
                return new Point(left + width / 2, top + height / 2);
        }
    }
} 