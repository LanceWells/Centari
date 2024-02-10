using System;
using Centari.BehaviorTree;
using Centari.Navigation;
using Centari.Player;
using Godot;

namespace Centari.Monsters;

public abstract class AbstractNavTask : AbstractMonsterTask, INavNode
{
  private Vector2 _nextPoint = Vector2.Zero;

  protected Vector2 NextPoint => _nextPoint;

  public virtual void SetNextPoint(Vector2 nextPoint)
  {
    _nextPoint = nextPoint;
  }

  protected AbstractNavTask(
    AbstractMonster monster,
    NavCoordinator nav,
    PlayerCharacter player
  ) : base(monster, nav, player)
  { }
}

public interface INavNode : INode
{
  public void SetNextPoint(Vector2 nextPoint);
}
