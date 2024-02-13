using System.Collections.Generic;
using Centari.Behaviors.Common;

public abstract class AbstractEnqueueNode<TNode, TQueue> : INode<TNode>
{
  public abstract void Init(ref TNode contextRef);

  public abstract NodeState Process(double delta);

  protected Queue<TQueue> Queue => _queue;

  private Queue<TQueue> _queue;

  protected AbstractEnqueueNode(ref Queue<TQueue> queue)
  {
    _queue = queue;
  }
}
