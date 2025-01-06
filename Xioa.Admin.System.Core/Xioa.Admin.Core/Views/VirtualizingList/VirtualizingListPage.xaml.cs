using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Xioa.Admin.Core.Views.VirtualizingList
{
    public partial class VirtualizingListPage : Page
    {
        private DispatcherTimer _memoryTimer;

        public VirtualizingListPage()
        {
            InitializeComponent();
            
            // 创建一个定时器来更新内存使用情况
            _memoryTimer = new DispatcherTimer();
            _memoryTimer.Interval = TimeSpan.FromSeconds(1);
            _memoryTimer.Tick += Timer_Tick;
            _memoryTimer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateMemoryUsage();
        }

        private void UpdateMemoryUsage()
        {
            var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            var memoryUsageMB = currentProcess.WorkingSet64 / (1024 * 1024);
            MemoryUsageText.Text = $"{memoryUsageMB} MB";
        }

        private void AddMoreItems_Click(object sender, RoutedEventArgs e)
        {
            var vm = (ViewModel.VirtualizingListViewModel)DataContext;
            vm.AddMoreItems(100000); // 添加1万条数据
        }
    }
}