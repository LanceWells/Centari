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
    Vector2 nextPos = _navContext.TrackedCreature.Position;

    if (thisPos.Y > nextPos.Y)
    {
      return NodeState.FAILURE;
    }

    float horizontalSpeed = _navContext.NextPoint.X < _navContext.ThisMonster.Position.X
      ? -_navContext.ThisMonster.WalkSpeed
      : _navContext.ThisMonster.WalkSpeed;

    Vector2 currVel = _navContext.ThisMonster.Velocity;
    Vector2 targetVel = new(horizontalSpeed, currVel.Y);

    _navContext.ThisMonster.Velocity = targetVel;

    return NodeState.RUNNING;
  }
}
