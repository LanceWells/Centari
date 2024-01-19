using System;
using Godot;

/// <summary>
/// A state used to represent a player being idle, with no input.
/// </summary>
public partial class IdleState : AbstractPlayerState
{
  /// <inheritdoc/>
  public override void Transition(
  StateMachine stateMachine,
  AnimationPlayer animationPlayer,
  Node owner
  )
  {
    base.Transition(stateMachine, animationPlayer, owner);
    animationPlayer.Play("Idle");
  }

  /// <inheritdoc/>
  public override void Detransition()
  {
    _animationPlayer.Stop();
  }

  /// <inheritdoc/>
  public override void PhysicsProcess(double delta)
  {
    Vector2 inputDir = _getMovementInput(delta);
    Vector2 gravityDir = _getGravity(delta);
    Vector2 jumpDir = _getJumpInput(delta);

    Vector2 velocity = _player.Velocity;
    velocity.X = inputDir.X;
    velocity.Y += gravityDir.Y;
    velocity.Y += jumpDir.Y;

    _player.Velocity = velocity;

    if (_player.IsOnFloor())
    {
      _player.Velocity += jumpDir;
    }

    _player.MoveAndSlide();

    _handleFireProjectile();

    if (inputDir != Vector2.Zero)
    {
      _stateMachine.TransitionState("WalkingState");
    }

    Console.WriteLine(_player.Velocity);
  }
}
