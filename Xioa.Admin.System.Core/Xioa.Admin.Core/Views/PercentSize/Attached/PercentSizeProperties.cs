using System.Globalization;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;

namespace Xioa.Admin.Core.Views.PercentSize.Attached;

public class PercentSizeProperties
{
    #region PercentWidth

    public static readonly DependencyProperty PercentWidthProperty =
        DependencyProperty.RegisterAttached(
            "PercentWidth",
            typeof(double),
            typeof(PercentSizeProperties),
            new PropertyMetadata(double.NaN, OnPercentWidthChanged));

    public static double GetPercentWidth(DependencyObject obj)
    {
        return (double)obj.GetValue(PercentWidthProperty);
    }

    public static void SetPercentWidth(DependencyObject obj, double value)
    {
        obj.SetValue(PercentWidthProperty, value);
    }

    private static void OnPercentWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element)
        {
            UpdateElementParentListener(element);
            UpdateElementWidth(element);
        }
    }

    #endregion

    #region PercentHeight

    public static readonly DependencyProperty PercentHeightProperty =
        DependencyProperty.RegisterAttached(
            "PercentHeight",
            typeof(double),
            typeof(PercentSizeProperties),
            new PropertyMetadata(double.NaN, OnPercentHeightChanged));

    public static double GetPercentHeight(DependencyObject obj)
    {
        return (double)obj.GetValue(PercentHeightProperty);
    }

    public static void SetPercentHeight(DependencyObject obj, double value)
    {
        obj.SetValue(PercentHeightProperty, value);
    }

    private static void OnPercentHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element)
        {
            UpdateElementParentListener(element);
            UpdateElementHeight(element);
        }
    }

    #endregion

    private static readonly ConcomitantDictionary<FrameworkElement, FrameworkElement> ParentCache
        = new ConcomitantDictionary<FrameworkElement, FrameworkElement>();

    private static void UpdateElementParentListener(FrameworkElement element)
    {
        element.Loaded += Element_Loaded;

        if (ParentCache.TryGetValue(element, out var oldParent))
        {
            oldParent.SizeChanged -= Parent_SizeChanged;
        }

        void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateElementSize(element);
        }

        element.Unloaded += (s, e) =>
        {
            if (ParentCache.TryGetValue(element, out var parent))
            {
                parent.SizeChanged -= Parent_SizeChanged;
                ParentCache.Remove(element);
            }
            element.Loaded -= Element_Loaded;
        };

        void Element_Loaded(object sender, RoutedEventArgs e)
        {
            var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
            if (parent != null)
            {
                parent.SizeChanged += Parent_SizeChanged;
                ParentCache[element] = parent;
            }
        }
    }

    private static void UpdateElementSize(FrameworkElement element)
    {
        UpdateElementWidth(element);
        UpdateElementHeight(element);
    }

    private static void UpdateElementWidth(FrameworkElement element)
    {
        var percentWidth = GetPercentWidth(element);
        if (!double.IsNaN(percentWidth))
        {
            var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
            if (parent != null)
            {
                element.Width = parent.ActualWidth * (percentWidth / 100.0);
            }
        }
    }

    private static void UpdateElementHeight(FrameworkElement element)
    {
        var percentHeight = GetPercentHeight(element);
        if (!double.IsNaN(percentHeight))
        {
            var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
            if (parent != null)
            {
                element.Height = parent.ActualHeight * (percentHeight / 100.0);
            }
        }
    }
}

// 辅助类，用于缓存父元素引用
public class ConcomitantDictionary<TKey, TValue> where TKey : class
{
    private readonly Dictionary<WeakReference<TKey>, TValue> _dictionary =
        new Dictionary<WeakReference<TKey>, TValue>(new WeakReferenceComparer<TKey>());

    public void Add(TKey key, TValue value)
    {
        Clean();
        _dictionary[new WeakReference<TKey>(key)] = value;
    }

    public bool Remove(TKey key)
    {
        Clean();
        var keyToRemove = _dictionary.Keys.FirstOrDefault(wr => wr.TryGetTarget(out var target) && target == key);
        return keyToRemove != null && _dictionary.Remove(keyToRemove);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        Clean();
        var keyFound = _dictionary.Keys.FirstOrDefault(wr => wr.TryGetTarget(out var target) && target == key);
        if (keyFound != null)
        {
            value = _dictionary[keyFound];
            return true;
        }
        value = default;
        return false;
    }

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out var value))
                return value;
            throw new KeyNotFoundException();
        }
        set => Add(key, value);
    }

    private void Clean()
    {
        var keysToRemove = _dictionary.Keys.Where(wr => !wr.TryGetTarget(out _)).ToList();
        foreach (var key in keysToRemove)
        {
            _dictionary.Remove(key);
        }
    }
}

public class WeakReferenceComparer<T> : IEqualityComparer<WeakReference<T>> where T : class
{
    public bool Equals(WeakReference<T> x, WeakReference<T> y)
    {
        if (x.TryGetTarget(out var targetX) && y.TryGetTarget(out var targetY))
        {
            return targetX.Equals(targetY);
        }
        return false;
    }

    public int GetHashCode(WeakReference<T> obj)
    {
        if (obj.TryGetTarget(out var target))
        {
            return target.GetHashCode();
        }
        return 0;
    }
}