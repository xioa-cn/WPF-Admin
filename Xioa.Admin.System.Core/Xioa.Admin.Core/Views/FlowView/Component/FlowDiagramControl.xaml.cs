using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Xioa.Admin.Core.Views.FlowView.Component
{
    public partial class FlowDiagramControl : UserControl
    {
     

        public FlowDiagramControl()
        {
            InitializeComponent();
           
        }

        private void ToolboxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button button)
            {
                // 启动拖放操作，将按钮的 Tag 作为拖放的数据
                DragDrop.DoDragDrop(button, button.Tag, DragDropEffects.Copy);
            }
        }
    }
}