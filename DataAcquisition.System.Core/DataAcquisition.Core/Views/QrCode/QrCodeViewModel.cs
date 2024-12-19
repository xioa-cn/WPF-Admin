using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAcquisition.Core.Views.QrCode.Utils;
using HandyControl.Controls;
using ICSharpCode.AvalonEdit;
using QRCoder;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace DataAcquisition.Core.Views.QrCode;

/// <summary>
/// @author Xioa
/// @date  2024年12月3日
/// </summary>
public partial class QrCodeViewModel : ObservableObject
{
    [ObservableProperty] private object? _source;
    public QrCodeViewModel()
    {
        Source = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets", "Svg", "title.svg");

        //var temp = QrCodeHelper.CreateQRCode("xioa", 1, "C:\\Users\\Administrator\\Desktop\\header.jpg");
    }
    [ObservableProperty] private string content;
    private string iconFile;
    [RelayCommand]
    private void CreateIcon()
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Multiselect = false;
        dialog.Title = "请选择文件夹";
        dialog.Filter = "(*.jpg,*.png,*.jpeg,*.bmp)|*.jgp;*.png;*.jpeg|All files(*.*)|*.*";
        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            iconFile = dialog.FileName;
        }
        else
        {
            return;
        }
    }

    [RelayCommand]
    private void CreateQrCode()
    {
        if (string.IsNullOrWhiteSpace(Content))
        {
            Growl.Error("请输入二维码内容");
            return;
        }

        var res = QrCodeHelper.CreateQRCode(Content, 10, iconFile);
        Source = res;
    }


    [RelayCommand]
    private async Task ReaderCode(Page page)
    {
        var window = System.Windows.Window.GetWindow(page);
        window.Hide();
        var bytes = DataAcquisition.Core.Views.QrCode.Utils.QrCodeHelper.CaptureScreen(window);
        await ScanScreenResult(bytes);
        window?.Show();
        if (window?.WindowState == WindowState.Minimized)
        {
            window.WindowState = WindowState.Normal;
        }
        window?.Activate();
        window?.Focus();
    }


    public async Task ScanScreenResult(byte[]? bytes)
    {
        var result = DataAcquisition.Core.Views.QrCode.Utils.QrCodeHelper.ParseBarcode(bytes);
        await AddScanResultAsync(result);
    }

    private async Task AddScanResultAsync(string? result)
    {
        if (DataAcquisition.Core.Views.QrCode.Utils.QrCodeHelper.IsNullOrEmpty(result))
        {
            Growl.Success("未识别到二维码");
        }
        else
        {
            Growl.Success("识别内容：" + result);
        }
    }

}