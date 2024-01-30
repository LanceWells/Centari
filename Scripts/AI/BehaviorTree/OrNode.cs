using System.Collections.Generic;

namespace Centari.BehaviorTree;

public class OrNode : TreeNode
{
  public OrNode(List<INode> children) : base(children) { }

  public override NodeState Evaluate(double delta)
  {
    foreach (INode node in _children)
    {
      switch (node.Evaluate(delta))
      {
        case NodeState.FAILURE:
          return NodeState.FAILURE;
        case NodeState.RUNNING:
          return NodeState.RUNNING;
        case NodeState.SUCCESS:
          continue;
        default:
          continue;
      }
    }

    return NodeState.SUCCESS;
  }
}
