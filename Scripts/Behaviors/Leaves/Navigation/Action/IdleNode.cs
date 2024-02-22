using System;
using Centari.Behaviors.Common;

namespace Centari.Behaviors.Leaves.Navigation;

public class IdleNode : INode<INavContext>
{
  private INavContext _context;

  public void Init(ref INavContext contextRef)
  {
    _context = contextRef;
  }

  public NodeState Process(double delta)
  {
    _context.ThisMonster.SetAnimation(Monsters.MonsterAnimation.Idle, false);
    return NodeState.RUNNING;
  }
}
