using Centari.Monsters;
using Centari.Navigation;
using Centari.Navigation.Rules;
using Godot;

namespace Centari.Behaviors;

public interface INavContext
{
  public NavCoordinator Nav
  {
    get;
  }

  public AbstractMonster ThisMonster
  {
    get;
  }

  public Node2D TrackedCreature
  {
    get;
  }

  public NavModes[] NavModes
  {
    get;
  }
}
