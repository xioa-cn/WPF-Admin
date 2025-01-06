using System;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Xioa.Admin.Core.Views.LiveChartsTemplate;
using HandyControl.Controls;

namespace Xioa.Admin.Core.Views.ErrorView;

public partial class ErrorViewModel : Xioa.Admin.Core.Services.ViewModels.ViewModelBase
{
    [ObservableProperty] private object? _content;

    public string[] Titles { get; set; } = new[]
    {
        "_403","_404","_500","InterError","NoData"
    };

    public ErrorViewModel()
    {
        ChangeContent(Titles[0]);
    }

    [RelayCommand]
    public void ChangeContent(string view)
    {
        var type = ChartsViewModel.GetTypeByStringName(view + "View");
        if (type is null)
        {
            Growl.Error("未找到页面");
            return;
        }

        var cont = Activator.CreateInstance(type);
        if (cont is UserControl control)
        {
            this.Content = control;
        }
    }
}