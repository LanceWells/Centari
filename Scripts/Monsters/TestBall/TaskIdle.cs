using Centari.BehaviorTree;

namespace Centari.Monsters;

public class TaskIdle<T> : TreeNode<T>
where T : INode
{
  public override NodeState Evaluate(double delta)
  {
    return NodeState.SUCCESS;
  }
}
