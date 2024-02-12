using Centari.Navigation;
using Centari.State;
using Godot;

namespace Centari.Player.States;

public partial class MantleClimbState : AbstractPlayerState
{
  // private TileInfo _mantlingTile;

  private Vector2 _mantleCorner;

  protected override StateCapabilities Capabilities => new()
  { };

  public void OnAnimationFinished(StringName animationName)
  {
    _stateMachine.TransitionState("IdleState");
  }

  public override void Transition(StateMachine stateMachine, AnimationPlayer animationPlayer, Node owner)
  {
    base.Transition(stateMachine, animationPlayer, owner);
    _player.Velocity = Vector2.Zero;
    _animationPlayer.Play("MantleClimb");
    _animationPlayer.AnimationFinished += OnAnimationFinished;

    if (_player.BodyRay.Item.GetCollider() is not TileMap tileMap)
    {
      // wat
      _stateMachine.TransitionState("IdleState");
    }
    else
    {
      var p = GetPlayerInputs();

      Vector2 collisionPoint = _player.BodyRay.Item.GetCollisionPoint();
      collisionPoint.X += _player.HeadRay.IsFlipped
        ? -1
        : 1;

      Vector2I tileMapPoint = tileMap.LocalToMap(collisionPoint);
      TileData tileData = tileMap.GetCellTileData(0, tileMapPoint);
      Vector2 tileCenter = tileMap.MapToLocal(tileMapPoint);

      Vector2 mantleCorner = new(
        tileCenter.X,
        tileCenter.Y
      );

      mantleCorner.X += _player.IsFlipped
        ? +(tileMap.TileSet.TileSize.X / 2)
        : -(tileMap.TileSet.TileSize.X / 2);

      mantleCorner.Y -= tileMap.TileSet.TileSize.Y / 2;

      _mantleCorner = mantleCorner;
    }
  }

  public override void Detransition()
  {
    base.Detransition();
    Vector2 newPos = new()
    {
      X = _mantleCorner.X + _player.MantleCornerPoint.Item.Position.X,
      Y = _mantleCorner.Y - _player.MantleCornerPoint.Item.Position.Y
    };

    _player.Position = newPos;
    _animationPlayer.AnimationFinished -= OnAnimationFinished;
  }

  public override void Process(double delta)
  {
    base.Process(delta);

    Vector2 playerMantlePoint = _player.MantleCornerPoint.Item.Position;
    Vector2 newPos = new()
    {
      X = _mantleCorner.X + (_player.IsFlipped ? playerMantlePoint.X : -playerMantlePoint.X),
      Y = _mantleCorner.Y - _player.MantleCornerPoint.Item.Position.Y
    };

    _player.Position = newPos;
    _player.Velocity = Vector2.Zero;
  }

  public override void PhysicsProcess(double delta)
  {
    base.PhysicsProcess(delta);

  }
}
