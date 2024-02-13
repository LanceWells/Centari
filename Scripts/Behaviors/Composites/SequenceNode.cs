using System.Collections.Generic;
using Centari.Behaviors.Common;

namespace Centari.Behaviors.Composites;

public class SequenceNode<T> : INode<T>
{
  private List<INode<T>> _children;

  private int _nodePointer;

  public void Init(ref T contextRef)
  {
    foreach (INode<T> node in _children)
    {
      node.Init(ref contextRef);
    }

    _nodePointer = 0;
  }

  public SequenceNode(List<INode<T>> children)
  {
    _children = children;
  }

  public NodeState Process(double delta)
  {
    foreach (INode<T> child in _children)
    {
      switch (child.Process(delta))
      {
        case NodeState.SUCCESS:
          break;
        case NodeState.FAILURE:
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
