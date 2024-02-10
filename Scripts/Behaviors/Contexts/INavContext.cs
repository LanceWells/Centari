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
    set;
  }

  public AbstractMonster ThisMonster
  {
    get;
    set;
  }

  public Node2D TrackedCreature
  {
    get;
    set;
  }

  public NavModes[] NavModes
  {
    get;
  }
}
