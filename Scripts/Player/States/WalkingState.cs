using Centari.State;
using Godot;

namespace Centari.Player.States;

/// <summary>
/// This is the standard movement state. A player should be able to perform most actions while
/// moving intentionally like this.
/// </summary>
public partial class WalkingState : AbstractPlayerState
{
  protected override StateCapabilities Capabilities => new()
  {
    CanWalk = true,
    CanJump = true,
    CanAttack = true,
    GravityAffected = true,
    CanFlip = true,
  };

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
    base.PhysicsProcess(delta);

    PlayerInputs p = GetPlayerInputs();

    if (p.Jump)
    {
      _stateMachine.TransitionState("JumpingState");
    }
    else if (!_player.IsOnFloor())
    {
      _stateMachine.TransitionState("FallingState");
    }
    else if (!p.MoveLeft && !p.MoveRight)
    {
      _stateMachine.TransitionState("IdleState");
    }
  }
}
