using System.Collections.Generic;
using Centari.BehaviorTree;
using Centari.Navigation;

namespace Centari.Monsters;

public abstract class AbstractBallTask : TreeNode
{
  protected NavCoordinator _nav;

  protected Player _player;

  protected TestBall _thisCreature;

  public AbstractBallTask(
    TestBall ball,
    NavCoordinator nav,
    Player player
  ) : base()
  {
    _thisCreature = ball;
    _nav = nav;
    _player = player;
  }
}
