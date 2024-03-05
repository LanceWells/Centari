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
    _root = new SequenceNode<INavContext>("Follow_Sq", new List<INode<INavContext>>() {
      new IsKnownTargetNode(),
      new SetPathNode(ref pathfindTempo, pathfindTempoKey),
      new FallbackNode<INavContext>("Move_FFb", new List<INode<INavContext>>() {
        new ReactiveSequenceNode<INavContext>("Jump_Sq", new List<INode<INavContext>>() {
          new IsNextPointAboveNode(),
          new JumpNode(),
        }),
        new ReactiveSequenceNode<INavContext>("Fall_Sq", new List<INode<INavContext>>() {
          new IsNextPointBelowNode(),
          new FallNode(),
        }),
        new WalkNode(),
      })
    });
  }

  public NodeState Process(double delta)
  {
    return _root.Process(delta);
  }
}