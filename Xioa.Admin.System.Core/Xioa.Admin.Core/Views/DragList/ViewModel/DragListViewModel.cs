using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Xioa.Admin.Core.Views.DragList.ViewModel;


public partial class DragListViewModel : ObservableObject 
{
    // 字符串类型实现 IDragDropItem
    private class StringDragItem : IDragDropItem
    {
        private readonly string _value;
        public StringDragItem(string value) => _value = value;
        public string DisplayText => _value;
    }

    public ObservableCollection<IDragDropItem> Items { get; } = new();

    public DragListViewModel()
    {
        foreach (var item in new[] { "ITEM1", "ITEM2", "ITEM3", "ITEM4", "ITEM5" })
        {
            Items.Add(new StringDragItem(item));
        }
    }

    [RelayCommand]
    private void TestLook() {
        Console.WriteLine(Items);
    }
}