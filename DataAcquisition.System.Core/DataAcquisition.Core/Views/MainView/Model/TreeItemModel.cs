using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using DataAcquisition.Model.Model.Login;

namespace DataAcquisition.Core.Views.MainView.Model
{
    public partial class TreeItemModel : ObservableObject
    {
        /// <summary>
        /// 持久化使用组件
        /// </summary>
        public bool IsPersistence { get; set; } = true;

        public string? Content { get; set; }

        public LoginAuth LoginAuth { get; set; }
        
        public object? Icon { get; set; } // 12.07 更改为Object类型 使可以接受更多用户想要类型的Icon 而不需要更改源码

        public Page? Page { get; set; }

        [ObservableProperty] private bool _isChecked;

        [ObservableProperty] private bool _isExpanded;

        public ObservableCollection<TreeItemModel> Children { get; set; } = new();

        public bool HasChildren => Children.Count > 0;

        public PageCanInterchange PageCanInterchange { get; set; } = PageCanInterchange.Can;

        public TreeItemModel()
        {
        }

        public PageStatus PageStatus { get; set; } = PageStatus.Page;

        public TreeItemModel(string con)
        {
            this.Content = con;
        }
    }
}