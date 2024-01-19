using System;
using Godot;

/// <summary>
/// This is the standard movement state. A player should be able to perform most actions while
/// moving intentionally like this.
/// </summary>
public partial class WalkingState : AbstractPlayerState
{
  /// <inheritdoc/>
  public override void Transition(
  StateMachine stateMachine,
  AnimationPlayer animationPlayer,
  Node owner
  )
  {
    base.Transition(stateMachine, animationPlayer, owner);
    animationPlayer.Play("Run");
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


    bool shouldFlip = _shouldFlip();
    _player.HandleFlip(shouldFlip);
    _player.MoveAndSlide();

    _handleFireProjectile();

    if (inputDir == Vector2.Zero)
    {
      _stateMachine.TransitionState("IdleState");
    }

    Console.WriteLine(_player.Velocity);
  }
}
