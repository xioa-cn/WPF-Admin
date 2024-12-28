using System;
using System.Threading.Tasks;
using System.Windows;
using Xioa.Admin.Core.Views.LoginView;
using Xioa.Admin.Core.Views.MainView;
using Xioa.Admin.Core.Views.MainView.Model;
using Xioa.Admin.Core.WindowManager;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using Xioa.Admin.Core.Services.Logging;
using Xioa.Admin.Core.Services.Tokens;

namespace Xioa.Admin.Core
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ILogService Logger => LogService.Instance;

        public static Window MainWindowShow { get; set; } = new MainWindow();

        // 无权限控制  IsEnabled 或 Visibility
        public static ViewAuthSwitch ViewAuthSwitch { get; } = ViewAuthSwitch.IsEnabled;


        protected override async void OnStartup(StartupEventArgs e)
        {
            ApplicationAxiosConfig.Initialized();

            if (e.Args.Length > 0)
            {
                // 程序接受外部参数
            }

            Logger.LogInfo("打开了软件");
            ThemeManager.Instance.IsDarkTheme = false;
            Logger.LogInfo("主题加载");
            LiveCharts.Configure(config =>
                config.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('汉')));
            WindowManager.SplashScreen splashScreen = new WindowManager.SplashScreen();

            splashScreen.ShowWindowWithFade();

            await Task.Delay(5);


            #region 检测

            string? mName = System.Diagnostics.Process.GetCurrentProcess().MainModule?.ModuleName;
            string? pName = System.IO.Path.GetFileNameWithoutExtension(mName);

            if (System.Diagnostics.Process.GetProcessesByName(pName).Length > 1)
            {
                MessageBox.Show("本程序一次只能运行一个实例！", "提示");
                Application.Current.Shutdown();
                Environment.Exit(0);
                return;
            }

            #endregion


            splashScreen.SwitchWindow(new Login1Window());


            // 设置全局异常处理
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                Logger.LogFatal("Unhandled Exception", args.ExceptionObject as Exception);
            };

            Current.DispatcherUnhandledException += (s, args) =>
            {
                Logger.LogError("Dispatcher Unhandled Exception", args.Exception);
                //args.Handled = true;
            };
            MainWindowShow.Loaded += (s, args) =>
            {
                InitializeNotifyIcon();
            };
            base.OnStartup(e);
        }
    }
}