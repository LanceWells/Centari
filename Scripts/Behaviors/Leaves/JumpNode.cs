using Centari.Behaviors.Common;
using Centari.Behaviors.Contexts;
using Godot;

namespace Centari.Behaviors.Leaves;

public class JumpNode : INode<INavContext>
{
  private INavContext _navContext;

  public void Init(ref INavContext contextRef)
  {
    _navContext = contextRef;
  }

  public NodeState Process(double delta)
  {
    Vector2 thisPos = _navContext.ThisMonster.Position;
    Vector2 nextPos = _navContext.TrackedCreature.Position;
    float gravity = _navContext.ThisMonster.Gravity;

    if (thisPos.Y < nextPos.Y)
    {
      return NodeState.FAILURE;
    }

    float jumpStrength = (nextPos.Y - thisPos.Y) * (gravity * 0.01f);

    Vector2 targetVel = new(0, jumpStrength);

    _navContext.ThisMonster.Velocity = targetVel;

    return NodeState.SUCCESS;
  }
}
