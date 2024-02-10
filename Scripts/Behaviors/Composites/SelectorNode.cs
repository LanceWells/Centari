using System.Collections.Generic;
using Centari.Behaviors.Common;

namespace Centari.Behaviors.Composites;

public class SelectorNode<T> : INode<T>
{
  private List<INode<T>> _children;

  public void Init(ref T contextRef)
  {
    foreach (INode<T> node in _children)
    {
      node.Init(ref contextRef);
    }
  }

  public SelectorNode(List<INode<T>> children)
  {
    _children = children;
  }

  public NodeState Process(double delta)
  {
    foreach (INode<T> node in _children)
    {
      switch (node.Process(delta))
      {
        case NodeState.FAILURE:
          return NodeState.FAILURE;
        case NodeState.RUNNING:
          return NodeState.RUNNING;
        case NodeState.SUCCESS:
          continue;
        default:
          continue;
      }
    }

    // If every child node returned success, return success.
    return NodeState.SUCCESS;
  }
}
