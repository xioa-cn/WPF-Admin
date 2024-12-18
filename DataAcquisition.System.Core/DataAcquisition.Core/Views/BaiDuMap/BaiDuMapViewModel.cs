using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Web.WebView2.Wpf;

namespace DataAcquisition.Core.Views.BaiDuMap;

/// <summary>
/// @author Xioa
/// @date  2024年12月4日
/// </summary>
public partial class BaiDuMapViewModel : ObservableObject
{
    [ObservableProperty] private string? _value;


    [RelayCommand]
    private async Task Chrome(WebView2? webView)
    {
        if (webView != null && webView.CoreWebView2 != null)
        {
            await webView.CoreWebView2.ExecuteScriptAsync($"LocateByName('{this.Value}')");
        }
       
    }
}