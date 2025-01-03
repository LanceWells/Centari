using System;
using System.Linq;
using Centari.Behaviors.Common;
using Godot;

namespace Centari.Behaviors.Leaves.Navigation;

// First, see if there is a path to our target.
// If there is, identify the next point to walk towards.
// Perhaps, if instead of walking to the point, we determine if it means "walk left" or
// "walk right".

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

    _knownPath = GetPath(thisPos, targetPos);

    if (_knownPath.Length > 0)
    {
      AlignVertical(thisPos, _knownPath[0]);

      if (thisPos.DistanceTo(_knownPath[0]) < 32)
      {
        _knownPath = _knownPath.Skip(1).ToArray();
      }
    }

    if (_knownPath.Length == 0)
    {
      return NodeState.SUCCESS;
    }

    Vector2 nextPos = _knownPath[0];

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

    return path;
  }

  private void AlignVertical(Vector2 thisPos, Vector2 nextPos)
  {
    bool onFloor = _context.ThisMonster.IsOnFloor();
    if (!onFloor)
    {
      return;
    }

    if (Math.Abs(thisPos.Y - nextPos.Y) <= 32)
    {
      return;
    }

    // double monsterHitboxWidth = _context.ThisMonster.HitBox.Size.X;
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

      Vector2[] nextPoint = new Vector2[1];
      nextPoint[0] = new Vector2(
        thisPos.X + 32,
        thisPos.Y
      );

      _knownPath = nextPoint.Concat(_knownPath).ToArray();
    }
    else if (rightResult.ContainsKey("position"))
    {
      Console.WriteLine($"Left collision at ${rightResult}");

      Vector2[] nextPoint = new Vector2[1];
      nextPoint[0] = new Vector2(
        thisPos.X - 32,
        thisPos.Y
      );

      _knownPath = nextPoint.Concat(_knownPath).ToArray();
    }
  }
}
