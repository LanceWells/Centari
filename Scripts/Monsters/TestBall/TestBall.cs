using Centari.Navigation;
using Centari.Navigation.Rules;
using Godot;

namespace Centari.Monsters;

public partial class TestBall : AbstractMonster
{
  private TestBallTree _tree;

  public static NavModes[] Nav
  {
    get
    {
      NavModes[] modes = new NavModes[1];
      modes[0] = NavModes.SNAIL;
      return modes;
    }
  }

  [Export]
  public float WalkSpeed = 50f;

  public void Prepare(NavCoordinator nav, Player player)
  {
    _tree = new TestBallTree(this, nav, player);
    _tree.Start();
  }

  public override void _PhysicsProcess(double delta)
  {
    _tree.Update(delta);
    base._PhysicsProcess(delta);
  }
}
