using System;
using Godot;

public abstract partial class AbstractProjectile : RigidBody2D, IProjectile
{
  public bool Friendly { get; set; } = true;

  public abstract float Damage { get; set; }

  public abstract float Speed { get; set; }

  public Vector2 Velocity { get; set; } = Vector2.Right;

  protected float? BendRotation = null;

  // public override void _Ready()
  // {
  //   Velocity = new Vector2(Speed, 0);
  //   base._Ready();
  // }

  public override void _PhysicsProcess(double delta)
  {
    if (BendRotation != null)
    {
      // float rotationDelta = BendRotation.Value * (float)delta;
      // float normalRotation = rotationDelta % (float)Math.PI * 2;
      // Rotation += normalRotation;

      // Rotation += BendRotation.Value * (float)delta;
      // Rotate(0.01f);
      // Velocity = Velocity.Rotated(Rotation);
    }

    Velocity = Velocity.Rotated(Rotation);

    MoveAndCollide(Velocity);
  }
}
