using System;
using System.Collections.Generic;
using System.Windows;

namespace Xioa.Admin.Core.Views.FlowView.Component.Models;

public class FlowDiagramData
{
    public List<NodeInfo> Nodes { get; set; } = new();
    public List<ConnectionData> Connections { get; set; } = new();
}

public class NodeInfo
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}

public class ConnectionData
{
    public string SourceId { get; set; }
    public string TargetId { get; set; }
} 