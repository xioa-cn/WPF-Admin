using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace Xioa.Admin.Core.Views.VsuaButton.Components;

public partial class Button17 : UserControl
{
    #region 配置参数

    private static class ParticleConfig
    {
        public static int Count = 20; // 粒子数量
        public static double MinSize = 6; // 最小尺寸
        public static double MaxSize = 8; // 最大尺寸
        public static Color Color = Color.FromRgb(255, 0, 129); // 粒子颜色
        public static double InitialX = 100; // 初始X位置
        public static double InitialY = 30; // 初始Y位置
        public static double MinDistance = 100; // 最小扩散距离
        public static double MaxDistance = 200; // 最大扩散距离
        public static double MoveDuration = 1.5; // 移动动画时长
        public static double FadeBeginTime = 1.0; // 淡出开始时间
        public static double FadeDuration = 0.5; // 淡出持续时间
    }

    #endregion

    private readonly Random _random = new Random();
    private Canvas _topCanvas;
    private Canvas _bottomCanvas;
    private bool _isAnimating;

    public Button17()
    {
        InitializeComponent();
        Loaded += OnLoaded;

        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var template = AnimatedButton.Template;
        _topCanvas = template.FindName("TopParticles", AnimatedButton) as Canvas;
        _bottomCanvas = template.FindName("BottomParticles", AnimatedButton) as Canvas;
        if (_topCanvas != null && _bottomCanvas != null)
        {
            AnimatedButton.Click += OnButtonClick;
        }
    }

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        if (!_isAnimating)
        {
            CreateAndAnimateParticles(_topCanvas, true);
            CreateAndAnimateParticles(_bottomCanvas, false);
        }
    }

    private void CreateAndAnimateParticles(Canvas canvas, bool isTop)
    {
        if (canvas == null) return;
        _isAnimating = true;
        canvas.Children.Clear();
        for (int i = 0; i < ParticleConfig.Count; i++)
        {
            var particle = CreateParticle();
            canvas.Children.Add(particle);
            var storyboard = CreateParticleAnimation(particle, isTop);
            // 只在最后一组粒子的动画完成时重置状态
            if (i == ParticleConfig.Count - 1 && isTop)
            {
                storyboard.Completed += (s, e) => _isAnimating = false;
            }

            storyboard.Begin();
        }
    }

    private Ellipse CreateParticle()
    {
        double size = ParticleConfig.MinSize +
                      (_random.NextDouble() * (ParticleConfig.MaxSize - ParticleConfig.MinSize));

        var particle = new Ellipse
        {
            Width = size,
            Height = size,
            Fill = new SolidColorBrush(ParticleConfig.Color)
        };
        Canvas.SetLeft(particle, ParticleConfig.InitialX - (size / 2));
        Canvas.SetTop(particle, ParticleConfig.InitialY - (size / 2));
        var transformGroup = new TransformGroup();
        transformGroup.Children.Add(new ScaleTransform());
        transformGroup.Children.Add(new TranslateTransform());
        particle.RenderTransform = transformGroup;
        return particle;
    }

    private Storyboard CreateParticleAnimation(Ellipse particle, bool isTop)
    {
        var storyboard = new Storyboard();
        // 计算随机移动方向和距离
        double angle = _random.NextDouble() * 360;
        double distance = _random.Next((int)ParticleConfig.MinDistance, (int)ParticleConfig.MaxDistance);

        double moveToX = Math.Cos(angle * Math.PI / 180) * distance;
        double moveToY = Math.Sin(angle * Math.PI / 180) * distance * (isTop ? -1 : 1);
        // 添加X方向移动动画
        storyboard.Children.Add(CreateAnimation(
            particle,
            "(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)",
            0, moveToX,
            ParticleConfig.MoveDuration));
        // 添加Y方向移动动画
        storyboard.Children.Add(CreateAnimation(
            particle,
            "(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.Y)",
            0, moveToY,
            ParticleConfig.MoveDuration));
        // 添加缩放动画
        var scaleX = CreateAnimation(
            particle,
            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)",
            1, 0,
            ParticleConfig.FadeDuration,
            ParticleConfig.FadeBeginTime);
        storyboard.Children.Add(scaleX);
        var scaleY = scaleX.Clone();
        Storyboard.SetTargetProperty(scaleY,
            new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
        storyboard.Children.Add(scaleY);
        return storyboard;
    }

    private DoubleAnimation CreateAnimation(
        DependencyObject target,
        string propertyPath,
        double from,
        double to,
        double duration,
        double? beginTime = null)
    {
        var animation = new DoubleAnimation
        {
            From = from,
            To = to,
            Duration = TimeSpan.FromSeconds(duration),
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
        };
        if (beginTime.HasValue)
        {
            animation.BeginTime = TimeSpan.FromSeconds(beginTime.Value);
        }

        Storyboard.SetTarget(animation, target);
        Storyboard.SetTargetProperty(animation, new PropertyPath(propertyPath));
        return animation;
    }


    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        CleanUp(); // 控件卸载时清理资源
    }

    private void CleanUp()
    {
        if (AnimatedButton != null)
        {
            AnimatedButton.Click -= OnButtonClick;
        }

        // 停止所有正在进行的动画
        if (_topCanvas != null)
        {
            _topCanvas.Children.Clear();
        }

        if (_bottomCanvas != null)
        {
            _bottomCanvas.Children.Clear();
        }

        // 移除事件处理器
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;
        _isAnimating = false;
    }
}