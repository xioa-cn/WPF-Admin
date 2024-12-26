using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Xioa.Admin.Core.Views.FlowView.Component.Models;

// 用于存储连接线相关信息的类

// 节点数据类
public class NodeData
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public FrameworkElement Element { get; set; }
    public List<ConnectionInfo> Connections { get; set; } = new();
}

public abstract class DiagramCommand
{
    public abstract void Execute();
    public abstract void Undo();
}

public class AddNodeCommand : DiagramCommand
{
    private readonly Canvas canvas;
    private readonly FrameworkElement node;
    private readonly Point position;
    private readonly Dictionary<string, NodeData> nodesById;
    private readonly NodeData nodeData;

    public AddNodeCommand(Canvas canvas, FrameworkElement node, Point position, Dictionary<string, NodeData> nodesById, NodeData nodeData)
    {
        this.canvas = canvas;
        this.node = node;
        this.position = position;
        this.nodesById = nodesById;
        this.nodeData = nodeData;
    }

    public override void Execute()
    {
        Canvas.SetLeft(node, position.X);
        Canvas.SetTop(node, position.Y);
        canvas.Children.Add(node);
        nodesById[nodeData.Id] = nodeData;
    }

    public override void Undo()
    {
        canvas.Children.Remove(node);
        nodesById.Remove(nodeData.Id);
    }
}

public class MoveNodeCommand : DiagramCommand
{
    private readonly FrameworkElement node;
    private readonly Point oldPosition;
    private readonly Point newPosition;

    public MoveNodeCommand(FrameworkElement node, Point oldPosition, Point newPosition)
    {
        this.node = node;
        this.oldPosition = oldPosition;
        this.newPosition = newPosition;
    }

    public override void Execute()
    {
        Canvas.SetLeft(node, newPosition.X);
        Canvas.SetTop(node, newPosition.Y);
    }

    public override void Undo()
    {
        Canvas.SetLeft(node, oldPosition.X);
        Canvas.SetTop(node, oldPosition.Y);
    }
}

public class AddConnectionCommand : DiagramCommand
{
    private readonly Canvas canvas;
    private readonly Line line;
    private readonly Polygon arrow;
    private readonly ConnectionInfo connectionInfo;
    private readonly NodeData sourceNodeData;
    private readonly NodeData targetNodeData;

    public AddConnectionCommand(Canvas canvas, Line line, Polygon arrow, ConnectionInfo connectionInfo,
        NodeData sourceNodeData, NodeData targetNodeData)
    {
        this.canvas = canvas;
        this.line = line;
        this.arrow = arrow;
        this.connectionInfo = connectionInfo;
        this.sourceNodeData = sourceNodeData;
        this.targetNodeData = targetNodeData;
    }

    public override void Execute()
    {
        canvas.Children.Add(line);
        canvas.Children.Add(arrow);
        sourceNodeData.Connections.Add(connectionInfo);
        targetNodeData.Connections.Add(connectionInfo);
    }

    public override void Undo()
    {
        canvas.Children.Remove(line);
        canvas.Children.Remove(arrow);
        sourceNodeData.Connections.Remove(connectionInfo);
        targetNodeData.Connections.Remove(connectionInfo);
    }
}

public class DeleteConnectionCommand : DiagramCommand
{
    private readonly Canvas canvas;
    private readonly Line line;
    private readonly Polygon arrow;
    private readonly ConnectionInfo connectionInfo;
    private readonly NodeData sourceNodeData;
    private readonly NodeData targetNodeData;

    public DeleteConnectionCommand(Canvas canvas, Line line, Polygon arrow, ConnectionInfo connectionInfo,
        NodeData sourceNodeData, NodeData targetNodeData)
    {
        this.canvas = canvas;
        this.line = line;
        this.arrow = arrow;
        this.connectionInfo = connectionInfo;
        this.sourceNodeData = sourceNodeData;
        this.targetNodeData = targetNodeData;
    }

    public override void Execute()
    {
        canvas.Children.Remove(line);
        canvas.Children.Remove(arrow);
        sourceNodeData.Connections.Remove(connectionInfo);
        targetNodeData.Connections.Remove(connectionInfo);
    }

    public override void Undo()
    {
        canvas.Children.Add(line);
        canvas.Children.Add(arrow);
        sourceNodeData.Connections.Add(connectionInfo);
        targetNodeData.Connections.Add(connectionInfo);
    }
} 