using System;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace Xioa.Admin.Core;

/// <summary>
/// @author Xioa
/// @date  2024年12月17日
/// </summary>
public partial class App : Application {
    private static NotifyIcon? _notifyIcon;

    public  void InitializeNotifyIcon() {
        _notifyIcon = new NotifyIcon {
            Icon = new System.Drawing.Icon("Assets/png/logo_32x32.ico"),
            Visible = true,
            Text = "Xioa.Admin"
        };

        _notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

        _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
        _notifyIcon.ContextMenuStrip.Items.Add("打开", null, Open_Click);
        _notifyIcon.ContextMenuStrip.Items.Add("退出", null, Exit_Click);
    }

    private void NotifyIcon_DoubleClick(object? sender, EventArgs e) {
        MainWindowShow?.Show();
        MainWindowShow?.Activate();
    }

    private void Open_Click(object? sender, EventArgs e) {
        MainWindowShow?.Show();
        MainWindowShow?.Activate();
    }

    private void Exit_Click(object? sender, EventArgs e) {
        _notifyIcon?.Dispose();
        Application.Current.Shutdown();
        Environment.Exit(0);
    }

    public static void DisposeNotifyIcon() {
        _notifyIcon?.Dispose();
    }

    protected override void OnExit(ExitEventArgs e) {
        _notifyIcon?.Dispose(); // 清理托盘图标
        base.OnExit(e);
    }
}