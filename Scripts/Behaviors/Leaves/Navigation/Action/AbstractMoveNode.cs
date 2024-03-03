using Centari.Behaviors.Common;
using Godot;

namespace Centari.Behaviors;

public abstract class AbstractMoveNode : INode<INavContext>
{
  protected INavContext _ctx;

  public void Init(ref INavContext contextRef)
  {
    _ctx = contextRef;
  }

  public abstract NodeState Process(double delta);

  protected void MoveTo(Vector2 nextPos, double delta)
  {
    float walkSpeed = _ctx.ThisMonster.WalkSpeed;
    Vector2 thisPos = _ctx.ThisMonster.Position;

    _ctx.ThisMonster.Position = thisPos.MoveToward(nextPos, (float)delta * walkSpeed);
    _ctx.ThisMonster.SetAnimation(Monsters.MonsterAnimation.Run, true);
  }

  protected void MoveHorizontal(float xPos, double delta)
  {
    MoveTo(new(xPos, _ctx.ThisMonster.Position.Y), delta);
  }

  protected void Jump(float jumpStrength)
  {
    Vector2 targetVel = new(0, jumpStrength);
    _ctx.ThisMonster.Velocity = targetVel;
  }

  protected struct EdgeCollisions
  {
    public bool Left;

    public bool Right;
  }

  protected EdgeCollisions GetEdgeCollisions()
  {
    if (_ctx.Path.Length == 0)
    {
      return new EdgeCollisions
      {
        Left = false,
        Right = false
      };
    }

    Vector2 thisPos = _ctx.ThisMonster.Position;
    Vector2 nextPos = _ctx.Path[0];

    CollisionShape2D shape = _ctx.ThisMonster.HitBox;
    Rect2 shapeRect = shape.Shape.GetRect();

    var world = _ctx.ThisMonster.GetWorld2D();
    var spaceState = world.DirectSpaceState;

    float leftX = thisPos.X - 16 + (shape.Position.X - (shapeRect.Size.X / 2)) * _ctx.ThisMonster.Scale.X;
    float rightX = thisPos.X + 16 + (shape.Position.X + (shapeRect.Size.X / 2)) * _ctx.ThisMonster.Scale.X;

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

    bool leftCollides = leftResult.ContainsKey("position");
    bool rightCollides = rightResult.ContainsKey("position");

    return new EdgeCollisions
    {
      Left = leftCollides,
      Right = rightCollides
    };
  }
}
