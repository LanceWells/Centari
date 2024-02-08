using System.Collections.Generic;

namespace Centari.BehaviorTree;

public abstract class TreeNode<T> : INode
where T : INode
{
  public abstract NodeState Evaluate(double delta);

  protected List<T> _children = new();

  public TreeNode()
  { }

  public TreeNode(List<T> children)
  {
    _children = children;
  }
}
