using Godot;

public partial class Fireball : AbstractProjectile
{
  [Export]
  public override float Damage { get; set; } = 1.0f;

  [Export]
  public override float Speed { get; set; } = 1000f;
}
