using Centari.Behaviors.Common;
using Godot;

namespace Centari.Behaviors;

public class WalkNode : AbstractMoveNode
{
  public override NodeState Process(double delta)
  {
    if (_ctx.Path.Length == 0)
    {
      return NodeState.SUCCESS;
    }

    Vector2 thisPos = _ctx.ThisMonster.Position;
    Vector2 nextPos = _ctx.Path[0];


    MoveTo(nextPos, delta);

    if (thisPos.DistanceTo(nextPos) < 32)
    {
      return NodeState.SUCCESS;
    }
    return NodeState.RUNNING;
  }
}
