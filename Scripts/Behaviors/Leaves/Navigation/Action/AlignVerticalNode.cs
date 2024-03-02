using System;
using System.Linq;
using Centari.Behaviors.Common;
using Godot;

namespace Centari.Behaviors.Leaves.Navigation;

public class AlignVerticalNode : INode<INavContext>
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
      return NodeState.SUCCESS;
    }

    Vector2 nextPos = _context.Path[0];
    Vector2 monsterPos = _context.ThisMonster.Position;

    double monsterHitboxWidth = _context.ThisMonster.HitBox.Size.X;
    float walkSpeed = _context.ThisMonster.WalkSpeed;

    var world = _context.ThisMonster.GetWorld2D();
    var spaceState = world.DirectSpaceState;

    Vector2 leftRayOrigin = new()
    {
      X = (float)(monsterPos.X - (monsterHitboxWidth / 2)),
      Y = monsterPos.Y
    };

    Vector2 rightRayOrigin = new()
    {
      X = (float)(monsterPos.X - (monsterHitboxWidth / 2)),
      Y = monsterPos.Y
    };

    bool nextPointUp = nextPos.Y < monsterPos.Y;
    float rayLength = nextPointUp
      ? nextPos.Y - monsterPos.Y
      : monsterPos.Y - nextPos.Y;

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
      Vector2[] nextPoint = new Vector2[1];
      nextPoint[0] = new Vector2(
        monsterPos.X + walkSpeed,
        monsterPos.Y
      );

      _context.Path = _context.Path.Concat(nextPoint).ToArray();
    }
    else if (rightResult.ContainsKey("position"))
    {
      Vector2[] nextPoint = new Vector2[1];
      nextPoint[0] = new Vector2(
        monsterPos.X - walkSpeed,
        monsterPos.Y
      );

      _context.Path = _context.Path.Concat(nextPoint).ToArray();
    }

    return NodeState.SUCCESS;
  }
}
