using Centari.Behaviors.Common;
using Centari.Behaviors.Contexts;
using Godot;

namespace Centari.Behaviors.Leaves;

public class SetNextPointNode : INode<INavContext>
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
    float walkSpeed = _navContext.ThisMonster.WalkSpeed;

    Vector2[] path = _navContext.Nav.GetPath(
      _navContext.NavModes,
      thisPos,
      nextPos
    );

    if (path.Length == 0)
    {
      return NodeState.FAILURE;
    }

    if (path.Length > 1 && thisPos.DistanceTo(nextPos) < (walkSpeed * 0.1))
    {
      _navContext.NextPoint = path[1];
    }
    else if (path.Length > 0)
    {
      _navContext.NextPoint = path[0];
    }

    return NodeState.SUCCESS;
  }
}
