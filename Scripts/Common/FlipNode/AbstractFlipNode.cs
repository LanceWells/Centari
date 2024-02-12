namespace Centari.Common;

public abstract class AbstractFlipNode<T>
{
  protected T _item;

  private bool _isFlipped;

  public T Item => _item;

  public AbstractFlipNode(T item)
  {
    _item = item;
  }

  public bool IsFlipped => _isFlipped;

  public void SetFlipped(bool isFlipped)
  {
    _isFlipped = isFlipped;
    if (isFlipped)
    {
      HandleItemFlipped();
    }
    else
    {
      HandleItemNotFlipped();
    }
  }

  public abstract void HandleItemFlipped();

  public abstract void HandleItemNotFlipped();
}
