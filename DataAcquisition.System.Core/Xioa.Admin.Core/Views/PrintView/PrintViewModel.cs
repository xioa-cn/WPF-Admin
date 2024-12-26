using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Xioa.Admin.Core.Views.PrintView.Services;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Windows;

namespace Xioa.Admin.Core.Views.PrintView;

///<summary>
/// @author：XIOA (xioa.liu)
/// @date：2024-35-08 03:35:00
/// @belong-sln：Xioa.Admin.System.Core 
/// @desc：PrintViewModel
///</summary>
public partial class PrintViewModel : ObservableObject
{
    private readonly IPrintService _printService;

    [ObservableProperty]
    private List<string> printers;

    [ObservableProperty]
    private string selectedPrinter;

    [ObservableProperty]
    private List<string> pageSizes;

    [ObservableProperty]
    private string selectedPageSize;

    [ObservableProperty]
    private List<string> printOrientations;

    [ObservableProperty]
    private string selectedOrientation;

    [ObservableProperty]
    private string printContent;

    [ObservableProperty]
    private string statusMessage;

    public PrintViewModel()
    {
        _printService = new StandardPrintService();
        InitializeProperties();
        LoadPrinters();
    }

    private void InitializeProperties()
    {
        // 初始化页面大小列表
        PageSizes = new List<string>
        {
            "A4",
            "A5",
            "Letter",
            "Legal"
        };
        SelectedPageSize = "A4";

        // 初始化打印方向
        PrintOrientations = new List<string>
        {
            "纵向",
            "横向"
        };
        SelectedOrientation = "纵向";

        // 初始化状态消息
        StatusMessage = "就绪";
        
        // 初始化打印内容
        PrintContent = "";
    }

    private void LoadPrinters()
    {
        try
        {
            Printers = _printService.GetPrinters();
            if (Printers.Any())
            {
                SelectedPrinter = Printers.First();
                StatusMessage = "已加载打印机列表";
            }
            else
            {
                StatusMessage = "未找到可用打印机";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载打印机失败: {ex.Message}";
        }
    }

    [RelayCommand]
    private void Preview()
    {
        try
        {
            if (string.IsNullOrEmpty(PrintContent))
            {
                MessageBox.Show("请输入要打印的内容", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _printService.PreviewContent(PrintContent);
            StatusMessage = "正在预览打印内容";
        }
        catch (Exception ex)
        {
            StatusMessage = $"预览失败: {ex.Message}";
            MessageBox.Show($"预览失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void Print()
    {
        try
        {
            if (string.IsNullOrEmpty(SelectedPrinter))
            {
                MessageBox.Show("请选择打印机", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrEmpty(PrintContent))
            {
                MessageBox.Show("请输入要打印的内容", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _printService.Print(SelectedPrinter, PrintContent);
            StatusMessage = "正在打印...";
        }
        catch (Exception ex)
        {
            StatusMessage = $"打印失败: {ex.Message}";
            MessageBox.Show($"打印失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}