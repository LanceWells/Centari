using Centari.Behaviors.Common;

namespace Centari.Behaviors.Leaves.Navigation;

public class IsOnFloorNode : INode<INavContext>
{
  private INavContext _context;

  public void Init(ref INavContext contextRef)
  {
    _context = contextRef;
  }

  public NodeState Process(double delta)
  {
    return _context.ThisMonster.IsOnFloor()
      ? NodeState.SUCCESS
      : NodeState.FAILURE;
  }
}
