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
  };

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
    else if (p.MoveLeft || p.MoveRight)
    {
      _stateMachine.TransitionState("WalkingState");
    }
  }
}
