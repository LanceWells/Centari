using Centari.Behaviors.Common;

namespace Centari.Behaviors.Leaves.Navigation;

public class IsPotentialTargetVisibleNode : INode<INavContext>
{
  private INavContext _context;

  public void Init(ref INavContext contextRef)
  {
    _context = contextRef;
  }

  public NodeState Process(double delta)
  {
    // TODO
    return NodeState.FAILURE;
  }
}
