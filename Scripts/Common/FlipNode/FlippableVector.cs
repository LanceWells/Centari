using Godot;

namespace Centari.Common;

/// <summary>
/// Contains a vector in a flippable container.
/// </summary>
public class FlippableNode : AbstractFlipNode<Node2D>
{
  private Vector2 _initialPos;

  /// <summary>
  /// Creats a new instance of a <see cref="FlippableNode"/>.
  /// </summary>
  /// <param name="node"></param>
  public FlippableNode(Node2D node)
  : base(node)
  {
    _item = node;
    _initialPos = node.Position;
  }

  protected override void HandleItemFlipped()
  {
    _item.Position = new Vector2(
      -_initialPos.X,
      _initialPos.Y
    );
  }

  protected override void HandleItemNotFlipped()
  {
    _item.Position = new Vector2(
      _initialPos.X,
      _initialPos.Y
    );
  }
}
