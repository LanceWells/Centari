using Centari.State;
using Godot;

namespace Centari.Player;

/// <summary>
/// This is the standard movement state. A player should be able to perform most actions while
/// moving intentionally like this.
/// </summary>
public partial class WalkingState : AbstractPlayerState
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
    else if (!p.MoveLeft && !p.MoveRight)
    {
      _stateMachine.TransitionState("IdleState");
    }
  }
}
