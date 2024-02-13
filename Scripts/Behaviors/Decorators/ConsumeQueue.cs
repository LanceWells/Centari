using System.Collections.Generic;
using Centari.Behaviors.Common;

namespace Centari.Behaviors.Composites;

public class ConsumeQueueNode<TContext, TQueue> : INode<TContext>
{
  private INode<TContext> _child;

  private Queue<TQueue> _queue;

  private TContext _context;

  public void Init(ref TContext contextRef)
  {
    _child.Init(ref contextRef);
    _context = contextRef;
  }

  public ConsumeQueueNode(INode<TContext> child, ref Queue<TQueue> queue)
  {
    _child = child;
    _queue = queue;
  }

  public NodeState Process(double delta)
  {
    if (_queue.Count > 0)
    {
      _child.Init(ref _context);

      switch (_child.Process(delta))
      {
        case NodeState.SUCCESS:
          _queue.Dequeue();
          return NodeState.RUNNING;
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
