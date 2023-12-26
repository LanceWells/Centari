using Godot;

public interface IProjectile
{
  [Export]
  public float Damage { get; }

  [Export]
  public bool Friendly { get; }

  [Export]
  public Vector2 Velocity { get; }
}
