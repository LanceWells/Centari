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

    if (_player.BodyRay.GetCollider() is not TileMap tile)
    {
      // wat
      _stateMachine.TransitionState("IdleState");
    }
    else
    {
      Vector2 collisionPoint = _player.BodyRay.GetCollisionPoint();
      collisionPoint.X += _player.IsFlipped
        ? -1
        : 1;

      Vector2I tileMapPoint = tile.LocalToMap(collisionPoint);
      Vector2 tileCenter = tile.MapToLocal(tileMapPoint);

      Vector2 mantleCorner = new(
        tileCenter.X,
        tileCenter.Y
      );

      mantleCorner.X += _player.IsFlipped
        ? +(tile.TileSet.TileSize.X / 2)
        : -(tile.TileSet.TileSize.X / 2);

      mantleCorner.Y -= tile.TileSet.TileSize.Y / 2;

      _mantleCorner = mantleCorner;
    }
  }

  /// <inheritdoc/>
  public override void Detransition(string nextState)
  {
    base.Detransition(nextState);

    Vector2 playerMantlePoint = _player.MantleCornerPoint.Position * _player.Scale;
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

    Vector2 playerMantlePoint = _player.MantleCornerPoint.Position * _player.Scale;
    Vector2 newPos = new()
    {
      X = _mantleCorner.X + (_player.IsFlipped ? playerMantlePoint.X : -playerMantlePoint.X),
      Y = _mantleCorner.Y - playerMantlePoint.Y
    };

    _player.Position = newPos;
    _player.Velocity = Vector2.Zero;
  }
}
