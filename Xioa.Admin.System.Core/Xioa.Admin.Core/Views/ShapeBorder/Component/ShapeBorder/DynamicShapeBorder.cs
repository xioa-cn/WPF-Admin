using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Xioa.Admin.Core.Views.ShapeBorder.Component.ShapeBorder;

public class DynamicShapeBorder : ContentControl  // 改为继承 ContentControl
{
    static DynamicShapeBorder()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicShapeBorder),
            new FrameworkPropertyMetadata(typeof(DynamicShapeBorder)));
    }

    #region 依赖属性

    public static readonly DependencyProperty ShapePathProperty =
        DependencyProperty.Register("ShapePath", typeof(Geometry), typeof(DynamicShapeBorder),
            new PropertyMetadata(default));

    public static readonly DependencyProperty PathFillProperty =
        DependencyProperty.Register("PathFill", typeof(Brush), typeof(DynamicShapeBorder),
            new PropertyMetadata(Brushes.Transparent));

    public static readonly DependencyProperty PathStrokeProperty =
        DependencyProperty.Register("PathStroke", typeof(Brush), typeof(DynamicShapeBorder),
            new PropertyMetadata(Brushes.Black));

    public static readonly DependencyProperty PathStrokeThicknessProperty =
        DependencyProperty.Register("PathStrokeThickness", typeof(double), typeof(DynamicShapeBorder),
            new PropertyMetadata(1.0));

    // 添加 Background 属性
    public static readonly DependencyProperty BackgroundProperty =
        DependencyProperty.Register("Background", typeof(Brush), typeof(DynamicShapeBorder),
            new PropertyMetadata(null));

    #endregion

    #region 属性

    public Geometry ShapePath
    {
        get => (Geometry)GetValue(ShapePathProperty);
        set => SetValue(ShapePathProperty, value);
    }

    public Brush PathFill
    {
        get => (Brush)GetValue(PathFillProperty);
        set => SetValue(PathFillProperty, value);
    }

    public Brush PathStroke
    {
        get => (Brush)GetValue(PathStrokeProperty);
        set => SetValue(PathStrokeProperty, value);
    }

    public double PathStrokeThickness
    {
        get => (double)GetValue(PathStrokeThicknessProperty);
        set => SetValue(PathStrokeThicknessProperty, value);
    }

    public Brush Background
    {
        get => (Brush)GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    #endregion

 

   
}