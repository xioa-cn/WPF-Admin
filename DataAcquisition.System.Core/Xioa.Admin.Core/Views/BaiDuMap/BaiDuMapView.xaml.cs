using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;

namespace Xioa.Admin.Core.Views.BaiDuMap;

public partial class BaiDuMapView : Page
{
    public BaiDuMapView()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
        webView.EnsureCoreWebView2Async();
    }
   
    
    private void WebView_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
    {
        if (webView is { CoreWebView2: not null })
        {
            webView.CoreWebView2.AddHostObjectToScript("BaiDuMapViewModel", this.DataContext as BaiDuMapViewModel);
            //加载页面
            // string rootPath = AppDomain.CurrentDomain.BaseDirectory; 
            // string filepath = System.IO.Path.Combine(rootPath, "html","HTMLPage5.html");
            webView.Source = new Uri("https://map.baidu.com/");
        }
       
    }
}