using System;
using System.Collections.Generic;
using Centari.Behaviors.Common;

namespace Centari.Behaviors;

public class ReactiveFallbackNode<T> : INode<T>
{
  private List<INode<T>> _children;

  private string _nodeName;

  public void Init(ref T contextRef)
  {
    foreach (INode<T> node in _children)
    {
      node.Init(ref contextRef);
    }
  }

  public ReactiveFallbackNode(string nodeName, List<INode<T>> children)
  {
    _children = children;
    _nodeName = nodeName;
  }

  public NodeState Process(double delta)
  {
    for (int i = 0; i < _children.Count; i++)
    {
      var node = _children[i];
      switch (node.Process(delta))
      {
        case NodeState.SUCCESS:
          return NodeState.SUCCESS;
        case NodeState.FAILURE:
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
