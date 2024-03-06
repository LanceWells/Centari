using System;
using Godot;

public abstract partial class AbstractProjectile : Area2D, IProjectile
{
  public bool Friendly { get; set; } = true;

  public abstract float Damage { get; set; }

  public abstract float Speed { get; set; }

  public Vector2 Velocity { get; set; } = Vector2.Right;

  protected float? BendRotation = null;

  [Signal]
  public delegate void CollideEventHandler(
    // KinematicCollision2D collision,
    Node2D collidingArea,
    AbstractProjectile projectile
  );

  // public override void _Process(double delta)
  // {
  //   base._Process(delta);
  // }

  public override void _Ready()
  {
    BodyEntered += (Node2D body) =>
    {
      EmitSignal(SignalName.Collide, body, this);
    };
  }

  public override void _PhysicsProcess(double delta)
  {
    Position += Velocity.Rotated(Rotation);

    // KinematicCollision2D collision = MoveAndCollide(Velocity);
    // KinematicCollision2D collision = MoveAndCollide(Velocity);

    // if (collision != null)
    // {
    //   EmitSignal(SignalName.Collide, collision, this);
    // }
  }
}
