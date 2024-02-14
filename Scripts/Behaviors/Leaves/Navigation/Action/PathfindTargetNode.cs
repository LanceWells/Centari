using System;
using System.Linq;
using Centari.Behaviors.Common;
using Godot;

namespace Centari.Behaviors.Leaves.Navigation;

public class PathfindTargetNode : INode<INavContext>
{
  INavContext _context;

  private Vector2 _lastKnownTargetPoint;

  private Vector2[] _lastKnownPath = Array.Empty<Vector2>();

  public void Init(ref INavContext contextRef)
  {
    _context = contextRef;
  }

  public NodeState Process(double delta)
  {
    if (_context.TrackedCreature == null)
    {
      return NodeState.SUCCESS;
    }

    Vector2 thisPos = _context.ThisMonster.Position;
    Vector2 targetPos = _context.TrackedCreature.Position;

    bool onFloor = _context.ThisMonster.IsOnFloor();
    float walkSpeed = _context.ThisMonster.WalkSpeed;
    float gravity = _context.ThisMonster.Gravity;

    Vector2[] path = _lastKnownPath;
    if (onFloor)
    {
      path = GetPath(thisPos, targetPos);
      if (path.Length > 0)
      {
        _lastKnownPath = path;
      }
    }

    if (path.Length == 0)
    {
      return NodeState.SUCCESS;
    }

    Vector2 nextPos = path[0];

    if (thisPos.Y > nextPos.Y)
    {
      // This nested check is important. If we don't ask if we're on the floor, the horizontal
      // movement can happen too early, and the monster will run into the wall on the way up.
      if (onFloor)
      {
        // Factor in our gravity. The multiplier here is because there's no 1:1 relationship between
        // gravity and how much we should jump. It's an estimated amount that looks natural.
        float jumpStrength = (nextPos.Y - thisPos.Y) * (gravity * 0.015f);

        Vector2 targetVel = new(0, jumpStrength);

        _context.ThisMonster.Velocity = targetVel;
      }

      return NodeState.RUNNING;
    }
    else if (Math.Abs(thisPos.X - nextPos.X) < 0.1)
    {
      return NodeState.SUCCESS;
    }

    _context.ThisMonster.Position = thisPos.MoveToward(nextPos, (float)delta * walkSpeed);

    return NodeState.RUNNING;
  }

  private Vector2[] GetPath(
    Vector2 thisPos,
    Vector2 targetPos
  )
  {
    float walkSpeed = _context.ThisMonster.WalkSpeed;

    Vector2[] path = _context.Nav.GetPath(
      _context.NavModes,
      thisPos,
      targetPos
    );

    // If we couldn't find the target, try its last known position. In future, this can be a little
    // more advanced, e.g. we check to see if there's a raycast to the target before setting its
    // last known position.
    if (path.Length == 0)
    {
      path = _context.Nav.GetPath(
        _context.NavModes,
        thisPos,
        _lastKnownTargetPoint
      );

      if (path.Length == 0)
      {
        return Array.Empty<Vector2>();
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

    return path;
  }
}
