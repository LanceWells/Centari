using System;
using Godot;

public partial class Fireball : AbstractProjectile
{
  [Export]
  public override float Damage { get; set; } = 1.0f;

  [Export]
  public override float Speed { get; set; } = 10f;

  private Timer bulletBendTimer;

  /// <inheritdoc/>
  public override void _Ready()
  {
    bulletBendTimer = GetNode<Timer>("BulletBendTimer");
    bulletBendTimer.Timeout += _onBulletBendTimerTimeout;

    base._Ready();
  }

  private void _onBulletBendTimerTimeout()
  {
    Vector2 mouse = GetViewport().GetMousePosition();
    float newAngleToMouse = Position.AngleTo(mouse);
    float angleDiff = newAngleToMouse - Rotation;

    float negative = angleDiff < 0 ? -1 : 1;

    Rotation = Math.Min(Math.Abs(angleDiff) * 0.02f, 0.04f) * negative;

    // BendRotation = Math.Max(angleDiff * 0.02f, 0.0005f) * negative;

    // float maxRotation = Math.Max(Math.Abs(Rotation - newAngleToMouse), 0.15f);
    // float midRotation = (Rotation + maxRotation) / 2;

    // BendRotation = midRotation;

    // This works for an initial bend, don't delete it yet.
    // Vector2 mouse = GetViewport().GetMousePosition();
    // Velocity = Velocity.Lerp(mouse, 0.1f).Normalized() * Speed;
  }
}
