using Centari.Common;
using Godot;

namespace Centari.Player;

/// <summary>
/// Contains a raycast in a flippable container.
/// </summary>
public class FlippableRayCast : AbstractFlipNode<RayCast2D>, IFlipNode<RayCast2D>
{
  private Vector2 _initialTarget;

  /// <summary>
  /// Creates a new instance of a <see cref="FlippableRayCast"/>.
  /// </summary>
  /// <param name="ray">The ray to contain.</param>
  public FlippableRayCast(RayCast2D ray)
  : base(ray)
  {
    _initialTarget = ray.TargetPosition;
    _item = ray;
  }

  protected override void HandleItemFlipped()
  {
    _item.TargetPosition = new Vector2(
      -_initialTarget.X,
      _initialTarget.Y
    );
  }

  protected override void HandleItemNotFlipped()
  {
    _item.TargetPosition = new Vector2(
      _initialTarget.X,
      _initialTarget.Y
    );
  }
}
