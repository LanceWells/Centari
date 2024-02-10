using Centari.BehaviorTree;
using Centari.Navigation;
using Centari.Player;
using Godot;

namespace Centari.Monsters;

public abstract class AbstractMonsterTask : TreeNode<INode>
{
  protected NavCoordinator _nav;

  private CharacterBody2D _creatureToTrack;

  protected CharacterBody2D TrackedCreature => _creatureToTrack;

  protected AbstractMonster _thisCreature;

  public AbstractMonsterTask(
    AbstractMonster monster,
    NavCoordinator nav,
    PlayerCharacter player
  ) : base()
  {
    _thisCreature = monster;
    _nav = nav;
    _creatureToTrack = player;
  }
}
