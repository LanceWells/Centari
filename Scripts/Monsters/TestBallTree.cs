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
    TreeNode<INode> root = new OrNode<INode>(
      new List<INode>()
      {
        new PathfindNode(new List<INavNode>()
        {
          new TaskJump(_ball, _nav, _player),
          new TaskAirFollow(_ball, _nav, _player),
          new TaskFollow(_ball, _nav, _player),
        },
        _nav,
        _player,
        _ball
        ),
        new TaskIdle(),
      }
    );

    return root;
  }
}
