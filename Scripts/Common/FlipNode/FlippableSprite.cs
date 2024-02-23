using Godot;

namespace Centari.Common;

/// <summary>
/// Contains a sprite in a flippable container.
/// </summary>
public class FlippableSprite<T> : AbstractFlipNode<T>
where T : Node2D
{
  private Vector2 _initialScale;

  /// <summary>
  /// Creats a new instance of a <see cref="FlippableNode"/>.
  /// </summary>
  /// <param name="node"></param>
  public FlippableSprite(T node)
  : base(node)
  {
    _item = node;
    _initialScale = node.Scale;
  }

  protected override void HandleItemFlipped()
  {
    _item.Scale = new(
      -_initialScale.X,
      _initialScale.Y
    );
  }

  protected override void HandleItemNotFlipped()
  {
    _item.Scale = new(
      _initialScale.X,
      _initialScale.Y
    );
  }
}
