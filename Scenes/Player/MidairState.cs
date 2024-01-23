using Godot;

public partial class MidairState : AbstractPlayerState
{
  protected override bool CanWalk => true;

  protected override bool CanJump => false;

  protected override bool GravityAffected => true;

  public override void Transition(StateMachine stateMachine, AnimationPlayer animationPlayer, Node owner)
  {
    base.Transition(stateMachine, animationPlayer, owner);
    _player.Velocity = new Vector2(_player.Velocity.X, -_player.JumpStrength);
  }

  protected override Vector2 GetWalking(Vector2 direction, double delta)
  {
    Vector2 walkDirection = Vector2.Zero;
    if (Input.IsActionPressed("move_left"))
    {
      walkDirection.X -= _player.MaxSpeed;
    }
    if (Input.IsActionPressed("move_right"))
    {
      walkDirection.X += _player.MaxSpeed;
    }

    Vector2 lerpedDir = direction.Lerp(
      walkDirection,
      (float)delta * _player.Friction * 0.05f
    );

    return lerpedDir;
  }

  public override void PhysicsProcess(double delta)
  {
    Vector2 inputDir = CalculateDirection(delta);
    _player.Velocity = inputDir;

    _player.MoveAndSlide();
    _handleFireProjectile();

    if (_player.IsOnFloor())
    {
      _stateMachine.TransitionState("IdleState");
    }
  }
}
