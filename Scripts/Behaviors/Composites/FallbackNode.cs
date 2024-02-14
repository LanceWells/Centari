using System;
using System.Collections.Generic;
using Centari.Behaviors.Common;

namespace Centari.Behaviors;

public class FallbackNode<T> : INode<T>
{
  private List<INode<T>> _children;

  private int _nodePointer;

  private string _nodeName;

  public void Init(ref T contextRef)
  {
    foreach (INode<T> node in _children)
    {
      node.Init(ref contextRef);
    }
  }

  public FallbackNode(string nodeName, List<INode<T>> children)
  {
    _children = children;
    _nodeName = nodeName;
  }

  public NodeState Process(double delta)
  {
    for (int i = _nodePointer; i < _children.Count; i++)
    {
      var child = _children[i];
      switch (child.Process(delta))
      {
        case NodeState.SUCCESS:
          return NodeState.SUCCESS;
        case NodeState.FAILURE:
          _nodePointer++;
          // Console.WriteLine($"Failure at {_nodeName} child {i}");
          break;
        case NodeState.RUNNING:
          return NodeState.RUNNING;
        default:
          break;
      }
    }

    return NodeState.FAILURE;
  }
}
