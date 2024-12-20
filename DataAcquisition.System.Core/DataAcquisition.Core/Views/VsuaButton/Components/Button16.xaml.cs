using HandyControl.Controls;
using SharpVectors.Dom;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DataAcquisition.Core.Views.VsuaButton.Components
{
    public partial class Button16 : UserControl
    {

        public Button16()
        {
            InitializeComponent();
            DeleteButton.Loaded += DeleteButton_Loaded;
        }

        #region 动画
       
        private Storyboard _startStoryboard;
        private Storyboard _endStoryboard;
        private Grid _contentGrid;
        private Path _deleteIcon;
        private Path _trashItem;
        private TextBlock _buttonText;

      

        private void DeleteButton_Loaded(object sender, RoutedEventArgs e)
        {
            var template = DeleteButton.Template;
            _contentGrid = template.FindName("PART_ContentGrid", DeleteButton) as Grid;
            _deleteIcon = template.FindName("DeleteIcon", DeleteButton) as Path;
            _trashItem = template.FindName("TrashItem", DeleteButton) as Path;
            _buttonText = template.FindName("ButtonText", DeleteButton) as TextBlock;

            _startStoryboard = CreateStartStoryboard();
            _endStoryboard = CreateEndStoryboard();
        }

        private void StartDeleteAnimation()
        {
            _startStoryboard?.Begin();
        }

        private void EndDeleteAnimation()
        {
            _endStoryboard?.Begin();
        }

        public async Task InoAction(Func<Task> action)
        {
            StartDeleteAnimation();
            await action();
            EndDeleteAnimation();
        }


        private Storyboard CreateStartStoryboard()
        {
            var storyboard = new Storyboard();

            // 收缩动画
            var widthAnimation = new DoubleAnimation
            {
                To = 60,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            Storyboard.SetTarget(widthAnimation, _contentGrid);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(FrameworkElement.WidthProperty));
            storyboard.Children.Add(widthAnimation);

            // 文字淡出
            var textFadeOut = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(0.2)
            };
            Storyboard.SetTarget(textFadeOut, _buttonText);
            Storyboard.SetTargetProperty(textFadeOut, new PropertyPath(UIElement.OpacityProperty));
            storyboard.Children.Add(textFadeOut);

            // 垃圾出现和下落
            var trashFadeIn = new DoubleAnimation
            {
                To = 1,
                BeginTime = TimeSpan.FromSeconds(0.3),
                Duration = TimeSpan.FromSeconds(0.1)
            };
            Storyboard.SetTarget(trashFadeIn, _trashItem);
            Storyboard.SetTargetProperty(trashFadeIn, new PropertyPath(UIElement.OpacityProperty));
            storyboard.Children.Add(trashFadeIn);

            var trashFall = new DoubleAnimation
            {
                From = 0,
                To = 30,
                BeginTime = TimeSpan.FromSeconds(0.3),
                Duration = TimeSpan.FromSeconds(0.3)
            };
            Storyboard.SetTarget(trashFall, _trashItem);
            Storyboard.SetTargetProperty(trashFall, new PropertyPath("(Path.RenderTransform).(TransformGroup.Children)[2].(TranslateTransform.Y)"));
            storyboard.Children.Add(trashFall);

            var trashRotate = new DoubleAnimation
            {
                To = 45,
                BeginTime = TimeSpan.FromSeconds(0.3),
                Duration = TimeSpan.FromSeconds(0.3)
            };
            Storyboard.SetTarget(trashRotate, _trashItem);
            Storyboard.SetTargetProperty(trashRotate, new PropertyPath("(Path.RenderTransform).(TransformGroup.Children)[1].(RotateTransform.Angle)"));
            storyboard.Children.Add(trashRotate);

            return storyboard;
        }

        private Storyboard CreateEndStoryboard()
        {
            var storyboard = new Storyboard();

            // 垃圾桶晃动
            var iconShake = new DoubleAnimation
            {
                From = -5,
                To = 5,
                Duration = TimeSpan.FromSeconds(0.1),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(2)
            };
            Storyboard.SetTarget(iconShake, _deleteIcon);
            Storyboard.SetTargetProperty(iconShake, new PropertyPath("(Path.RenderTransform).(TransformGroup.Children)[1].(RotateTransform.Angle)"));
            storyboard.Children.Add(iconShake);

            // 垃圾消失
            var trashFadeOut = new DoubleAnimation
            {
                To = 0,
                BeginTime = TimeSpan.FromSeconds(0.3),
                Duration = TimeSpan.FromSeconds(0.1)
            };
            Storyboard.SetTarget(trashFadeOut, _trashItem);
            Storyboard.SetTargetProperty(trashFadeOut, new PropertyPath(UIElement.OpacityProperty));
            storyboard.Children.Add(trashFadeOut);

            // 恢复宽度
            var widthRestore = new DoubleAnimation
            {
                To = 200,
                BeginTime = TimeSpan.FromSeconds(0.6),
                Duration = TimeSpan.FromSeconds(0.3)
            };
            Storyboard.SetTarget(widthRestore, _contentGrid);
            Storyboard.SetTargetProperty(widthRestore, new PropertyPath(FrameworkElement.WidthProperty));
            storyboard.Children.Add(widthRestore);

            // 文字淡入
            var textFadeIn = new DoubleAnimation
            {
                To = 1,
                BeginTime = TimeSpan.FromSeconds(0.6),
                Duration = TimeSpan.FromSeconds(0.3)
            };
            Storyboard.SetTarget(textFadeIn, _buttonText);
            Storyboard.SetTargetProperty(textFadeIn, new PropertyPath(UIElement.OpacityProperty));
            storyboard.Children.Add(textFadeIn);

            return storyboard;
        }

        #endregion

        private async void Clear_Click(object sender, RoutedEventArgs e)
        {
            await InoAction(async () =>
             {
                 await Task.Delay(2000);
                 Growl.Info("清理成功！");
             });

        }
    }
}
