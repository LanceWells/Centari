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
  public override void Process(double delta)
  { }

  /// <inheritdoc/>
  public override void PhysicsProcess(double delta)
  {
	Vector2 direction = _handleMovement(delta);

	if (direction.Length() > 0.0f)
	{
	  _stateMachine.TransitionState("WalkingState");
	}
	else
	{
	  _player.Velocity = _player.Velocity.Lerp(Vector2.Zero, _player.Friction);
	}
  }
}
