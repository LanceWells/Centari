using Centari.Behaviors.Common;

namespace Centari.Behaviors.Leaves.Navigation;

public class IsTargetAboveNode : INode<INavContext>
{
  private INavContext _context;

  public void Init(ref INavContext contextRef)
  {
    _context = contextRef;
  }

  public NodeState Process(double delta)
  {
    return _context.TrackedCreature != null
      ? NodeState.SUCCESS
      : NodeState.FAILURE;
  }
}
