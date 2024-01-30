using System.Collections.Generic;
using Centari.BehaviorTree;
using Centari.Navigation;
using Centari.Player;

namespace Centari.Monsters;

public class TestBallTree : Tree
{
  private TestBall _ball;

  private NavCoordinator _nav;

  private PlayerCharacter _player;

  public TestBallTree(
    TestBall ball,
    NavCoordinator nav,
    PlayerCharacter player
  )
  {
    _ball = ball;
    _nav = nav;
    _player = player;
  }

  protected override INode SetupTree()
  {
    TreeNode root = new OrNode(
      new List<INode>()
      {
        new TaskFollow(_ball, _nav, _player),
        new TaskIdle(),
      }
    );

    return root;
  }
}
