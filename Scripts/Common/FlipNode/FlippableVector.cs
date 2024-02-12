using Godot;

namespace Centari.Common;

public class FlippableNode : AbstractFlipNode<Node2D>
{
  private Vector2 _initialPos;

  public FlippableNode(Node2D node)
  : base(node)
  {
    _item = node;
    _initialPos = node.Position;
  }

  public override void HandleItemFlipped()
  {
    _item.Position = new Vector2(
      -_initialPos.X,
      _initialPos.Y
    );
  }

  public override void HandleItemNotFlipped()
  {
    _item.Position = new Vector2(
      _initialPos.X,
      _initialPos.Y
    );
  }
}
