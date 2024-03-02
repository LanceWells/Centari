using System;
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

  private NodeTempo _pathfindTempo;

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

    NodeTempo pathfindTempo = new(50, 50);
    string pathfindTempoKey = $"TestBall_{thisMonster.GetRid().Id}";
    pathfindTempo.Register(pathfindTempoKey);

    _pathfindTempo = pathfindTempo;

    /*
     * Some notes here:
        - The critter keeps getting stuck on corners when jumping or falling. This could probably
          be mitigated by raycasting vertically when moving up/down in order to find which side
          is hung up on an edge.

        - Some of these operations are running every frame, which is nuts. It would be nice if there
          was not only a throttle, but a manager for tasks that enables them round-robin style. For
          example, if there was a pathfind round-robin manager, it could cache the response for most
          pathfinding each trip, only updating the "next" entry when it's time to update that
          pathfinding. 
     */

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
              new PathfindTargetNodeV2(ref pathfindTempo, pathfindTempoKey),
            })
          }),
          new PathfindTargetNodeV2(ref pathfindTempo, pathfindTempoKey),
        })
      }),
      new KnowVisibleTargetNode(),
      new IdleNode()
    });

    _root.Init(ref _ctx);
  }

  public void Process(double delta)
  {
    _pathfindTempo.Update(delta);
    _root.Process(delta);
  }
}

public class ExampleFollowTreeContext : INavContext
{
  private NavCoordinator _nav;

  private AbstractMonster _thisMonster;

  private Node2D _trackedCreature;

  private List<Node2D> _possibleTargets = new();

  private Vector2[] _path = Array.Empty<Vector2>();

  private Vector2 _lastKnownTargetPoint = new();

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

  public Vector2[] Path
  {
    get => _path;
    set => _path = value;
  }

  public Vector2 LastKnownTargetPoint
  {
    get => _lastKnownTargetPoint;
    set => _lastKnownTargetPoint = value;
  }
}
