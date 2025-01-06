using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading.Tasks;
using System;
using System.Windows.Threading;

namespace Xioa.Admin.Core.Views.BindingPage
{
    public partial class BindingPageViewModel : ObservableObject
    {
        [ObservableProperty] private string userName;
        [ObservableProperty] private string firstName;
        [ObservableProperty] private string lastName;
        [ObservableProperty] private decimal price;
        [ObservableProperty] private int quantity;
        [ObservableProperty] private decimal discountRate = 0.9M;
        [ObservableProperty] private bool isAdvancedEnabled;
        [ObservableProperty] private bool hasPermission = true;
        [ObservableProperty] private int featureCount = 5;
        [ObservableProperty]
        private string? realTimeData =null;
        [ObservableProperty]
        private string? cacheData = null;
        [ObservableProperty]
        private string defaultData;

        public BindingPageViewModel()
        {
            DefaultData = "这是默认数据";
        }

        [RelayCommand]
        private async Task LoadRealTimeData()
        {
            try
            {
                RealTimeData = "RealTimeData";              
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载数据时出错: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task LoadCacheData()
        {
            try
            {
                CacheData = "CacheData";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载数据时出错: {ex.Message}");
            }
        }

        [RelayCommand]
        private void ClearData()
        {
            RealTimeData = null;
            CacheData = null;
        }
    }

}