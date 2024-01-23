using System;
using Godot;

public abstract partial class AbstractProjectile : RigidBody2D, IProjectile
{
  public bool Friendly { get; set; } = true;

  public abstract float Damage { get; set; }

  public abstract float Speed { get; set; }

  public Vector2 Velocity { get; set; } = Vector2.Right;

  protected float? BendRotation = null;

  [Signal]
  public delegate void CollideEventHandler(
    KinematicCollision2D collision,
    AbstractProjectile projectile
  );

  public override void _PhysicsProcess(double delta)
  {
    Velocity = Velocity.Rotated(Rotation);

    KinematicCollision2D collision = MoveAndCollide(Velocity);

    if (collision != null)
    {
      EmitSignal(SignalName.Collide, collision, this);
    }
  }
}
