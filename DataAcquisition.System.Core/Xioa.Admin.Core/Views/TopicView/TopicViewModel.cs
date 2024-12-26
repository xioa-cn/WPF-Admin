using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Xioa.Admin.Core.Views.MainView.Model;

namespace Xioa.Admin.Core.Views.TopicView;

public partial class TopicViewModel : ObservableObject
{
    public string[] Themes { get; set; } =
    {
        "Green", "Purple", "Red", "Yellow", "Pink", "Orange", "Cyan", "Blue",
        "DarkGreen", "DarkPurple", "DarkRed", "DarkYellow", "DarkPink", "DarkOrange", "DarkCyan", "DarkBlue",
    };


    [RelayCommand]
    public void Use(string? content)
    {
        if (string.IsNullOrEmpty(content)) return;
        ThemeManager.UseTheme(content);
    }
}