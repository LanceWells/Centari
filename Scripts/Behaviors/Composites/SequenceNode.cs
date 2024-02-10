using System.Collections.Generic;
using Centari.Behaviors.Common;

namespace Centari.Behaviors.Composites;

public class SequenceNode<T> : INode<T>
{
  private List<INode<T>> _children;

  public void Init(ref T contextRef)
  {
    foreach (INode<T> node in _children)
    {
      node.Init(ref contextRef);
    }
  }

  public SequenceNode(List<INode<T>> children)
  {
    _children = children;
  }

  public NodeState Process(double delta)
  {
    bool anyChildRunning = false;

    foreach (INode<T> child in _children)
    {
      switch (child.Process(delta))
      {
        case NodeState.RUNNING:
          return NodeState.RUNNING;
        case NodeState.FAILURE:
          return NodeState.FAILURE;
        case NodeState.SUCCESS:
          break;
        default:
          break;
      }
    }

    return anyChildRunning
      ? NodeState.RUNNING
      : NodeState.SUCCESS;
  }
}
