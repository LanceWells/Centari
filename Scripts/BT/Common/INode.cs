namespace Centari.BT;

public interface INode
{
  public void Reset();

  public void Cancel();

  public NodeState Process(double delta);
}
