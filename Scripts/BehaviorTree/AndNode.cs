using System.Collections.Generic;

namespace Centari.BehaviorTree;

public class AndNode : TreeNode
{
  public AndNode(List<INode> children) : base(children) { }

  public override NodeState Evaluate(double delta)
  {
    bool anyChildRunning = false;

    foreach (TreeNode node in _children)
    {
      switch (node.Evaluate(delta))
      {
        case NodeState.FAILURE:
          return NodeState.FAILURE;
        case NodeState.RUNNING:
          anyChildRunning = true;
          break;
        case NodeState.SUCCESS:
          continue;
        default:
          continue;
      }
    }

    return anyChildRunning
      ? NodeState.RUNNING
      : NodeState.SUCCESS;
  }
}
