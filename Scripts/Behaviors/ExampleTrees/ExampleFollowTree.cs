using Centari.Behaviors.Common;
using Centari.Behaviors.Contexts;
using Centari.Behaviors.Decorators;
using Centari.Behaviors.Leaves;
using Centari.Monsters;
using Centari.Navigation;
using Centari.Navigation.Rules;
using Godot;

namespace Centari.Behaviors.ExampleTrees;

public class ExampleFollowTree
{
  INavContext _ctx;

  INode<INavContext> _root;

  public ExampleFollowTree(
    NavCoordinator nav,
    AbstractMonster thisMonster,
    Node2D trackedCreature
  )
  {
    _ctx = new ExampleFollowTreeContext(
      nav,
      thisMonster,
      trackedCreature
    );

    _root = new RepeatUntilFailNode<INavContext>(
      new PathfindNode()
    );

    _root.Init(ref _ctx);
  }

  public void Process(double delta)
  {
    _root.Process(delta);
  }
}

public class ExampleFollowTreeContext : INavContext
{
  private NavCoordinator _nav;

  private AbstractMonster _thisMonster;

  private Node2D _trackedCreature;

  private NavModes[] _navModes;

  public ExampleFollowTreeContext(
    NavCoordinator nav,
    AbstractMonster thisMonster,
    Node2D trackedCreature
  )
  {
    _nav = nav;
    _thisMonster = thisMonster;
    _trackedCreature = trackedCreature;
    _navModes = new NavModes[] { Navigation.Rules.NavModes.SNAIL };
  }

  public NavCoordinator Nav
  {
    get => _nav;
  }

  public AbstractMonster ThisMonster
  {
    get => _thisMonster;
  }

  public Node2D TrackedCreature
  {
    get => _trackedCreature;
  }

  public NavModes[] NavModes
  {
    get => _navModes;
  }
}
