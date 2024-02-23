using Centari.State;
using Godot;

namespace Centari.Player.States;

/// <summary>
/// A state used to represent a player being idle, with no input.
/// </summary>
public partial class IdleState : AbstractPlayerState
{
  protected override StateCapabilities Capabilities => new()
  {
    CanWalk = true,
    CanJump = true,
    CanAttack = true,
    GravityAffected = false,
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
    animationPlayer.Play("Idle");
  }

  /// <inheritdoc/>
  public override void Detransition(string nextState)
  {
    base.Detransition(nextState);

    _animationPlayer.Stop();
  }

  /// <inheritdoc/>
  public override void PhysicsProcess(double delta)
  {
    bool pressJump = _inputQueue.Peek(PlayerInput.Jump);
    bool pressLeft = _inputQueue.Peek(PlayerInput.MoveLeft);
    bool pressRight = _inputQueue.Peek(PlayerInput.MoveRight);

    if (pressJump)
    {
      _stateMachine.TransitionState("JumpingState");
    }
    else if (!_player.IsOnFloor())
    {
      _stateMachine.TransitionState("FallingState");
    }
    else if (pressLeft || pressRight)
    {
      _stateMachine.TransitionState("WalkingState");
    }

    base.PhysicsProcess(delta);
  }
}
