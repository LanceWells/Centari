using Centari.Behaviors.Common;

namespace Centari.Behaviors;

public class AlwaysSucceedNode<T> : INode<T>
{
  private INode<T> _child;

  public AlwaysSucceedNode(INode<T> child)
  {
    _child = child;
  }

  public void Init(ref T contextRef)
  {
    _child.Init(ref contextRef);
  }

  public NodeState Process(double delta)
  {
    _child.Process(delta);
    return NodeState.SUCCESS;
  }
}
