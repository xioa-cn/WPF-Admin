using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace Xioa.Admin.Core.Views.DataValidator.ViewModel;

public partial class ValidatorViewModel : ObservableValidator
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "用户名不能为空")]
    [MinLength(3, ErrorMessage = "用户名最少需要3个字符")]
    [MaxLength(20, ErrorMessage = "用户名最多20个字符")]
    private string _username;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "邮箱不能为空")]
    [EmailAddress(ErrorMessage = "请输入有效的邮箱地址")]
    private string _email;

    [RelayCommand(CanExecute = nameof(CanRegister))]
    private void Register()
    {
        ValidateAllProperties();
        
        if (HasErrors)
        {
            MessageBox.Show("请修正所有错误后再提交", "验证错误");
            return;
        }

        MessageBox.Show($"注册成功！\n用户名: {Username}\n邮箱: {Email}", "注册成功");
        ClearForm();
    }

    private bool CanRegister()
    {
        return !HasErrors;
    }
    
    private void ClearForm()
    {
        Username = string.Empty;
        Email = string.Empty;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        RegisterCommand.NotifyCanExecuteChanged();
    }
}