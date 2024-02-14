using Centari.Behaviors.Common;
using Godot;

namespace Centari.Behaviors.Leaves.Navigation;

public class IsTargetVisibleNode : INode<INavContext>
{
  private INavContext _context;

  public void Init(ref INavContext contextRef)
  {
    _context = contextRef;
  }

  public NodeState Process(double delta)
  {
    if (_context.TrackedCreature == null)
    {
      return NodeState.FAILURE;
    }

    Vector2 fromRay = _context.ThisMonster.Position;
    Vector2 toRay = _context.TrackedCreature.Position;

    // If the target is directly on top of the monster, it's better to assume that we can see it.
    // This saves on the raycast calculation, and it also avoids an issue where 0-length raycasts
    // can fail to detect their target.
    if (fromRay.DistanceTo(toRay) < 32)
    {
      return NodeState.SUCCESS;
    }

    var world = _context.ThisMonster.GetWorld2D();
    var spaceState = world.DirectSpaceState;
    var query = PhysicsRayQueryParameters2D.Create(fromRay, toRay, 3);
    var result = spaceState.IntersectRay(query);

    return result.ContainsKey("position")
      ? NodeState.SUCCESS
      : NodeState.FAILURE;
  }
}
