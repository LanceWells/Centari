using System;
using System.Linq;
using Centari.Behaviors.Common;
using Godot;

namespace Centari.Behaviors;

public class PathfindTargetNodeV2 : INode<INavContext>
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

  public PathfindTargetNodeV2(ref NodeTempo pathfindTempo, string pathfindTempoKey)
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

    bool onFloor = _context.ThisMonster.IsOnFloor();
    float gravity = _context.ThisMonster.Gravity;
    Vector2 thisPos = _context.ThisMonster.Position;
    Vector2 targetPos = _context.TrackedCreature.Position;

    _knownPath = GetPath(thisPos, targetPos);

    if (_knownPath.Length == 0)
    {
      return NodeState.SUCCESS;
    }

    Vector2 nextPos = _knownPath[0];

    if (thisPos.Y - nextPos.Y > 32)
    {
      // Next position is up. Ensure the vertical is aligned.
      //
      // If we're under something, prepend another point that should get us out from under it, and
      // keep going.
      //
      // If not, and if we're on the ground, jump and return running.

      Vector2? verticalPos = AlignVertical(thisPos, nextPos);
      if (verticalPos == null && onFloor)
      {
        // Factor in our gravity. The multiplier here is because there's no 1:1 relationship between
        // gravity and how much we should jump. It's an estimated amount that looks natural.
        float jumpStrength = (nextPos.Y - thisPos.Y) * (gravity * 0.015f);
        Vector2 targetVel = new(0, jumpStrength);
        _context.ThisMonster.Velocity = targetVel;
      }
      else
      {
        MoveTo(verticalPos.Value, delta);
        return NodeState.RUNNING;
      }
    }

    if (thisPos.X > _knownPath[0].X)
    {
      // We're to the right, move left.
      MoveTo(new(thisPos.X - 32, thisPos.Y), delta);
    }
    else
    {
      MoveTo(new(thisPos.X + 32, thisPos.Y), delta);
    }

    return NodeState.RUNNING;
  }

  private void MoveTo(Vector2 nextPos, double delta)
  {
    float walkSpeed = _context.ThisMonster.WalkSpeed;
    Vector2 thisPos = _context.ThisMonster.Position;

    _context.ThisMonster.Position = thisPos.MoveToward(nextPos, (float)delta * walkSpeed);
    _context.ThisMonster.SetAnimation(Monsters.MonsterAnimation.Run, true);
  }

  private Vector2[] GetPath(
    Vector2 thisPos,
    Vector2 targetPos
  )
  {
    bool onFloor = _context.ThisMonster.IsOnFloor();
    if (!onFloor)
    {
      return _knownPath;
    }

    bool doUpdatePathfind = _pathfindTempo.TimeFor(_pathfindTempoKey);
    if (!doUpdatePathfind)
    {
      return _knownPath;
    }

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

    return path;
  }

  private Vector2? AlignVertical(Vector2 thisPos, Vector2 nextPos)
  {
    bool onFloor = _context.ThisMonster.IsOnFloor();
    if (!onFloor)
    {
      return null;
    }

    if (Math.Abs(thisPos.Y - nextPos.Y) <= 32)
    {
      return null;
    }

    CollisionShape2D shape = _context.ThisMonster.HitBox;
    Rect2 shapeRect = shape.Shape.GetRect();
    float walkSpeed = _context.ThisMonster.WalkSpeed;

    var world = _context.ThisMonster.GetWorld2D();
    var spaceState = world.DirectSpaceState;

    float leftX = thisPos.X - 16 + (shape.Position.X - (shapeRect.Size.X / 2)) * _context.ThisMonster.Scale.X;
    float rightX = thisPos.X + 16 + (shape.Position.X + (shapeRect.Size.X / 2)) * _context.ThisMonster.Scale.X;

    Vector2 leftRayOrigin = new()
    {
      X = leftX,
      Y = thisPos.Y
    };

    Vector2 rightRayOrigin = new()
    {
      X = rightX,
      Y = thisPos.Y
    };

    // bool nextPointUp = nextPos.Y < thisPos.Y;
    float rayLength = nextPos.Y - thisPos.Y;

    using var leftQuery = PhysicsRayQueryParameters2D.Create(
      leftRayOrigin, new(leftRayOrigin.X, leftRayOrigin.Y + rayLength), 1
    );

    using var rightQuery = PhysicsRayQueryParameters2D.Create(
      rightRayOrigin, new(rightRayOrigin.X, rightRayOrigin.Y + rayLength), 1
    );

    leftQuery.CollideWithBodies = true;
    rightQuery.CollideWithBodies = true;

    var leftResult = spaceState.IntersectRay(leftQuery);
    var rightResult = spaceState.IntersectRay(rightQuery);

    if (leftResult.ContainsKey("position"))
    {
      Console.WriteLine($"Left collision at ${leftResult}");

      Vector2 nextPoint = new(
        thisPos.X + 32,
        thisPos.Y
      );

      return nextPoint;
    }
    else if (rightResult.ContainsKey("position"))
    {
      Console.WriteLine($"Left collision at ${rightResult}");

      Vector2 nextPoint = new(
        thisPos.X - 32,
        thisPos.Y
      );

      return nextPoint;
    }

    return null;
  }
}
