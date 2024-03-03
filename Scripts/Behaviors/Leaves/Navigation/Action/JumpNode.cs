using Centari.Behaviors.Common;
using Godot;

namespace Centari.Behaviors;

public class JumpNode : AbstractMoveNode
{
  public override NodeState Process(double delta)
  {
    if (_ctx.Path.Length == 0)
    {
      return NodeState.FAILURE;
    }

    float walkSpeed = _ctx.ThisMonster.WalkSpeed;
    float gravity = _ctx.ThisMonster.Gravity;
    Vector2 thisPos = _ctx.ThisMonster.Position;
    Vector2 nextPos = _ctx.Path[0];

    bool onFloor = _ctx.ThisMonster.IsOnFloor();
    if (!onFloor)
    {
      if (nextPos.Y > thisPos.Y)
      {
        MoveHorizontal(nextPos.X, delta);
      }

      return NodeState.RUNNING;
    }

    EdgeCollisions e = GetEdgeCollisions();

    if (e.Left && e.Right)
    {
      return NodeState.FAILURE;
    }
    else if (!e.Left && !e.Right)
    {
      float jumpStrength = (nextPos.Y - thisPos.Y) * (gravity * 0.015f);
      Jump(jumpStrength);
    }
    else if (e.Left)
    {
      MoveHorizontal(thisPos.X + 32, delta);
    }
    else if (e.Right)
    {
      MoveHorizontal(thisPos.X - 32, delta);
    }

    if (thisPos.DistanceTo(nextPos) < 32)
    {
      return NodeState.SUCCESS;
    }

    return NodeState.RUNNING;
  }
}
