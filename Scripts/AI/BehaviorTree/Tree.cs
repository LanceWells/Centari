namespace Centari.BehaviorTree;

public abstract class Tree
{
  private INode _root = null;

  public void Start()
  {
    _root = SetupTree();
  }

  public void Update(double delta)
  {
    if (_root != null)
    {
      _root.Evaluate(delta);
    }
  }

  protected abstract INode SetupTree();
}
