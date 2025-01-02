using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Xioa.Admin.Core.Views.Video.ViewModel;

//http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4
//http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4
// 添加一个简单的输入对话框类
public class InputDialog : Window {
    private TextBox textBox;
    public string ResponseText { get; private set; }

    public InputDialog(string question) {
        Width = 300;
        Height = 150;
        Title = "输入URL";
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        // 文本框
        textBox = new TextBox {
            Margin = new Thickness(10),
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetRow(textBox, 0);
        grid.Children.Add(textBox);

        // 按钮面板
        var buttonPanel = new StackPanel {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        Grid.SetRow(buttonPanel, 1);

        var okButton = new Button {
            Content = "确定",
            Width = 60,
            Margin = new Thickness(5)
        };
        okButton.Click += (s, e) => { DialogResult = true; };

        var cancelButton = new Button {
            Content = "取消",
            Width = 60,
            Margin = new Thickness(5)
        };
        cancelButton.Click += (s, e) => { DialogResult = false; };

        buttonPanel.Children.Add(okButton);
        buttonPanel.Children.Add(cancelButton);
        grid.Children.Add(buttonPanel);

        Content = grid;
    }

    protected override void OnSourceInitialized(EventArgs e) {
        base.OnSourceInitialized(e);
        textBox.Focus();
    }

    protected override void OnClosing(CancelEventArgs e) {
        ResponseText = textBox.Text;
        base.OnClosing(e);
    }
}