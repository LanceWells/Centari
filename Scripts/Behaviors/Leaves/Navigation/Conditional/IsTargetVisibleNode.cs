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
    Vector2 fromRay = _context.ThisMonster.Position;
    Vector2 toRay = _context.TrackedCreature.Position;

    var world = _context.ThisMonster.GetWorld2D();
    var spaceState = world.DirectSpaceState;
    var query = PhysicsRayQueryParameters2D.Create(fromRay, toRay, 2);
    var result = spaceState.IntersectRay(query);

    return result.ContainsKey("position")
      ? NodeState.SUCCESS
      : NodeState.FAILURE;
  }
}
