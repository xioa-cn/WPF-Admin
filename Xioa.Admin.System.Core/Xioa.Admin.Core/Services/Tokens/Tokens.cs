using CommunityToolkit.Mvvm.ComponentModel;

namespace Xioa.Admin.Core.Services.Tokens;

public partial class Tokens : ObservableObject
{
    public static Tokens Instance = new Tokens();
    [ObservableProperty]
    public string? _AccessToken;
    [ObservableProperty]
    public string? _RefreshToken;
}