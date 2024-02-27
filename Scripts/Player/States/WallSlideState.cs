using System;
using Centari.Navigation;
using Centari.State;
using Godot;

namespace Centari.Player.States;

public partial class WallSlideState : AbstractPlayerState
{
  private Timer _dropTimer;

  private float _playerSlidePosX;

  protected override StateCapabilities Capabilities => new()
  {
    CanAttack = true,
    GravityAffected = true,
  };

  protected override Vector2 GetGravity(Vector2 direction, double delta)
  {
    Vector2 gravity = new(0, _player.Gravity / 10);
    direction = direction.Lerp(gravity, (float)delta * 0.2f);
    return direction;
  }

  public void OnDropTimeout()
  {
    _stateMachine.TransitionState("FallingState");
  }

  public override void Transition(
    StateMachine stateMachine,
    AnimationPlayer animationPlayer,
    Node owner,
    string previousState
  )
  {
    base.Transition(stateMachine, animationPlayer, owner, previousState);

    _player.Velocity = Vector2.Zero;
    animationPlayer.Play("Wall Slide");

    _dropTimer = GetNode<Timer>("DropTimer");
    _dropTimer.Timeout += OnDropTimeout;

    if (_player.BodyRay.Item.GetCollider() is not TileMap tile)
    {
      // wat
      _stateMachine.TransitionState("IdleState");
    }
  }

  public override void PhysicsProcess(double delta)
  {
    if (_player.BodyRay.Item.GetCollider() is not TileMap tile)
    {
      _stateMachine.TransitionState("IdleState");
      return;
    }
    else
    {
      Vector2 collisionPoint = _player.BodyRay.Item.GetCollisionPoint();
      collisionPoint.X += _player.HeadRay.IsFlipped
      ? -1
      : 1;

      Vector2I tileMapPoint = tile.LocalToMap(collisionPoint);
      TileData tileData = tile.GetCellTileData(0, tileMapPoint);
      TileInfo tileInfo = new(tileData, tileMapPoint);

      if (!tileInfo.IsPlatform)
      {
        _stateMachine.TransitionState("IdleState");
        return;
      }
    }

    if (_player.IsOnFloor())
    {
      _stateMachine.TransitionState("IdleState");
      return;
    }

    bool moveL = _inputQueue.Dequeue(PlayerInput.MoveLeft);
    bool moveR = _inputQueue.Dequeue(PlayerInput.MoveRight);
    bool shouldDrop = (_player.IsFlipped && !moveL) || (!_player.IsFlipped && !moveR);

    if (_dropTimer.IsStopped() && shouldDrop)
    {
      _dropTimer.Start();
    }
    else if (!shouldDrop)
    {
      _dropTimer.Stop();
    }

    if (InputQueue.LiveJustPressed(PlayerInput.Jump))
    {
      Vector2 jumpDirection = new();
      jumpDirection.Y -= 1;
      jumpDirection.X += _player.IsFlipped ? 1 : -1;
      jumpDirection = jumpDirection.Normalized() * _player.JumpStrength;

      _player.Velocity = jumpDirection;
      _player.HandleFlip(!_player.IsFlipped);

      _stateMachine.TransitionState("JumpingState");
    }

    base.PhysicsProcess(delta);
  }

  public override void Detransition(string nextState)
  {
    _inputQueue.Dequeue(PlayerInput.Jump);
    _dropTimer.Timeout -= OnDropTimeout;

    base.Detransition(nextState);
  }
}
