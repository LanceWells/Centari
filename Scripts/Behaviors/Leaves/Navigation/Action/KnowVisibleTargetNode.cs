using System;
using Centari.Behaviors.Common;
using Godot;

namespace Centari.Behaviors.Leaves.Navigation;

public class KnowVisibleTargetNode : INode<INavContext>
{
  private INavContext _context;

  public void Init(ref INavContext contextRef)
  {
    _context = contextRef;
  }

  public NodeState Process(double delta)
  {
    if (_context.PossibleTargets.Count == 0)
    {
      return NodeState.FAILURE;
    }

    // TODO: Find a way to prioritize.
    foreach (var target in _context.PossibleTargets)
    {
      long targetId = (long)target.GetInstanceId();

      Vector2 fromRay = _context.ThisMonster.Position;
      Vector2 toRay = target.Position;

      var world = _context.ThisMonster.GetWorld2D();
      var spaceState = world.DirectSpaceState;
      var query = PhysicsRayQueryParameters2D.Create(fromRay, toRay, 3);
      var result = spaceState.IntersectRay(query);
      query.Dispose();

      if (result.ContainsKey("collider_id") && (long)result["collider_id"].Obj == targetId)
      {
        _context.TrackedCreature = target;
        return NodeState.SUCCESS;
      }
    }

    return NodeState.FAILURE;
  }
}
