using HandyControl.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Xioa.Admin.Core.Views.XioaIcon;

public partial class IconPage : Page
{
    public IconPage()
    {
        InitializeComponent();
        LoadAllIcons();
    }

    private void LoadAllIcons()
    {
        var resourceDictionary = (ResourceDictionary)Application.LoadComponent(
            new Uri("/Xioa.Admin.Core;component/Views/XioaIcon/Themes/Icon.xaml", UriKind.Relative));

        var wrapPanel = new WrapPanel();
        wrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
        wrapPanel.VerticalAlignment = VerticalAlignment.Center;
        var iconStyle = (Style)Resources["IconStyle"];

        foreach (var key in resourceDictionary.Keys)
        {
            var panel = new StackPanel();
            panel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.VerticalAlignment = VerticalAlignment.Center;
            panel.Children.Add(resourceDictionary[key] as Path);
            panel.Children.Add(new TextBlock
                { Margin = new Thickness(0, 10, 0, 10), FontSize = 8, Text = key.ToString() });
            var contentControl = new Button
            {
                Name = key.ToString(),
                Content = panel,
                Margin = new Thickness(10),
                Style = (Style)Resources["Path_Button"]
            };
            //contentControl.Background = Brushes.Transparent;
            contentControl.ToolTip = new ToolTip()
            {
                Content = new TextBlock() { Text = $"StaticResource:{key}" }
            };

            contentControl.Click += Copy_Path;
            wrapPanel.Children.Add(contentControl);
        }

        var scrollViewer = new System.Windows.Controls.ScrollViewer { Content = wrapPanel };
        this.Content = scrollViewer;
    }

    private void Copy_Path(object sender, RoutedEventArgs e)
    {
        var text = (sender as Button).Name;
        var contentHeader = "<ContentControl Content=\"{StaticResource ";
        var contentEnd = "}\"/>";
        Clipboard.SetText($" <!-- 需要接入资源字典XioaIcon/Themes/Icon.xaml --> \n {contentHeader}{text}{contentEnd}");
        Growl.Info("复制内容到剪贴板！");
    }
}