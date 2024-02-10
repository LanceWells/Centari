using Centari.Behaviors.Common;

namespace Centari.Behaviors.Decorators;

public class InverterNode<T> : INode<T>
{
  private INode<T> _child;

  public InverterNode(INode<T> child)
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
      NodeState.RUNNING => NodeState.RUNNING,
      NodeState.FAILURE => NodeState.SUCCESS,
      NodeState.SUCCESS => NodeState.FAILURE,
      _ => NodeState.SUCCESS,
    };
  }
}
