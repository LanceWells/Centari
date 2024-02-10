using Centari.State;
using Godot;

namespace Centari.Player.States;

public partial class FallingState : AbstractPlayerState
{
  protected override StateCapabilities Capabilities => new()
  {
    CanWalk = true,
    CanJump = false,
    CanAttack = true,
    GravityAffected = true,
  };

  /// <inheritdoc/>
  protected override Vector2 GetWalking(Vector2 direction, double delta)
  {
    PlayerInputs p = GetPlayerInputs();

    Vector2 walkDirection = Vector2.Zero;
    if (p.MoveLeft)
    {
      walkDirection.X -= _player.MaxSpeed;
    }
    if (p.MoveRight)
    {
      walkDirection.X += _player.MaxSpeed;
    }

    Vector2 lerpedDir = direction.Lerp(
      walkDirection,
      (float)delta * _player.Friction * 0.05f
    );

    return lerpedDir;
  }

  public override void Transition(StateMachine stateMachine, AnimationPlayer animationPlayer, Node owner)
  {
    base.Transition(stateMachine, animationPlayer, owner);
  }

  public override void Detransition()
  { }

  public override void PhysicsProcess(double delta)
  {
    base.PhysicsProcess(delta);

    if (_player.IsOnFloor())
    {
      _stateMachine.TransitionState("IdleState");
    }
  }
}
