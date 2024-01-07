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
    Vector2 direction = _handleMovement(delta);

    _handleFireProjectile();

    if (direction.Length() > 0.0f)
    {
      _stateMachine.TransitionState("WalkingState");
    }
    else
    {
      _player.Velocity = Vector2.Zero;
    }
  }
}