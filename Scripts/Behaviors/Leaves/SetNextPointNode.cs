using Centari.Behaviors.Common;
using Centari.Behaviors.Contexts;
using Godot;

namespace Centari.Behaviors.Leaves;

public class SetNextPointNode : INode<INavContext>
{
  private INavContext _navContext;

  private Vector2 _lastKnownTargetPoint;

  public void Init(ref INavContext contextRef)
  {
    _navContext = contextRef;
  }

  public NodeState Process(double delta)
  {
    Vector2 thisPos = _navContext.ThisMonster.Position;
    Vector2 targetPos = _navContext.TrackedCreature.Position;
    float walkSpeed = _navContext.ThisMonster.WalkSpeed;

    Vector2[] path = _navContext.Nav.GetPath(
      _navContext.NavModes,
      thisPos,
      targetPos
    );

    if (path.Length == 0)
    {
      path = _navContext.Nav.GetPath(
        _navContext.NavModes,
        thisPos,
        _lastKnownTargetPoint
      );

      if (path.Length == 0)
      {
        return NodeState.FAILURE;
      }
    }
    else
    {
      _lastKnownTargetPoint = targetPos;
    }

    if (path.Length > 1 && thisPos.DistanceTo(path[0]) < (walkSpeed * 0.1f))
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
