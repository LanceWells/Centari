using Centari.Behaviors.Common;
using Godot;

namespace Centari.Behaviors;

public class SetPathNode : INode<INavContext>
{
  private INavContext _ctx;

  private NodeTempo _pathfindTempo;

  private string _pathfindTempoKey;

  public SetPathNode(
    ref NodeTempo pathfindTempo,
    string pathfindTempoKey
  )
  {
    _pathfindTempo = pathfindTempo;
    _pathfindTempoKey = pathfindTempoKey;
  }

  public void Init(ref INavContext contextRef)
  {
    _ctx = contextRef;
  }

  public NodeState Process(double delta)
  {
    if (!_ctx.ThisMonster.IsOnFloor())
    {
      return NodeState.SUCCESS;
    }

    if (_ctx.TrackedCreature == null)
    {
      return NodeState.SUCCESS;
    }

    bool doUpdatePathfind = _pathfindTempo.TimeFor(_pathfindTempoKey);
    if (!doUpdatePathfind)
    {
      return NodeState.SUCCESS;
    }

    Vector2 thisPos = _ctx.ThisMonster.Position;
    Vector2 targetPos = _ctx.TrackedCreature.Position;
    Vector2[] path = _ctx.Nav.GetPath(
      _ctx.ThisMonster.NavOptions,
      thisPos,
      targetPos
    );

    if (path.Length == 0)
    {
      path = _ctx.Nav.GetPath(
        _ctx.ThisMonster.NavOptions,
        thisPos,
        _ctx.LastKnownTargetPoint
      );

      if (path.Length == 0)
      {
        return NodeState.SUCCESS;
      }
    }
    else
    {
      _ctx.LastKnownTargetPoint = targetPos;
    }

    _ctx.Path = path;

    return NodeState.SUCCESS;
  }
}
