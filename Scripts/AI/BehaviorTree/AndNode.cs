using System.Collections.Generic;

namespace Centari.BehaviorTree;

public class AndNode<T> : TreeNode<T>
where T : INode
{
  public AndNode(List<T> children) : base(children) { }

  public override NodeState Evaluate(double delta)
  {
    bool anyChildRunning = false;

    foreach (T node in _children)
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
