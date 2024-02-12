using Centari.Common;
using Godot;

namespace Centari.Player;

public class FlippableRayCast : AbstractFlipNode<RayCast2D>
{
  private Vector2 _initialTarget;

  public FlippableRayCast(RayCast2D ray)
  : base(ray)
  {
    _initialTarget = ray.TargetPosition;
    _item = ray;
  }

  public override void HandleItemFlipped()
  {
    _item.TargetPosition = new Vector2(
      -_initialTarget.X,
      _initialTarget.Y
    );
  }

  public override void HandleItemNotFlipped()
  {
    _item.TargetPosition = new Vector2(
      _initialTarget.X,
      _initialTarget.Y
    );
  }
}
