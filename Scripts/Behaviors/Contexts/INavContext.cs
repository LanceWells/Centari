using Centari.Monsters;
using Centari.Navigation;
using Centari.Navigation.Rules;
using Godot;

namespace Centari.Behaviors.Contexts;

public interface INavContext
{
  public Vector2 NextPoint
  {
    get;
    set;
  }

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
