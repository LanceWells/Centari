using Godot;

namespace Centari.Projectiles;

public partial class Fireball : AbstractProjectile
{
  private Color _magicColor = new();

  public override float Damage { get; set; } = 1.0f;

  public override float Speed { get; set; } = 15f;

  public override Color MagicColor
  {
    get => _magicColor;
    set => _magicColor = value;
  }

  /// <inheritdoc/>
  public override void _Ready()
  {
    base._Ready();
  }
}
