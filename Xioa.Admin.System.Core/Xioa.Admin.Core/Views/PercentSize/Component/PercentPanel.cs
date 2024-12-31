using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Xioa.Admin.Core.Views.PercentSize.Component;

public class PercentPanel : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        double totalWidth = 0;
        double totalHeight = 0;
        double maxWidth = 0;
        double maxHeight = 0;  // 添加这个变量

        // 计算垂直布局时每个元素可用的高度
        double availableItemHeight = Orientation == Orientation.Vertical 
            ? (availableSize.Height - (Spacing * (Children.Count - 1))) / Children.Count 
            : availableSize.Height;

        foreach (UIElement child in Children)
        {
            if (child != null)
            {
                double width = availableSize.Width;
                double height = availableItemHeight;

                double percentWidth = GetPercentWidth(child);
                double percentHeight = GetPercentHeight(child);

                if (!double.IsNaN(percentWidth))
                    width = availableSize.Width * (percentWidth / 100.0);
                if (!double.IsNaN(percentHeight))
                    height = availableItemHeight * (percentHeight / 100.0);

                child.Measure(new Size(width, height));

                if (Orientation == Orientation.Horizontal)
                {
                    totalWidth += child.DesiredSize.Width;
                    maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
                }
                else
                {
                    totalHeight += child.DesiredSize.Height;
                    maxWidth = Math.Max(maxWidth, child.DesiredSize.Width);
                }
            }
        }

        if (Children.Count > 1)
        {
            if (Orientation == Orientation.Horizontal)
                totalWidth += Spacing * (Children.Count - 1);
            else
                totalHeight += Spacing * (Children.Count - 1);
        }

        return new Size(
            Orientation == Orientation.Horizontal ? totalWidth : maxWidth,
            Orientation == Orientation.Horizontal ? maxHeight : totalHeight);  // 使用maxHeight替代之前的错误引用
    }
    
    protected override Size ArrangeOverride(Size finalSize)
    {
        double offset = 0;
        double availableItemHeight = Orientation == Orientation.Vertical
            ? (finalSize.Height - (Spacing * (Children.Count - 1))) / Children.Count
            : finalSize.Height;

        foreach (UIElement child in Children)
        {
            if (child != null)
            {
                double width = child.DesiredSize.Width;
                double height = child.DesiredSize.Height;

                double percentWidth = GetPercentWidth(child);
                double percentHeight = GetPercentHeight(child);

                if (!double.IsNaN(percentWidth))
                    width = finalSize.Width * (percentWidth / 100.0);
                if (!double.IsNaN(percentHeight))
                    height = availableItemHeight * (percentHeight / 100.0); // 基于可用项高度计算

                if (Orientation == Orientation.Horizontal)
                {
                    child.Arrange(new Rect(offset, 0, width, height));
                    offset += width + Spacing;
                }
                else
                {
                    child.Arrange(new Rect(0, offset, width, height));
                    offset += height + Spacing;
                }
            }
        }
        return finalSize;
    }

    #region Orientation Property
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(
            "Orientation",
            typeof(Orientation),
            typeof(PercentPanel),
            new FrameworkPropertyMetadata(
                Orientation.Horizontal,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsMeasure));

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }
    #endregion

    #region Spacing Property
    public static readonly DependencyProperty SpacingProperty =
        DependencyProperty.Register(
            "Spacing",
            typeof(double),
            typeof(PercentPanel),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsMeasure));

    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }
    #endregion

    #region Attached Properties
    public static readonly DependencyProperty PercentWidthProperty =
        DependencyProperty.RegisterAttached(
            "PercentWidth",
            typeof(double),
            typeof(PercentPanel),
            new FrameworkPropertyMetadata(double.NaN,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsMeasure,
                OnPercentChanged));

    public static readonly DependencyProperty PercentHeightProperty =
        DependencyProperty.RegisterAttached(
            "PercentHeight",
            typeof(double),
            typeof(PercentPanel),
            new FrameworkPropertyMetadata(double.NaN,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsMeasure,
                OnPercentChanged));

    private static void OnPercentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element && VisualTreeHelper.GetParent(element) is PercentPanel panel)
        {
            panel.InvalidateMeasure();
            panel.InvalidateArrange();
        }
    }

    public static double GetPercentWidth(DependencyObject obj)
    {
        return (double)obj.GetValue(PercentWidthProperty);
    }

    public static void SetPercentWidth(DependencyObject obj, double value)
    {
        obj.SetValue(PercentWidthProperty, value);
    }

    public static double GetPercentHeight(DependencyObject obj)
    {
        return (double)obj.GetValue(PercentHeightProperty);
    }

    public static void SetPercentHeight(DependencyObject obj, double value)
    {
        obj.SetValue(PercentHeightProperty, value);
    }
    #endregion

   
    
}