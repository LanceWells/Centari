using System.Collections.Generic;
using Centari.Behaviors.Common;
using Centari.Behaviors.Composites;
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
      new SequenceNode<INavContext>(new List<INode<INavContext>>{
        new AlwaysSucceedNode<INavContext>(
          new SequenceNode<INavContext>(new List<INode<INavContext>>{
            new TestOnGroundNode(),
            new SetNextPointNode(),
          })
        ),
        new SelectorNode<INavContext>(new List<INode<INavContext>>{
          new InverterNode<INavContext>(
            new WalkHorizontalNode()
          ),
          new SequenceNode<INavContext>(new List<INode<INavContext>>{
            new TestOnGroundNode(),
            new JumpNode(),
          })
        })
      })
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
  private Vector2 _nextPoint;

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

  public Vector2 NextPoint
  {
    get => _nextPoint;
    set => _nextPoint = value;
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
