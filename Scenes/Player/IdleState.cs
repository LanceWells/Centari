using Godot;

/// <summary>
/// A state used to represent a player being idle, with no input.
/// </summary>
public partial class IdleState : AbstractPlayerState
{
  /// <inheritdoc/>
  protected override bool CanWalk => true;

  /// <inheritdoc/>
  protected override bool GravityAffected => true;

  /// <inheritdoc/>
  protected override bool CanAttack => true;

  /// <inheritdoc/>
  protected override bool CanFlip => true;

  protected override bool CanJump => true;

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
    PlayerInputs p = GetPlayerInputs();
    Vector2 inputDir = CalculateDirection(delta);
    _player.Velocity = inputDir;

    _player.HandleFlip(_shouldFlip());
    _player.MoveAndSlide();
    _handleFireProjectile();

    if (!_player.IsOnFloor())
    {
      _stateMachine.TransitionState("MidairState");
    }
    // if (p.Jump)
    // {
    //   _stateMachine.TransitionState("MidairState");
    // }
    else if (p.MoveLeft || p.MoveRight)
    {
      _stateMachine.TransitionState("WalkingState");
    }
  }
}
