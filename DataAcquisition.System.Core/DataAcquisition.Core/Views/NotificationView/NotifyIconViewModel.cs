using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAcquisition.Core.Views.DialogView.Model;
using DataAcquisition.Core.Views.NotificationView.Model;
using HandyControl.Controls;
using HandyControl.Tools.Extension;

namespace DataAcquisition.Core.Views.NotificationView;

/// <summary>
/// @author Xioa
/// @date  2024年12月17日
/// </summary>
public partial class NotifyIconViewModel : ObservableObject, IDialogResultable<CloseEnum>
{
    public CloseEnum Result { get; set; }
    public Action CloseAction { get; set; }



    [ObservableProperty] private bool _close;
    [ObservableProperty] private bool _mini = true;

    [RelayCommand]
    private void Closed()
    {
        if (Close)
        {
            this.Result = CloseEnum.Close;
        }else if (Mini)
        {
            this.Result = CloseEnum.Notify;
        }
        Dialog.Close(MessageToken.DialogPageToken);
    }
    [RelayCommand]
    private void Cancel()
    {
        this.Result = CloseEnum.None;
        Dialog.Close(MessageToken.DialogPageToken);
    }
}