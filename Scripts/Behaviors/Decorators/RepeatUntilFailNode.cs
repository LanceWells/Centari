using Centari.Behaviors.Common;

namespace Centari.Behaviors;

public class RepeatUntilFailNode<T> : INode<T>
{
  private INode<T> _child;

  public RepeatUntilFailNode(INode<T> child)
  {
    _child = child;
  }

  public void Init(ref T contextRef)
  {
    _child.Init(ref contextRef);
  }

  public NodeState Process(double delta)
  {
    NodeState nodeState = _child.Process(delta);
    return nodeState switch
    {
      NodeState.FAILURE => NodeState.FAILURE,
      _ => NodeState.RUNNING,
    };
  }
}
