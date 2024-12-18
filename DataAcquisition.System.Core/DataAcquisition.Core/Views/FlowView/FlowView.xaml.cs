using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace DataAcquisition.Core.Views.FlowView;

public partial class FlowView : Page
{
    public FlowView()
    {
        DataContext = new FlowViewModel();
        InitializeComponent();
    }

    private void AddNode_Click(object sender, RoutedEventArgs e)
    {
        flowchartControl.AddNode("ProcessNode", new Point(100, 100));
    }
}