using System;
using System.Collections.Generic;
using Centari.Behaviors.Common;

namespace Centari.Behaviors;

public class ReactiveSequenceNode<T> : INode<T>
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

  public ReactiveSequenceNode(string nodeName, List<INode<T>> children)
  {
    _children = children;
    _nodeName = nodeName;
  }

  public NodeState Process(double delta)
  {
    for (int i = 0; i < _children.Count; i++)
    {
      var child = _children[i];
      switch (child.Process(delta))
      {
        case NodeState.SUCCESS:
          break;
        case NodeState.FAILURE:
          // Console.WriteLine($"Failure at {_nodeName} child {i}");
          return NodeState.FAILURE;
        case NodeState.RUNNING:
          return NodeState.RUNNING;
        default:
          break;
      }
    }

    return NodeState.SUCCESS;
  }
}
