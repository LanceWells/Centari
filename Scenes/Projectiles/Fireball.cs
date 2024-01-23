using System;
using Godot;

public partial class Fireball : AbstractProjectile
{
  [Export]
  public override float Damage { get; set; } = 1.0f;

  [Export]
  public override float Speed { get; set; } = 10f;

  private CollisionShape2D bbox;

  // private Timer bulletBendTimer;

  /// <inheritdoc/>
  public override void _Ready()
  {
    // bulletBendTimer = GetNode<Timer>("BulletBendTimer");
    // bulletBendTimer.Timeout += _onBulletBendTimerTimeout;

    bbox = GetNode<CollisionShape2D>("CollsionShape2D");

    base._Ready();
  }

  private void _onBulletBendTimerTimeout()
  {
    //   Vector2 mouse = GetViewport().GetMousePosition();
    //   float newAngleToMouse = Position.AngleTo(mouse);
    //   float angleDiff = newAngleToMouse - Rotation;

    //   float negative = angleDiff < 0 ? -1 : 1;

    //   Rotation = Math.Min(Math.Abs(angleDiff) * 0.02f, 0.04f) * negative;

  }
}
