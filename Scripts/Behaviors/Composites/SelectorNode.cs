using System.Collections.Generic;
using Centari.Behaviors.Common;

namespace Centari.Behaviors.Composites;

/// <summary>
/// This node acts as an "OR" statement. The idea is that it will process nodes until Its goal is to add fallback behavior. For example, this
/// node could be used in the context of opening a door. 
/// </summary>
/// <typeparam name="T"></typeparam>
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
