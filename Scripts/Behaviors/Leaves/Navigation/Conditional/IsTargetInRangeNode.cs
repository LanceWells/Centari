using Centari.Behaviors.Common;
using Godot;

namespace Centari.Behaviors.Leaves.Navigation;

public class IsTargetInRangeNode : INode<INavContext>
{
  private INavContext _context;

  public void Init(ref INavContext contextRef)
  {
    _context = contextRef;
  }

  public NodeState Process(double delta)
  {
    Vector2 thisPos = _context.ThisMonster.Position;
    Vector2 targetPos = _context.TrackedCreature.Position;

    float meleeRange = _context.ThisMonster.ProjectileRange;

    return thisPos.DistanceTo(targetPos) < meleeRange
      ? NodeState.SUCCESS
      : NodeState.FAILURE;
  }
}
