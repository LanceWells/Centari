using Centari.Behaviors.ExampleTrees;
using Centari.Navigation;
using Centari.Player;

namespace Centari.Monsters;

public partial class TestBall : AbstractMonster
{
  private ExampleFollowTree _tree;

  public void Prepare(NavCoordinator nav, PlayerCharacter player)
  {
    _tree = new ExampleFollowTree(nav, this, player);
  }

  public override void _PhysicsProcess(double delta)
  {
    _tree?.Process(delta);
    base._PhysicsProcess(delta);
  }
}
