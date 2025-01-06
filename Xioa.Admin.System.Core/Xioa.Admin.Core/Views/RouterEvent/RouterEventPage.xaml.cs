using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Xioa.Admin.Core.Views.RouterEvent
{
    public partial class RouterEventPage : Page
    {
        public RouterEventPage()
        {
            InitializeComponent();
            //隧道阶段（Tunneling）-Preview事件
            //事件从最外层向内传播：
            //1.OuterBorder 先收到 PreviewMouseDown
            //然后是 MiddlePanel 收到 PreviewMouseDown
            //最后是 EventButton 收到 PreviewMouseDown

            //冒泡阶段（Bubbling）-普通事件
            //事件从触发点向外传播：
            //EventButton 先触发 MouseDown
            //然后是 MiddlePanel 触发 MouseDown
            //最后是 OuterBorder 触发 MouseDown



            // 为三个层级添加事件处理
            EventButton.AddHandler(Button.MouseDownEvent, new MouseButtonEventHandler(Element_MouseDown), true);
            EventButton.AddHandler(Button.PreviewMouseDownEvent, new MouseButtonEventHandler(Element_PreviewMouseDown), true);

            MiddlePanel.AddHandler(StackPanel.MouseDownEvent, new MouseButtonEventHandler(Element_MouseDown), true);
            MiddlePanel.AddHandler(StackPanel.PreviewMouseDownEvent, new MouseButtonEventHandler(Element_PreviewMouseDown), true);

            OuterBorder.AddHandler(Border.MouseDownEvent, new MouseButtonEventHandler(Element_MouseDown), true);
            OuterBorder.AddHandler(Border.PreviewMouseDownEvent, new MouseButtonEventHandler(Element_PreviewMouseDown), true);
        }

        private void Element_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // 处理隧道事件
            string elementName = (sender as FrameworkElement)?.Name ?? "Unknown";
            LogEvent($"Preview MouseDown on {elementName} (隧道事件)");
        }

        private void Element_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // 处理冒泡事件
            string elementName = (sender as FrameworkElement)?.Name ?? "Unknown";
            LogEvent($"MouseDown on {elementName} (冒泡事件)");
        }

        private void LogEvent(string message)
        {
            // 在日志文本框中添加新的事件记录
            EventLog.Text += $"{DateTime.Now:HH:mm:ss.fff} - {message}\n";
            EventLog.ScrollToEnd();
        }


        private void ClearLog()
        {
            EventLog.Clear();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearLog();
        }
    }
}