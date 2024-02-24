using Centari.Navigation;
using Centari.State;
using Godot;

namespace Centari.Player.States;

public partial class WallSlideState : AbstractPlayerState
{
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

    if (_player.BodyRay.Item.GetCollider() is not TileMap tile)
    {
      // wat
      _stateMachine.TransitionState("IdleState");
    }
    else
    {
      Vector2 collisionPoint = _player.BodyRay.Item.GetCollisionPoint();
      collisionPoint.X += _player.HeadRay.IsFlipped
        ? -1
        : 1;

      Vector2I tileMapPoint = tile.LocalToMap(collisionPoint);
      Vector2 tileCenter = tile.MapToLocal(tileMapPoint);

      double tileWidth = tile.TileSet.TileSize.X;

      float playerSlidePosX = tileCenter.X;

      playerSlidePosX += _player.IsFlipped
        ? (float)(tileWidth / 2) + _player.GetLeftEdge()
        : -(float)(tileWidth / 2) + _player.GetRightEdge();

      // float playerSlideXPos = tileCenter.X
      //   + (float)(tileWidth / 2)
      //   + (_player.IsFlipped
      //     ? _player.GetLeftEdge()
      //     : _player.GetRightEdge()
      //   );

      _player.Position = new(playerSlidePosX, _player.Position.Y);
    }
  }

  public override void PhysicsProcess(double delta)
  {
    if (_player.IsFlipped && !_inputQueue.Dequeue(PlayerInput.MoveLeft))
    {
      _stateMachine.TransitionState("FallingState");
    }
    else if (!_player.IsFlipped && !_inputQueue.Dequeue(PlayerInput.MoveRight))
    {
      _stateMachine.TransitionState("FallingState");
    }
    if (InputQueue.LiveJustPressed(PlayerInput.Jump))
    {
      _inputQueue.Dequeue(PlayerInput.Jump);

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
    base.Detransition(nextState);
  }
}
