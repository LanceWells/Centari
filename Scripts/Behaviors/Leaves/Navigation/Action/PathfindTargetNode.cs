using System;
using System.Linq;
using Centari.Behaviors.Common;
using Godot;

namespace Centari.Behaviors.Leaves.Navigation;

public class PathfindTargetNode : INode<INavContext>
{
  INavContext _context;

  private Vector2 _lastKnownTargetPoint;

  private Vector2[] _knownPath = Array.Empty<Vector2>();

  private NodeTempo _pathfindTempo;

  private string _pathfindTempoKey;

  public void Init(ref INavContext contextRef)
  {
    _context = contextRef;
  }

  public PathfindTargetNode(ref NodeTempo pathfindTempo, string pathfindTempoKey)
  {
    _pathfindTempo = pathfindTempo;
    _pathfindTempoKey = pathfindTempoKey;
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

    if (onFloor)
    {
      Vector2[] path = GetPath(thisPos, targetPos);
      if (path.Length > 0)
      {
        _knownPath = path;
      }
    }

    if (_knownPath.Length == 0)
    {
      return NodeState.SUCCESS;
    }

    Vector2 nextPos = _knownPath[0];
    if (thisPos.DistanceTo(nextPos) < 32)
    {
      _knownPath = _knownPath.Skip(1).ToArray();
    }

    if (thisPos.Y - nextPos.Y > 32)
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

    bool moveLeft = _context.ThisMonster.Position > nextPos;

    _context.ThisMonster.Position = thisPos.MoveToward(nextPos, (float)delta * walkSpeed);
    _context.ThisMonster.SetAnimation(Monsters.MonsterAnimation.Run, moveLeft);

    return NodeState.RUNNING;
  }

  private Vector2[] GetPath(
    Vector2 thisPos,
    Vector2 targetPos
  )
  {
    float walkSpeed = _context.ThisMonster.WalkSpeed;

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
