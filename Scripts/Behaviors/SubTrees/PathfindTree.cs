using System.Collections.Generic;
using Centari.Behaviors.Common;
using Centari.Behaviors.Leaves.Navigation;

namespace Centari.Behaviors;

public class PathfindTree : INode<INavContext>
{
  private INode<INavContext> _root;

  private INavContext _contextRef;

  public void Init(ref INavContext contextRef)
  {
    _contextRef = contextRef;
    _root.Init(ref _contextRef);
  }

  public PathfindTree(ref NodeTempo pathfindTempo, string pathfindTempoKey)
  {
    _root = new ReactiveSequenceNode<INavContext>("PathfindRootSequence", new List<INode<INavContext>>() {
      new IsKnownTargetNode(),
      new AlwaysSucceedNode<INavContext>(
        new ReactiveSequenceNode<INavContext>("PathfindIfOnGround", new List<INode<INavContext>>() {
          new IsOnFloorNode(),
          new TempoNode<INavContext>(
            new SetPathNode(),
            ref pathfindTempo,
            pathfindTempoKey
          )
        })
      ),
      new ReactiveFallbackNode<INavContext>("MoveFallback", new List<INode<INavContext>>() {
        new ReactiveSequenceNode<INavContext>("JumpMove", new List<INode<INavContext>>() {
          new IsOnFloorNode(),
          new IsNextPointAboveNode(),
          new AlignVerticalNode(),
          new JumpNode(),
        }),
        new ReactiveSequenceNode<INavContext>("FallMove", new List<INode<INavContext>>() {
          new IsOnFloorNode(),
          new IsNextPointBelowNode(),
          new AlignVerticalNode(),
          new MoveHorizontalNode(),
        }),
        new ReactiveSequenceNode<INavContext>("AirMove", new List<INode<INavContext>>() {
          new InverterNode<INavContext>(
            new IsOnFloorNode()
          ),
          new IsNextPointBelowNode(),
          new MoveHorizontalNode(),
        }),
        new ReactiveSequenceNode<INavContext>("LandMove", new List<INode<INavContext>>() {
          new IsOnFloorNode(),
          new MoveHorizontalNode(),
        })
      })
    });
  }

  public NodeState Process(double delta)
  {
    return _root.Process(delta);
  }
}
