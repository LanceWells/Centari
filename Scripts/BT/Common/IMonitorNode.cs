using System.Collections.Generic;

namespace Centari.BT;

public abstract class AbstractMonitorNode<T> : INode
where T : struct
{
  protected INode _child;

  protected T _itemToMonitor;

  protected T _cachedItemToMonitor;

  protected NodeState _state = NodeState.RUNNING;

  public AbstractMonitorNode(
    T itemToMonitor,
    INode child
  )
  {
    _child = child;
    _itemToMonitor = itemToMonitor;
    _cachedItemToMonitor = itemToMonitor;
  }

  public void Cancel()
  {
    _child.Cancel();
  }

  public void Reset()
  {
    _state = NodeState.RUNNING;
    _child.Reset();
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
