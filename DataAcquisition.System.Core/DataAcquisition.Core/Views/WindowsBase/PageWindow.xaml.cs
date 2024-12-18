using CommunityToolkit.Mvvm.Messaging;
using DataAcquisition.Core.Views.MainView.Components;
using DataAcquisition.Core.Views.MainView.Model;
using System.Windows;
using DataAcquisition.Core.Views.DialogView.Model;
using HandyControl.Controls;

namespace DataAcquisition.Core.Views.WindowBase
{
    /// <summary>
    /// PageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PageWindow : System.Windows.Window
    {
        public TreeItemModel ItemModel { get; set; }

        public PageWindow(TreeItemModel model, double width = 800, double height = 450)
        {
            Dialog.Register(MessageToken.NewPageToken,this);
            model.PageStatus = PageStatus.Windows;
            ItemModel = model;
            InitializeComponent();
            this.Width = width;
            this.Height = height;
            this.Title = ItemModel.Content;
            this.frame.Navigate(ItemModel.Page);
            
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            BreadCrumbBar.items.Remove(ItemModel);
            this.Close();
        }

        private void MiniSize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BackWindow_Click(object sender, RoutedEventArgs e)
        {
            if (ItemModel.PageCanInterchange == PageCanInterchange.NonePage)
            {
                return;
            }
            BreadCrumbBar.items.Remove(ItemModel);
            ItemModel.IsChecked = true;
            WeakReferenceMessenger.Default.Send(ItemModel);
            
            this.Close();
        }

        private void Max_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }

            else if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
        }
    }
}
