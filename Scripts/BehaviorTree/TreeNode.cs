using System.Collections.Generic;

namespace Centari.BehaviorTree;

public abstract class TreeNode : INode
{
  public abstract NodeState Evaluate(double delta);

  protected List<INode> _children = new();

  public TreeNode()
  { }

  public TreeNode(List<INode> children)
  {
    _children = children;
  }
}
