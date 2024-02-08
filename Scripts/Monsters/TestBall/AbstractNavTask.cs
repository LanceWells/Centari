using System;
using Centari.BehaviorTree;
using Centari.Navigation;
using Centari.Player;
using Godot;

namespace Centari.Monsters;

public abstract class AbstractNavTask : AbstractMonsterTask, INavNode
{
  private Vector2[] _path = Array.Empty<Vector2>();

  protected Vector2 NextPoint
  {
    get
    {
      if (_path.Length == 0)
      {
        return new Vector2(-1, -1);
      }
      return _path[0];
    }
  }

  public void SetNav(Vector2[] path)
  {
    _path = path;
  }

  protected AbstractNavTask(
    TestBall ball,
    NavCoordinator nav,
    PlayerCharacter player
  ) : base(ball, nav, player)
  {
    _path = new Vector2[0];
  }
}

public interface INavNode : INode
{
  public void SetNav(Vector2[] path);
}
