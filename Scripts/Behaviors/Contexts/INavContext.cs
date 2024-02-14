using System.Collections.Generic;
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
    set;
    get;
  }

  public List<Node2D> PossibleTargets
  {
    get;
  }

  public NavModes[] NavModes
  {
    get;
  }
}
