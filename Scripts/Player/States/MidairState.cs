using System;
using Centari.State;
using Godot;

namespace Centari.Player;

public partial class MidairState : AbstractPlayerState
{
  /// <inheritdoc/>
  protected override bool CanWalk => true;

  /// <inheritdoc/>
  protected override bool GravityAffected => true;

  /// <inheritdoc/>
  protected override bool CanAttack => true;

  /// <inheritdoc/>
  protected override bool CanFlip => true;

  protected override bool CanJump => false;

  public void OnBodyEntered(Node2D body)
  {
    PlayerInputs p = GetPlayerInputs();

    Vector2 mantleBoost = Vector2.Zero;
    if (p.MoveLeft && _player.Velocity.X < 0 && Math.Abs(_player.Velocity.Y) < 500)
    {
      mantleBoost.Y -= _player.MaxSpeed * 0.6f;
      mantleBoost.X -= _player.MaxSpeed * 0.6f;
    }

    if (p.MoveRight && _player.Velocity.X > 0 && Math.Abs(_player.Velocity.Y) < 500)
    {
      mantleBoost.Y -= _player.MaxSpeed * 0.6f;
      mantleBoost.X += _player.MaxSpeed * 0.6f;
    }

    // If the mantle activates, do a lil mantle animation.

    _player.Velocity = mantleBoost;
  }

  /// <inheritdoc/>
  public override void Transition(StateMachine stateMachine, AnimationPlayer animationPlayer, Node owner)
  {
    base.Transition(stateMachine, animationPlayer, owner);
    _player.MantleArea.BodyEntered += OnBodyEntered;
  }

  public override void Detransition()
  {
    base.Detransition();
    _player.MantleArea.BodyEntered -= OnBodyEntered;
  }

  /// <inheritdoc/>
  protected override Vector2 GetWalking(Vector2 direction, double delta)
  {
    PlayerInputs p = GetPlayerInputs();

    Vector2 walkDirection = Vector2.Zero;
    if (p.MoveLeft)
    {
      walkDirection.X -= _player.MaxSpeed;
    }
    if (p.MoveRight)
    {
      walkDirection.X += _player.MaxSpeed;
    }

    Vector2 lerpedDir = direction.Lerp(
      walkDirection,
      (float)delta * _player.Friction * 0.05f
    );

    return lerpedDir;
  }

  /// <inheritdoc/>
  public override void PhysicsProcess(double delta)
  {
    Vector2 inputDir = CalculateDirection(delta);
    _player.Velocity = inputDir;

    _player.MoveAndSlide();
    _handleFireProjectile();

    if (_player.IsOnFloor())
    {
      _stateMachine.TransitionState("IdleState");
    }
  }
}
