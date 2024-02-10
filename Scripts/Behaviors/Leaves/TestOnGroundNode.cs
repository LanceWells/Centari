using Centari.Behaviors.Common;
using Centari.Behaviors.Contexts;

namespace Centari.Behaviors.Leaves;

public class TestOnGroundNode : INode<INavContext>
{
  private INavContext _navContext;

  public void Init(ref INavContext contextRef)
  {
    _navContext = contextRef;
  }

  public NodeState Process(double delta)
  {
    bool onFloor = _navContext.ThisMonster.IsOnFloor();
    return onFloor ? NodeState.SUCCESS : NodeState.FAILURE;
  }
}
