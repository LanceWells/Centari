using System;
using Centari.State;
using Godot;

namespace Centari.Player.States;

/// <summary>
/// This state should be entered when the player is mid-air. This may be entered when the player has
/// either walked off an edge, or is actively jumping.
/// </summary>
public partial class MidairState : AbstractPlayerState
{
  /// <inheritdoc/>
  protected override StateCapabilities Capabilities => new()
  {
    CanWalk = true,
    CanJump = false,
    CanAttack = true,
    GravityAffected = true,
  };

  /// <summary>
  /// This handler is called when the "mantle area" for a player collides witha body. This is used
  /// to determine if the player's foot is caught against a tile corner. If this happens, we push
  /// the player up and over the corner to help them move.
  /// </summary>
  /// <param name="body">The body that was collided with.</param>
  public void OnMantleAreaBodyEntered(Node2D body)
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

    // TODO: If the mantle activates, do a lil mantle animation.

    _player.Velocity = mantleBoost;
  }

  /// <inheritdoc/>
  public override void Transition(StateMachine stateMachine, AnimationPlayer animationPlayer, Node owner)
  {
    base.Transition(stateMachine, animationPlayer, owner);
    _player.MantleArea.BodyEntered += OnMantleAreaBodyEntered;
  }

  /// <inheritdoc/>
  public override void Detransition()
  {
    _player.MantleArea.BodyEntered -= OnMantleAreaBodyEntered;
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
