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
    Vector2 direction = _handleMovement(delta);

    _handleFireProjectile();

    if (direction == Vector2.Zero)
    {
      _stateMachine.TransitionState("IdleState");
    }
    else
    {
      _player.Velocity = direction * _player.MaxSpeed;
    }

    bool shouldFlip = _shouldFlip();
    _player.HandleFlip(shouldFlip);
  }
}
