using Godot;

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
    GD.Print(animationPlayer.GetAnimationList().Join(", "));
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

    if (direction == Vector2.Zero)
    {
      _stateMachine.TransitionState("IdleState");
    }
    else
    {
      _player.Velocity = _player.Velocity.Lerp(direction, _player.Acceleration);
    }

    _player._sprite.FlipH = _shouldFlip();
  }
}
