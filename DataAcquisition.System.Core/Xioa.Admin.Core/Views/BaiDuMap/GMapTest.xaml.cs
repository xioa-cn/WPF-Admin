using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

namespace Xioa.Admin.Core.Views.BaiDuMap;

public partial class GMapTest : Page
{
    public GMapTest()
    {
        // 配置 GMap.NET
        GMap.NET.GMaps.Instance.Mode = AccessMode.ServerAndCache;
        
        // 配置代理
        // System.Net.WebRequest.DefaultWebProxy = System.Net.WebProxy.GetDefaultProxy();
        // GMap.NET.MapProviders.GMapProvider.WebProxy = System.Net.WebProxy.GetDefaultProxy();
        //
        // 配置 SSL/TLS
        System.Net.ServicePointManager.SecurityProtocol = 
            System.Net.SecurityProtocolType.Tls12 | 
            System.Net.SecurityProtocolType.Tls11 | 
            System.Net.SecurityProtocolType.Tls;
        // 初始化 GMap.NET
        GMap.NET.GMaps.Instance.Mode = AccessMode.ServerAndCache;
    
        InitializeComponent();
        // 确保SQLite初始化
        //SQLitePCL.Batteries.Init();
        
        this.Loaded += GMapTest_Loaded;
        //InitializeMap();
    }
    private void GMapTest_Loaded(object sender, RoutedEventArgs e)
    {
        InitializeMap();
    }
    private void InitializeMap()
    {
        try
        {
            // 配置代理和超时
            GMapProvider.WebProxy = System.Net.WebProxy.GetDefaultProxy();
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            GMaps.Instance.UseUrlCache = true;
            
            // 设置超时时间
            GMapProvider.TimeoutMs = 10000; // 10秒超时
            
            // 配置缓存位置
            string cacheLocation = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "GMapNETCache"
            );
            MainMap.CacheLocation = cacheLocation;

            // 配置地图控件
            MainMap.Manager.Mode = AccessMode.ServerAndCache;
            MainMap.MapProvider = GMapProviders.OpenStreetMap;
            
            // 设置中国区域的坐标
            MainMap.Position = new PointLatLng(31.230416, 121.473701); // 上海
            MainMap.MinZoom = 2;
            MainMap.MaxZoom = 17;
            MainMap.Zoom = 10;
            
            // 其他设置
            MainMap.ShowCenter = false;
            MainMap.DragButton = MouseButton.Left;
            MainMap.ShowTileGridLines = false;
            
            // 使用主线程更新UI
            MainMap.CanDragMap = true;
            MainMap.ScaleMode = ScaleModes.Integer;
            
            // 选择默认地图源
            MapProviderComboBox.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine
            ($"地图初始化失败: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void MapProviderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MainMap == null) return;

        try
        {
            switch (MapProviderComboBox.SelectedIndex)
            {
                case 0: // OpenStreetMap
                    MainMap.MapProvider = GMapProviders.OpenStreetMap;
                    break;
                case 1: // 高德卫星
                    //MainMap.MapProvider = GMapProviders.AMapSatellite;
                    break;
                case 2: // 谷歌中国
                    MainMap.MapProvider = GMapProviders.GoogleChinaMap;
                    break;
                case 3: // OpenStreetMap
                    MainMap.MapProvider = GMapProviders.OpenStreetMap;
                    break;
            }

            // 刷新地图
            MainMap.ReloadMap();
        }
        catch (Exception ex)
        {
            Console.WriteLine
            ($"切换地图源失败: {ex.Message}");
        }
    }
    private void LocationButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // 定位到上海
            MainMap.Position = new PointLatLng(31.230416, 121.473701);
            MainMap.Zoom = 13;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"定位失败: {ex.Message}");
        }
    }
    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        SearchLocation();
    }

    private void SearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            SearchLocation();
        }
    }

    private void SearchLocation()
    {
        if (string.IsNullOrEmpty(SearchBox.Text)) return;

        try
        {
            // 这里需要实现具体的搜索逻辑
            MessageBox.Show("搜索功能需要配置具体的地图服务API");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"搜索失败: {ex.Message}");
        }
    }

    private void MainMap_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        try
        {
            Point point = e.GetPosition(MainMap);
            PointLatLng pointLatLng = MainMap.FromLocalToLatLng((int)point.X, (int)point.Y);

            // 添加标记
            var marker = new GMapMarker(pointLatLng)
            {
                Shape = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Red,
                    Stroke = Brushes.White,
                    StrokeThickness = 2
                }
            };

            MainMap.Markers.Add(marker);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"添加标记失败: {ex.Message}");
        }
    }
}