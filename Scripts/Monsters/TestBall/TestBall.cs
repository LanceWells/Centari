using Centari.Navigation;
using Centari.Navigation.Rules;
using Centari.Player;
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

  public void Prepare(NavCoordinator nav, PlayerCharacter player)
  {
    _tree = new TestBallTree(this, nav, player);
    _tree.Start();
  }

  public override void _PhysicsProcess(double delta)
  {
    if (_tree != null)
    {
      _tree.Update(delta);
    }
    base._PhysicsProcess(delta);
  }
}
