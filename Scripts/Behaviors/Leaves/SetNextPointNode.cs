using System.Collections.Generic;
using System.Linq;
using Centari.Behaviors.Common;
using Centari.Behaviors.Contexts;
using Godot;

namespace Centari.Behaviors.Leaves;

public class SetNextPointNode : AbstractEnqueueNode<INavContext, Vector2>
{
  private INavContext _navContext;

  private Vector2 _lastKnownTargetPoint;

  public override void Init(ref INavContext contextRef)
  {
    _navContext = contextRef;
  }

  public SetNextPointNode(ref Queue<Vector2> queue)
  : base(ref queue)
  { }

  public override NodeState Process(double delta)
  {
    Vector2 thisPos = _navContext.ThisMonster.Position;
    Vector2 targetPos = _navContext.TrackedCreature.Position;
    float walkSpeed = _navContext.ThisMonster.WalkSpeed;

    Vector2[] path = _navContext.Nav.GetPath(
      _navContext.NavModes,
      thisPos,
      targetPos
    );

    // If we couldn't find the target, try its last known position. In future, this can be a little
    // more advanced, e.g. we check to see if there's a raycast to the target before setting its
    // last known position.
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
      path = path.Skip(1).ToArray();
    }

    Queue.Clear();
    foreach (var vec in path)
    {
      Queue.Enqueue(vec);
    }

    return NodeState.SUCCESS;
  }
}
