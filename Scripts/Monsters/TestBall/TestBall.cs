using Centari.Behaviors.ExampleTrees;
using Centari.Navigation;
using Centari.Player;
using Godot;

namespace Centari.Monsters;

public partial class TestBall : AbstractMonster
{
  private ExampleFollowTree _tree;

  private Rect2 _hitbox;

  public override Rect2 HitBox
  {
    get => _hitbox;
  }

  public override void _Ready()
  {
    base._Ready();
    var _collisionShape = GetNode<CollisionShape2D>("HitBox");

    _hitbox = CalcBoundingBox(_collisionShape);
  }

  public void Prepare(NavCoordinator nav, PlayerCharacter player)
  {
    _tree = new ExampleFollowTree(nav, this, player);
  }

  public override void _Process(double delta)
  {
    _tree?.Process(delta);
    base._Process(delta);
  }
}
