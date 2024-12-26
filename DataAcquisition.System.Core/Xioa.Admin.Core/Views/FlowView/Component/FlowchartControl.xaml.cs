using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Xioa.Admin.Core.Views.FlowView.Component
{
    public partial class FlowchartControl : UserControl
    {
        public FlowchartControl()
        {
            InitializeComponent();
        }

        public void AddNode(string nodeType, Point position)
        {
            var node = new Ellipse
            {
                Width = 50,
                Height = 50,
                Fill = Brushes.Blue,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            Canvas.SetLeft(node, position.X);
            Canvas.SetTop(node, position.Y);
            FlowCanvas.Children.Add(node);
        }
    }
} 