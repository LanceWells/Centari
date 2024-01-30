using Centari.BehaviorTree;

namespace Centari.Monsters;

public class TaskIdle : TreeNode
{
  public override NodeState Evaluate(double delta)
  {
    return NodeState.SUCCESS;
  }
}
