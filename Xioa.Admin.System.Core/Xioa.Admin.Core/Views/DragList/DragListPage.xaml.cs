using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xioa.Admin.Core.Views.DragList.ViewModel;

namespace Xioa.Admin.Core.Views.DragList;

public partial class DragListPage : Page
{
    private Point startPoint;

    public DragListPage()
    {
        InitializeComponent();
    }

    private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        startPoint = e.GetPosition(null);
    }

    private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                ListBox listBox = sender as ListBox;
                ListBoxItem listBoxItem = FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                if (listBoxItem != null && listBoxItem.DataContext is IDragDropItem dragItem)
                {
                    var dataObject = new DataObject();
                    dataObject.SetData("DragDropItem", dragItem);
                    DragDrop.DoDragDrop(listBoxItem, dataObject, DragDropEffects.Move);
                }
            }
        }
    }

    private void ListBox_Drop(object sender, DragEventArgs e)
    {
        if (sender is ListBox listBox)
        {
            var source = e.Data.GetData("DragDropItem") as IDragDropItem;
            var target = (e.OriginalSource as FrameworkElement)?.DataContext as IDragDropItem;

            if (source != null && target != null)
            {
                int sourceIndex = listBox.Items.IndexOf(source);
                int targetIndex = listBox.Items.IndexOf(target);

                if (sourceIndex != -1 && targetIndex != -1)
                {
                    var vm = DataContext as DragListViewModel;
                    vm?.MoveItem(sourceIndex, targetIndex);
                }
            }
        }
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        base.OnPreviewKeyDown(e);
        
        if (e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control)
        {
            var vm = DataContext as DragListViewModel;
            vm?.UndoCommand.Execute(null);
            e.Handled = true;
        }
    }

    private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
    {
        do
        {
            if (current is T)
            {
                return (T)current;
            }
            current = VisualTreeHelper.GetParent(current);
        }
        while (current != null);
        return null;
    }
}