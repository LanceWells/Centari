namespace Centari.Common;

/// <summary>
/// An abstract implementation of a <see cref="IFlipNode"/>.
/// </summary>
/// <typeparam name="T">The type of node to flip.</typeparam>
public abstract class AbstractFlipNode<T> : IFlipNode<T>
{
  protected T _item;

  private bool _isFlipped;

  /// <inheritdoc/>
  public T Item => _item;

  /// <inheritdoc/>
  public bool IsFlipped => _isFlipped;

  /// <summary>
  /// Creates a new instance of an <see cref="AbstractFlipNode"/>.
  /// </summary>
  /// <param name="item">The item to contain.</param>
  public AbstractFlipNode(T item)
  {
    _item = item;
  }

  /// <inheritdoc/>
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

  /// <summary>
  /// Implemented by the subtype. This is used to add behavior to <see cref="SetFlipped"/>.
  /// </summary>
  protected abstract void HandleItemFlipped();

  /// <summary>
  /// Implemented by the subtype. This is used to add behavior to <see cref="SetFlipped"/>.
  /// </summary>
  protected abstract void HandleItemNotFlipped();
}
