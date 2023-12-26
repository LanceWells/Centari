using Godot;

public partial class Firebolt : Area2D, IProjectile
{
  public float Damage => 1.0f;

  public bool Friendly => true;

  public Vector2 Velocity { get; set; } = Vector2.Right;

  public override void _PhysicsProcess(double delta)
  {
    Position += Velocity * (float)delta * 1000;
  }
}
