using System.Collections.Generic;
using Centari.Behaviors.Common;
using Centari.Behaviors.Leaves.Navigation;

namespace Centari.Behaviors;

public class PathfindSubTree : INode<INavContext>
{
  private INavContext _ctx;

  private INode<INavContext> _root;

  public void Init(ref INavContext contextRef)
  {
    _ctx = contextRef;
    _root.Init(ref contextRef);
  }

  public PathfindSubTree(
    ref NodeTempo pathfindTempo,
    string pathfindTempoKey
  )
  {
    _root = new ReactiveFallbackNode<INavContext>("Fb_SubtreeRoot", new List<INode<INavContext>>() {
      new ReactiveSequenceNode<INavContext>("Follow_Sq", new List<INode<INavContext>>() {
        new IsKnownTargetNode(),
        new SetPathNode(ref pathfindTempo, pathfindTempoKey),
        new FullFallbackNode<INavContext>("Move_FFb", new List<INode<INavContext>>() {
          new ReactiveSequenceNode<INavContext>("Jump_Sq", new List<INode<INavContext>>() {
            new IsOnFloorNode(),
            new IsTargetAboveNode(),
            new JumpNode(),
          }),
          new ReactiveSequenceNode<INavContext>("Fall_Sq", new List<INode<INavContext>>() {
            new IsTargetBelowNode(),
            new FallNode(),
          }),
          new WalkNode(),
        })
      })
    });
  }

  public NodeState Process(double delta)
  {
    return _root.Process(delta);
  }
}