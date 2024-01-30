namespace Centari.BehaviorTree;

public interface INode
{
  public NodeState Evaluate(double delta);
}
