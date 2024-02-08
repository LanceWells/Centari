using System.Collections.Generic;

namespace Centari.BehaviorTree;

public class OrNode<T> : TreeNode<T>
where T : INode
{
  public OrNode(List<T> children) : base(children) { }

  public override NodeState Evaluate(double delta)
  {
    foreach (T node in _children)
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
