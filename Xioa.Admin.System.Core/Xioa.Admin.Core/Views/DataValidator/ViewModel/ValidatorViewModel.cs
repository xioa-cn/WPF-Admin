using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using Xioa.Admin.Core.Views.DataValidator.Validations;
using HandyControl.Controls;

namespace Xioa.Admin.Core.Views.DataValidator.ViewModel;

public partial class ValidatorViewModel : ObservableValidator
{
    [ObservableProperty] private float _age;

    [FloatRule(0, 100, ErrorMessage = "数值必须在0到100之间")]
    [ObservableProperty]
    [Required(ErrorMessage = "不能为空值")]
    [NotifyDataErrorInfo]
    private string _height = "0";

    [RelayCommand]
    private void Submit()
    {
        Growl.Success($"UI-Submit!{Age}");
    }
    [RelayCommand(CanExecute = nameof(CanSubmit))]
    private void VmSubmit()
    {
        Growl.Success($"VM-Submit!{Height}");
    }

    private bool CanSubmit()
    {
        // 检查所有属性是否有效
        ValidateAllProperties();
        return !HasErrors;
    }

    // 当属性变化时重新检查命令是否可用
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        SubmitCommand.NotifyCanExecuteChanged();
    }
}