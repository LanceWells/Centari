using Centari.Behaviors.Common;
using Godot;

namespace Centari.Behaviors;

public class FallNode : AbstractMoveNode
{
  public override NodeState Process(double delta)
  {
    bool isOnFloor = _ctx.ThisMonster.IsOnFloor();
    if (!isOnFloor)
    {
      return NodeState.RUNNING;
    }

    if (_ctx.Path.Length == 0)
    {
      return NodeState.SUCCESS;
    }

    float gravity = _ctx.ThisMonster.Gravity;
    Vector2 thisPos = _ctx.ThisMonster.Position;
    Vector2 nextPos = _ctx.Path[0];

    EdgeCollisions e = GetEdgeCollisions();

    if (e.Left && e.Right)
    {
      return NodeState.FAILURE;
    }
    else if (!e.Left && !e.Right)
    {
      return NodeState.FAILURE;
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
