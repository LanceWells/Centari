using System.Collections.Generic;
using Centari.Behaviors.Common;
using Centari.Behaviors.Leaves.Navigation;
using Centari.Monsters;
using Centari.Navigation;
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

    _root = new ReactiveFallbackNode<INavContext>("RF_Root", new List<INode<INavContext>>() {
      new ReactiveSequenceNode<INavContext>("RS_KnownTarget", new List<INode<INavContext>>() {
        new IsKnownTargetNode(),
        new ReactiveFallbackNode<INavContext>("RF_TargetVisible", new List<INode<INavContext>>() {
          new ReactiveSequenceNode<INavContext>("RS_TargetVisible", new List<INode<INavContext>>() {
            new IsTargetVisibleNode(),
            new ReactiveFallbackNode<INavContext>("RF_Attack", new List<INode<INavContext>>() {
              new ReactiveSequenceNode<INavContext>("RS_Melee", new List<INode<INavContext>>() {
                new IsTargetInMeleeNode(),
                new AttackMeleeNode(),
              }),
              new ReactiveSequenceNode<INavContext>("RS_Range", new List<INode<INavContext>>() {
                new IsTargetInRangeNode(),
                new AttackRangedNode(),
              }),
              new PathfindTargetNode(),
            })
          }),
          new PathfindTargetNode(),
        })
      }),
      new KnowVisibleTargetNode(),
      new IdleNode()
    });

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

  private List<Node2D> _possibleTargets = new();

  public ExampleFollowTreeContext(
    NavCoordinator nav,
    AbstractMonster thisMonster,
    Node2D player
  )
  {
    _nav = nav;
    _thisMonster = thisMonster;
    _possibleTargets.Add(player);
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
    set => _trackedCreature = value;
  }

  public List<Node2D> PossibleTargets
  {
    get => _possibleTargets;
  }
}
