using System;
using Godot;

/// <summary>
/// A state used to represent a player being idle, with no input.
/// </summary>
public partial class IdleState : AbstractPlayerState
{
  protected override bool CanWalk => true;
  protected override bool CanJump => true;
  protected override bool GravityAffected => true;

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
    Vector2 inputDir = CalculateDirection(delta);
    _player.Velocity = inputDir;

    _player.HandleFlip(_shouldFlip());
    _player.MoveAndSlide();

    _handleFireProjectile();

    if (Input.IsActionPressed("jump"))
    {
      _stateMachine.TransitionState("MidairState");
    }
    else if (Input.IsActionPressed("move_left") || Input.IsActionPressed("move_right"))
    {
      _stateMachine.TransitionState("WalkingState");
    }

    Console.WriteLine(_player.Velocity);
  }
}
