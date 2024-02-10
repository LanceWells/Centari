using Centari.Behaviors.Common;
using Centari.Behaviors.Contexts;
using Godot;

namespace Centari.Behaviors.Leaves;

public class WalkHorizontalNode : INode<INavContext>
{
  private INavContext _navContext;

  public void Init(ref INavContext contextRef)
  {
    _navContext = contextRef;
  }

  public NodeState Process(double delta)
  {
    Vector2 thisPos = _navContext.ThisMonster.Position;
    Vector2 nextPos = _navContext.NextPoint;
    float walkSpeed = _navContext.ThisMonster.WalkSpeed;

    if (thisPos.Y > nextPos.Y)
    {
      return NodeState.FAILURE;
    }

    _navContext.ThisMonster.Position = thisPos.MoveToward(nextPos, (float)delta * walkSpeed);

    return NodeState.RUNNING;
  }
}
