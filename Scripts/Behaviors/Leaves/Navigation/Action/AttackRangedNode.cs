using System;
using Centari.Behaviors.Common;

namespace Centari.Behaviors.Leaves.Navigation;

public class AttackRangedNode : INode<INavContext>
{
  private INavContext _context;

  public void Init(ref INavContext contextRef)
  {
    _context = contextRef;
  }

  public NodeState Process(double delta)
  {
    Console.WriteLine("ranged rawr");
    return NodeState.SUCCESS;
  }
}
