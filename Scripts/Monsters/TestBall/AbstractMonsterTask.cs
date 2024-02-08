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

  protected TestBall _thisCreature;

  public AbstractMonsterTask(
    TestBall ball,
    NavCoordinator nav,
    PlayerCharacter player
  ) : base()
  {
    _thisCreature = ball;
    _nav = nav;
    _creatureToTrack = player;
  }
}
