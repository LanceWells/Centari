using System.Collections.Generic;
using Centari.Behaviors.Common;

namespace Centari.Behaviors;

public class ReactiveFallbackNode<T> : INode<T>
{
  private List<INode<T>> _children;

  public void Init(ref T contextRef)
  {
    foreach (INode<T> node in _children)
    {
      node.Init(ref contextRef);
    }
  }

  public ReactiveFallbackNode(List<INode<T>> children)
  {
    _children = children;
  }

  public NodeState Process(double delta)
  {
    foreach (INode<T> node in _children)
    {
      switch (node.Process(delta))
      {
        case NodeState.SUCCESS:
          return NodeState.SUCCESS;
        case NodeState.FAILURE:
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
