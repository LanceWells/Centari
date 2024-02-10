using Centari.Behaviors.Common;

namespace Centari.Behaviors.Decorators;

public class CacheResponseNode<T> : INode<T>
{
  private INode<T> _child;

  private NodeState _state = NodeState.RUNNING;

  public CacheResponseNode(INode<T> child)
  {
    _child = child;
  }

  public void Init(ref T contextRef)
  {
    _child.Init(ref contextRef);
  }

  public NodeState Process(double delta)
  {
    if (_state != NodeState.RUNNING)
    {
      return _state;
    }

    _state = _child.Process(delta);
    return _state;
  }
}
