using System;
using Centari.Behaviors.Common;

namespace Centari.Behaviors.Leaves.Navigation;

public class KnowVisibleTargetNode : INode<INavContext>
{
  private INavContext _context;

  public void Init(ref INavContext contextRef)
  {
    _context = contextRef;
  }

  public NodeState Process(double delta)
  {
    if (_context.PossibleTargets.Count == 0)
    {
      return NodeState.FAILURE;
    }

    // TODO: Find a way to prioritize.
    _context.TrackedCreature = _context.PossibleTargets[0];

    return NodeState.SUCCESS;
  }
}
