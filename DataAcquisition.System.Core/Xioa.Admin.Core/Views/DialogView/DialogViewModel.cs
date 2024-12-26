using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Xioa.Admin.Core.Views.DialogView.Components;
using Xioa.Admin.Core.Views.DialogView.Model;
using Xioa.Admin.Core.Views.DialogView.ViewModel;
using HandyControl.Controls;
using HandyControl.Tools.Extension;

namespace Xioa.Admin.Core.Views.DialogView;

/// <summary>
/// @author Xioa
/// @date  2024年12月17日
/// </summary>
public partial class DialogViewModel : ObservableObject
{
    public string Message { get; set; } = "DialogViewModel";

    [RelayCommand]
    private async Task ShowInteractiveDialogCmd()
    {
        try
        {
            Dialog? dialog = null;

            dialog = Dialog.Show<ProgressDialog>();
            await dialog.Initialize<ProgressDialogViewModel>(vm =>
                {
                    vm.Message = Message;
                    vm.Dialog = dialog;
                })
                .GetResultAsync<string>().ContinueWith(str => Message = str.Result);
            Growl.Success($"提交内容：{Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error showing dialog: {ex.Message}");
        }
    }
}