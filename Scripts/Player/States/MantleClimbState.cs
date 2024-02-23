using Centari.Navigation;
using Centari.State;
using Godot;

namespace Centari.Player.States;

public partial class MantleClimbState : AbstractPlayerState
{
  private Vector2 _mantleCorner;

  protected override StateCapabilities Capabilities => new()
  { };

  /// <summary>
  /// When the animation has finished, that means that the mantle has completed. This is def
  /// intentional so that we can configure the length of the mantle based on the animation, and
  /// there's no synchronization needed between the two timings.
  /// </summary>
  /// <param name="animationName"></param>
  public void OnAnimationFinished(StringName animationName)
  {
    _stateMachine.TransitionState("IdleState");
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

  /// <inheritdoc/>
  public override void Detransition(string nextState)
  {
    base.Detransition(nextState);

    Vector2 playerMantlePoint = _player.MantleCornerPoint.Item.Position * _player.Scale;
    Vector2 newPos = new()
    {
      X = _mantleCorner.X + playerMantlePoint.X,
      Y = _mantleCorner.Y - playerMantlePoint.Y
    };

    _player.Position = newPos;
    _animationPlayer.AnimationFinished -= OnAnimationFinished;
  }

  /// <inheritdoc/>
  public override void Process(double delta)
  {
    base.Process(delta);

    Vector2 playerMantlePoint = _player.MantleCornerPoint.Item.Position * _player.Scale;
    Vector2 newPos = new()
    {
      X = _mantleCorner.X + (_player.IsFlipped ? playerMantlePoint.X : -playerMantlePoint.X),
      Y = _mantleCorner.Y - playerMantlePoint.Y
    };

    _player.Position = newPos;
    _player.Velocity = Vector2.Zero;
  }
}
