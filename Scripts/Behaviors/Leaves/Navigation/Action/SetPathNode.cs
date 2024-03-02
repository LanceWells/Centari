using System.Linq;
using Centari.Behaviors.Common;
using Godot;

namespace Centari.Behaviors.Leaves.Navigation;

public class SetPathNode : INode<INavContext>
{
  private INavContext _context;

  private Vector2 _lastKnownTargetPoint;

  public void Init(ref INavContext contextRef)
  {
    _context = contextRef;
  }

  public NodeState Process(double delta)
  {
    if (_context.TrackedCreature == null)
    {
      return NodeState.FAILURE;
    }

    float walkSpeed = _context.ThisMonster.WalkSpeed;

    Vector2 thisPos = _context.ThisMonster.Position;
    Vector2 targetPos = _context.TrackedCreature.Position;

    Vector2[] path = _context.Nav.GetPath(
      _context.ThisMonster.NavOptions,
      thisPos,
      targetPos
    );

    // If we couldn't find the target, try its last known position. In future, this can be a little
    // more advanced, e.g. we check to see if there's a raycast to the target before setting its
    // last known position.
    if (path.Length == 0)
    {
      path = _context.Nav.GetPath(
        _context.ThisMonster.NavOptions,
        thisPos,
        _lastKnownTargetPoint
      );

      if (path.Length == 0)
      {
        return NodeState.FAILURE;
      }
    }
    else
    {
      _lastKnownTargetPoint = targetPos;
    }

    if (path.Length > 1 && thisPos.DistanceTo(path[0]) < (walkSpeed * 0.1f))
    {
      path = path.Skip(1).ToArray();
    }

    _context.Path = path;

    return NodeState.SUCCESS;
  }
}
