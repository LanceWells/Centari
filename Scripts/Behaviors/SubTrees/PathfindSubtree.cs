using System.Collections.Generic;
using Centari.Behaviors.Common;

namespace Centari.Behaviors;

public class PathfindSubTree : INode<INavContext>
{
  INavContext _ctx;

  INode<INavContext> _root;

  public void Init(ref INavContext contextRef)
  {
    _ctx = contextRef;
    _root.Init(ref contextRef);
  }

  public PathfindSubTree()
  {
    _root = new ReactiveFallbackNode
  }

  public NodeState Process(double delta)
  {
    throw new System.NotImplementedException();
  }
}