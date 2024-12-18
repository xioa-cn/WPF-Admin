using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DataAcquisition.Core.Views.AntDiagram;

public partial class AntDiagramView : Page
{
    private DoubleAnimation? dashOffsetAnimation;
    private DoubleAnimation? dashOffsetAnimation1;
    public AntDiagramView()
    {
        InitializeComponent();
        InitializeAnimation();
    }

    private void InitializeAnimation()
    {
        // 创建蚂蚁线动画
        dashOffsetAnimation = new DoubleAnimation
        {
            From = 0,
            To = 8, // 虚线模式的总长度   如下 4+4 = 8
            //  < Path.StrokeDashArray >
            //            < DoubleCollection > 4,4 </ DoubleCollection >
            //  </ Path.StrokeDashArray >
            Duration = TimeSpan.FromSeconds(0.5),
            RepeatBehavior = RepeatBehavior.Forever
        };
        dashOffsetAnimation1 = new DoubleAnimation
        {
            From = 8,
            To = 0, // 虚线模式的总长度
            Duration = TimeSpan.FromSeconds(1),
            RepeatBehavior = RepeatBehavior.Forever
        };
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        // 开始动画
        AntLine.BeginAnimation(System.Windows.Shapes.Shape.StrokeDashOffsetProperty, dashOffsetAnimation);
        // 开始动画
        PipeLine.BeginAnimation(Shape.StrokeDashOffsetProperty, dashOffsetAnimation1);
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        // 停止动画
        AntLine.BeginAnimation(System.Windows.Shapes.Shape.StrokeDashOffsetProperty, null);
        PipeLine.BeginAnimation(Shape.StrokeDashOffsetProperty, null);
    }
}