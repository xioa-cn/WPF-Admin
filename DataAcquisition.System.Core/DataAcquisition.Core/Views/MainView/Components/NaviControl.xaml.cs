using CommunityToolkit.Mvvm.Messaging;
using DataAcquisition.Core.Views.MainView.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using DataAcquisition.Core.Views.WindowBase;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;

namespace DataAcquisition.Core.Views.MainView.Components
{
    public partial class NaviControl : UserControl
    {
        private ObservableCollection<TreeItemModel>? _baseItem;
        public ObservableCollection<TreeItemModel> BaseList
        {
            get => _baseItem;
            set => _baseItem = value;
        }

        private bool _isCollapsed = false;
        private bool _isUpdatingSelection = false;
        private bool _isMouseOverPopup = false;

        public NaviControl()
        {
            InitializeComponent();
            BaseList = MainViewModel.TreeItemModels;
            WeakReferenceMessenger.Default.Register<NaviSendMessenger<TreeItemModel>>(
                this, ChangeIsChecked
            );
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (scrollviewer != null)
            {
                scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 200 && !_isCollapsed)
            {
                _isCollapsed = true;
                CollapseAllItems();
            }
            else if (e.NewSize.Width >= 200)
            {
                _isCollapsed = false;
            }
        }

        private void CollapseAllItems()
        {
            if (BaseList == null) return;

            foreach (var item in BaseList)
            {
                CollapseItem(item);
            }
        }

        private void CollapseItem(TreeItemModel item)
        {
            item.IsExpanded = false;
            foreach (var child in item.Children)
            {
                CollapseItem(child);
            }
        }

        private void ChangeIsChecked(object recipient, NaviSendMessenger<TreeItemModel> message)
        {
            if (_isUpdatingSelection) return;

            _isUpdatingSelection = true;
            try
            {
                // 清除之前的选中状态
                if (olditemModel != null)
                {
                    olditemModel.IsChecked = false;
                }

                // 设置新的选中状态
                olditemModel = message.Model;
                if (olditemModel != null)
                {
                    olditemModel.IsChecked = true;
                }
            }
            finally
            {
                _isUpdatingSelection = false;
            }
        }

        public static TreeItemModel? olditemModel;

        private void NavToPage_Click(object sender, RoutedEventArgs e)
        {
            if (_isUpdatingSelection) return;

            if (sender is RadioButton radioButton && radioButton.Tag is TreeItemModel value)
            {
                _isUpdatingSelection = true;
                try
                {
                    if (value.HasChildren && ActualWidth < 200)
                    {
                        WeakReferenceMessenger.Default.Send(new ChangeMainBorderSizeMessanger());
                        value.IsExpanded = !value.IsExpanded;
                        
                    }
                    
                    // 如果是展开状态，处理子项的展开/折叠
                    if (ActualWidth >= 200)
                    {
                        if (value.HasChildren)
                        {
                            value.IsChecked = false;
                            if(olditemModel is not null)
                            {
                                olditemModel.IsChecked = true;
                            }
                            value.IsExpanded = !value.IsExpanded;
                            e.Handled = true;
                            return;
                        }
                    }

                    // 清除之前的选中状态
                    if (olditemModel != null)
                    {
                        olditemModel.IsChecked = false;
                    }

                    // 设置新的选中状态
                    //olditemModel = value;
                    value.IsChecked = true;

                    // 处理导航
                    if (value.PageCanInterchange == PageCanInterchange.NonePage)
                    {
                        var windowOpen = BreadCrumbBar.items.FirstOrDefault(e => e.Key == value);
                        if (windowOpen.Value is not null)
                        {
                            windowOpen.Value.Activate();
                            windowOpen.Value.Focusable = true;
                            return;
                        }
                        PageWindow pageWindow = new PageWindow(value);
                        BreadCrumbBar.items.Add(value, pageWindow);
                        pageWindow.Show();
                        return;
                    }

                    WeakReferenceMessenger.Default.Send(value);
                }
                finally
                {
                    _isUpdatingSelection = false;
                }
            }
        }

        private void Popup_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseOverPopup = false;
            if (sender is FrameworkElement element && element.Parent is Popup popup)
            {
                // 使用Dispatcher延迟关闭，给一个小的延时让鼠标有时间移动到子菜单
                Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    if (!_isMouseOverPopup)
                    {
                        popup.IsOpen = false;
                    }
                }), System.Windows.Threading.DispatcherPriority.Input);
            }
        }

        private void Popup_MouseEnter(object sender, MouseEventArgs e)
        {
            _isMouseOverPopup = true;
        }
    }
}
