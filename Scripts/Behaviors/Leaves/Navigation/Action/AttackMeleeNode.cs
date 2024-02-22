using System;
using Centari.Behaviors.Common;

namespace Centari.Behaviors.Leaves.Navigation;

public class AttackMeleeNode : INode<INavContext>
{
  private INavContext _context;

  public void Init(ref INavContext contextRef)
  {
    _context = contextRef;
  }

  public NodeState Process(double delta)
  {
    Console.WriteLine("rawr");
    _context.ThisMonster.SetAnimation(Monsters.MonsterAnimation.Idle, false);
    return NodeState.SUCCESS;
  }
}
