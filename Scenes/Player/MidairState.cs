using System;
using Godot;

public partial class MidairState : AbstractPlayerState
{
  /// <inheritdoc/>
  protected override bool CanWalk => true;

  /// <inheritdoc/>
  protected override bool GravityAffected => true;

  /// <inheritdoc/>
  protected override bool CanAttack => true;

  /// <inheritdoc/>
  protected override bool CanFlip => true;

  protected override bool CanJump => false;

  public void OnBodyEntered(Node2D body)
  {
    Console.WriteLine("Mantle");
  }

  /// <inheritdoc/>
  public override void Transition(StateMachine stateMachine, AnimationPlayer animationPlayer, Node owner)
  {
    base.Transition(stateMachine, animationPlayer, owner);
    // _player.Velocity = new Vector2(_player.Velocity.X, -_player.JumpStrength);
    _player.MantleArea.BodyEntered += OnBodyEntered;
  }

  public override void Detransition()
  {
    base.Detransition();
    _player.MantleArea.BodyEntered -= OnBodyEntered;
  }

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

  /// <inheritdoc/>
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
