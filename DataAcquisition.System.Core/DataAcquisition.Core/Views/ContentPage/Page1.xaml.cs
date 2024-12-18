using DataAcquisition.Core.Views.LoginView;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DataAcquisition.Core.Views.ContentPage
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();
        }

        private void Login1_Click(object sender, RoutedEventArgs e)
        {
            Window login = new LoginWindow()
            {
                DataContext = new LoginViewModel()
            };
            login.Show();
        }

        private void BorderMouseEnter(object sender, MouseEventArgs e)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation yd1 = new DoubleAnimation();
            pa.RenderTransform = new RotateTransform();
            pa.RenderTransformOrigin = new Point(125, 125);
            yd1.From = 45; //动画的起始值
            yd1.To = 0; //动画的结束值
            yd1.Duration = TimeSpan.FromSeconds(0.5); //动画的播放时间            
            Storyboard.SetTarget(yd1, pa); //给故事板绑定动画
            Storyboard.SetTargetProperty(yd1, new PropertyPath("RenderTransform.Angle")); //动画的依赖属性
            sb.Children.Add(yd1); //故事板添加动画
            sb.Begin(); //播放动画
        }

        private void Login2_Click(object sender, RoutedEventArgs e)
        {
            Window login = new Login1Window()
            {
                DataContext = new LoginViewModel()
            };
            login.Show();
        }
    }
}