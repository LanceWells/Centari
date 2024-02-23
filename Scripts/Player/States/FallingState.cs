using System;
using Centari.Navigation;
using Centari.State;
using Godot;

namespace Centari.Player.States;

public partial class FallingState : AbstractPlayerState
{
  private Timer _coyoteTimer;

  private bool _canCoyote = false;

  protected override StateCapabilities Capabilities => new()
  {
    CanWalk = true,
    CanAttack = true,
    GravityAffected = true,

    // Coyote jump 🦅
    CanJump = true,
  };

  /// <inheritdoc/>
  protected override Vector2 GetWalking(Vector2 direction, double delta)
  {
    Vector2 walkDirection = Vector2.Zero;
    if (_inputQueue.Dequeue(PlayerInput.MoveLeft))
    {
      walkDirection.X -= _player.MaxSpeed;
    }
    if (_inputQueue.Dequeue(PlayerInput.MoveRight))
    {
      walkDirection.X += _player.MaxSpeed;
    }

    Vector2 lerpedDir = direction.Lerp(
      walkDirection,
      (float)delta * _player.Friction * 0.2f
    );

    return lerpedDir;
  }

  public void OnCoyoteTimeout()
  {
    _canCoyote = false;
  }

  /// <inheritdoc/>
  protected override Vector2 GetJumping(Vector2 direction, double delta)
  {
    if (!_canCoyote || !InputQueue.LivePeek(PlayerInput.Jump))
    {
      return direction;
    }

    Vector2 notFallingVector = new(
      direction.X,
      Math.Min(direction.Y, 0)
    );

    Vector2 jumping = base.GetJumping(notFallingVector, delta);
    return jumping;
  }

  /// <inheritdoc/>
  public override void Transition(
  StateMachine stateMachine,
  AnimationPlayer animationPlayer,
  Node owner,
  string previousState
)
  {
    base.Transition(stateMachine, animationPlayer, owner, previousState);

    _canCoyote = false;
    _coyoteTimer = GetNode<Timer>("CoyoteTimer");
    _coyoteTimer.Timeout += OnCoyoteTimeout;

    if (previousState == "JumpingState")
    {
      _animationPlayer.Play("Jump Max");
      _animationPlayer.Queue("Jump Down");
    }
    else
    {
      _animationPlayer.Play("Jump Down");
      _canCoyote = true;
      _coyoteTimer.Start();
    }
  }

  /// <inheritdoc/>
  public override void Detransition(string nextState)
  {
    base.Detransition(nextState);

    bool playerMoving = InputQueue.LivePeek(PlayerInput.MoveLeft)
      || InputQueue.LivePeek(PlayerInput.MoveRight);

    // Only make a landing "stick" if we're not doing a coyote jump.
    if (nextState != "JumpingState" && !playerMoving)
    {
      _player.Velocity = _player.Velocity.Lerp(Vector2.Zero, 0.9f);
    }

    _coyoteTimer.Timeout -= OnCoyoteTimeout;
  }

  /// <inheritdoc/>
  public override void PhysicsProcess(double delta)
  {
    bool pressJump = _inputQueue.Peek(PlayerInput.Jump);

    if (_player.IsOnFloor())
    {
      _stateMachine.TransitionState("IdleState");
    }
    if (pressJump && _canCoyote)
    {
      _stateMachine.TransitionState("JumpingState");
    }

    base.PhysicsProcess(delta);
  }

  /// <inheritdoc/>
  public override void Process(double delta)
  {
    base.Process(delta);

    if (_player.HeadRay.Item.GetCollider() is not null)
    {
      return;
    }

    if (_player.BodyRay.Item.GetCollider() is not TileMap tileMap)
    {
      return;
    }

    // There's what seems to be a bug when raycasting to the left against a tilemap. The pixel for
    // detecting a given tile to the right seems to land in the tile, but raycasting to the left
    // seems to pick the tile to the right of that tile when translating to map coords.

    Vector2 collisionPoint = _player.BodyRay.Item.GetCollisionPoint();
    collisionPoint.X += _player.HeadRay.IsFlipped
      ? -1
      : 1;

    Vector2I tileMapPoint = tileMap.LocalToMap(collisionPoint);
    TileData tileData = tileMap.GetCellTileData(0, tileMapPoint);
    TileInfo tileInfo = new(tileData, tileMapPoint);

    if (!tileInfo.IsPlatform)
    {
      return;
    }

    _stateMachine.TransitionState("MantleClimbState");
  }
}
