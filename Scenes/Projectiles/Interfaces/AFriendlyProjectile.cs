using Godot;

public abstract partial class AbstractProjectile : Area2D, IProjectile
{
  public bool Friendly { get; set; } = true;

  public abstract float Damage { get; set; }

  public abstract float Speed { get; set; }

  public Vector2 Velocity { get; set; } = Vector2.Right;

  public override void _Ready()
  {
    AnimatedSprite2D animatedSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");

    if (animatedSprite != null)
    {
      animatedSprite.Play();
    }

    base._Ready();
  }

  public override void _PhysicsProcess(double delta)
  {
    Position += Velocity * (float)delta * Speed;
  }
}
