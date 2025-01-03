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
    Node owner,
    string previousState
  )
  {
    base.Transition(stateMachine, animationPlayer, owner, previousState);

    if (previousState == "IdleState")
    {
      animationPlayer.Play("IdleToRun");
      animationPlayer.Queue("Run");
    }
    else
    {
      animationPlayer.Play("Run");
    }
  }

  /// <inheritdoc/>
  public override void PhysicsProcess(double delta)
  {
    if (_inputQueue.Peek(PlayerInput.Jump))
    {
      _stateMachine.TransitionState("JumpingState");
    }
    else if (!_player.IsOnFloor())
    {
      _stateMachine.TransitionState("FallingState");
    }
    else if (!_inputQueue.Peek(PlayerInput.MoveLeft) && !_inputQueue.Peek(PlayerInput.MoveRight))
    {
      _stateMachine.TransitionState("IdleState");
    }

    base.PhysicsProcess(delta);
  }
}
