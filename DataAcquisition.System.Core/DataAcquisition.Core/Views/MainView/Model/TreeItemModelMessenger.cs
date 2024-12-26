namespace DataAcquisition.Core.Views.MainView.Model;

/// <summary>
/// @author Xioa
/// @date  2024-12-26 10:22:05
/// </summary>
public enum MessengerStatus {
    None,
    FromNavBarToPage,
    FromWindowToPage,
}

public class TreeItemModelMessenger {
    public MessengerStatus MessengerStatus { get; set; }
    public TreeItemModel Item { get; set; }
}