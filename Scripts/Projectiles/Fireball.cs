using System;
using Godot;

public partial class Fireball : AbstractProjectile
{
  public override float Damage { get; set; } = 1.0f;

  public override float Speed { get; set; } = 10f;

  /// <inheritdoc/>
  public override void _Ready()
  {
    base._Ready();
  }
}
