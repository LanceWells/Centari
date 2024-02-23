using Centari.Navigation;
using Centari.State;
using Godot;

namespace Centari.Player.States;

public partial class FallingState : AbstractPlayerState
{
  protected override StateCapabilities Capabilities => new()
  {
    CanWalk = true,
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
      (float)delta * _player.Friction * 0.2f
    );

    return lerpedDir;
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

    if (previousState == "JumpingState")
    {
      _animationPlayer.Play("Jump Max");
      _animationPlayer.Queue("Jump Down");
    }
    else
    {
      _animationPlayer.Play("Jump Down");
    }
  }

  /// <inheritdoc/>
  public override void Detransition()
  {
    _player.Velocity = _player.Velocity.Lerp(Vector2.Zero, 0.85f);
  }

  /// <inheritdoc/>
  public override void PhysicsProcess(double delta)
  {
    base.PhysicsProcess(delta);

    if (_player.IsOnFloor())
    {
      _stateMachine.TransitionState("IdleState");
    }
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
