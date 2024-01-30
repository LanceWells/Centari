using Centari.BehaviorTree;
using Centari.Navigation;
using Centari.Player;

namespace Centari.Monsters;

public abstract class AbstractBallTask : TreeNode
{
  protected NavCoordinator _nav;

  protected PlayerCharacter _player;

  protected TestBall _thisCreature;

  public AbstractBallTask(
    TestBall ball,
    NavCoordinator nav,
    PlayerCharacter player
  ) : base()
  {
    _thisCreature = ball;
    _nav = nav;
    _player = player;
  }
}
