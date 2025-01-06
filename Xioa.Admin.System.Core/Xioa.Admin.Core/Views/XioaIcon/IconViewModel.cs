using CommunityToolkit.Mvvm.ComponentModel;

namespace Xioa.Admin.Core.Views.XioaIcon;

/// <summary>
/// @author Xioa
/// @date  2024年12月16日
/// </summary>
public partial class IconViewModel : Xioa.Admin.Core.Services.ViewModels.ViewModelBase
{
    private string? name;

    public string? Name
    {
        get => name;
        set => SetProperty(ref name, value);
    }
}