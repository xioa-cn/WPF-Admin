using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Xioa.Admin.Core.Views.DragList.ViewModel;

public partial class DragListViewModel : ObservableObject 
{
    private readonly Stack<(int FromIndex, int ToIndex)> _undoStack = new();
    
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

    public void MoveItem(int fromIndex, int toIndex)
    {
        _undoStack.Push((fromIndex, toIndex));
        Items.Move(fromIndex, toIndex);
    }

    [RelayCommand]
    private void Undo()
    {
        if (_undoStack.Count > 0)
        {
            var (fromIndex, toIndex) = _undoStack.Pop();
            // 反向移动以撤销操作
            Items.Move(toIndex, fromIndex);
        }
    }

    [RelayCommand]
    private void TestLook() {
        Console.WriteLine(Items);
    }
}