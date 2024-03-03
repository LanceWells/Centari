using System;
using Centari.Behaviors.Common;

namespace Centari.Behaviors.Leaves.Navigation;

public class IsNextPointAboveNode : INode<INavContext>
{
  private INavContext _context;

  public void Init(ref INavContext contextRef)
  {
    _context = contextRef;
  }

  public NodeState Process(double delta)
  {
    if (_context.Path.Length == 0)
    {
      return NodeState.FAILURE;
    }

    double nextPointY = _context.Path[0].Y;
    double thisPointY = _context.ThisMonster.Position.Y;

    if (Math.Abs(nextPointY - thisPointY) < 16)
    {
      return NodeState.FAILURE;
    }

    if (nextPointY < thisPointY)
    {
      return NodeState.SUCCESS;
    }

    return NodeState.FAILURE;
  }
}