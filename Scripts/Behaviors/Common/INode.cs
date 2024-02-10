namespace Centari.Behaviors.Common;

public interface INode<T>
{
  public void Init(ref T contextRef);

  public NodeState Process(double delta);
}
