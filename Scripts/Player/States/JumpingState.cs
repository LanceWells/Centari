using System;
using Centari.Navigation;
using Centari.State;
using Godot;

namespace Centari.Player.States;

public partial class JumpingState : AbstractPlayerState
{
  /// <summary>
  /// This timer defines how long the player can be moving upwards in their jump. Once this has
  /// timed out, then the player should transition into a falling state.
  /// </summary>
  public Timer JumpEnabledTimer;

  /// <summary>
  /// This timer defines how long before the player can "cancel" the jump. This lets a jump always
  /// have some lift off the ground, but still lets the player cancel the top portion of the jump if
  /// they want a lower trajectory.
  /// </summary>
  public Timer JumpCancelAfterTimer;

  /// <summary>
  /// If true, the player can cancel the jump. Set via <see cref="JumpCancelAfterTimer"/>.
  /// </summary>
  private bool _canCancelJump;

  protected override StateCapabilities Capabilities => new()
  {
    CanWalk = true,
    CanJump = true,
    CanAttack = true,
    GravityAffected = true,
  };

  /// <summary>
  /// This handler is called when the "mantle area" for a player collides witha body. This is used
  /// to determine if the player's foot is caught against a tile corner. If this happens, we push
  /// the player up and over the corner to help them move.
  /// </summary>
  /// <param name="body">The body that was collided with.</param>
  public void OnMantleClimbRayBodyEntered(Node2D body)
  {
    PlayerInputs p = GetPlayerInputs();

    Console.WriteLine("mantle");

    Vector2 mantleBoost = Vector2.Zero;
    // if (p.MoveLeft && _player.Velocity.X < 0 && Math.Abs(_player.Velocity.Y) < 500)
    if (p.MoveLeft && _player.Velocity.X < 0)
    {
      mantleBoost.Y -= _player.MaxSpeed * 2;
      mantleBoost.X -= _player.MaxSpeed;
    }

    // if (p.MoveRight && _player.Velocity.X > 0 && Math.Abs(_player.Velocity.Y) < 500)
    if (p.MoveRight && _player.Velocity.X > 0)
    {
      mantleBoost.Y -= _player.MaxSpeed * 2;
      mantleBoost.X += _player.MaxSpeed;
    }

    // TODO: If the mantle activates, do a lil mantle animation.

    _player.Velocity = mantleBoost;

    _stateMachine.TransitionState("MantleClimbState");
  }

  public void OnJumpEnabledTimeout()
  {
    if (_player.IsOnFloor())
    {
      _stateMachine.TransitionState("JumpingState");
    }
    else
    {
      _stateMachine.TransitionState("FallingState");
    }
  }

  public void OnJumpCancelAfterTimeout()
  {
    _canCancelJump = true;
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

  protected override Vector2 GetJumping(Vector2 direction, double delta)
  {
    PlayerInputs p = GetPlayerInputs();

    Vector2 vel = direction;
    if (!_canCancelJump || p.Jump)
    {
      vel.Y = -_player.JumpStrength;
    }

    return vel;
  }

  public override void Transition(StateMachine stateMachine, AnimationPlayer animationPlayer, Node owner)
  {
    base.Transition(stateMachine, animationPlayer, owner);

    JumpEnabledTimer = GetNode<Timer>("JumpEnabledTimer");
    JumpEnabledTimer.Timeout += OnJumpEnabledTimeout;
    JumpEnabledTimer.Start();

    JumpCancelAfterTimer = GetNode<Timer>("JumpCancelAfterTimer");
    JumpCancelAfterTimer.Timeout += OnJumpCancelAfterTimeout;
    JumpCancelAfterTimer.Start();

    _canCancelJump = false;
  }

  public override void Detransition()
  {
    JumpEnabledTimer.Timeout -= OnJumpEnabledTimeout;
    JumpCancelAfterTimer.Timeout -= OnJumpCancelAfterTimeout;
  }

  public override void Process(double delta)
  {
    base.Process(delta);

    if (_player.HeadRay.Item.GetCollider() is TileMap)
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

  public override void PhysicsProcess(double delta)
  {
    base.PhysicsProcess(delta);
    PlayerInputs p = GetPlayerInputs();

    if (_player.IsOnFloor())
    {
      _stateMachine.TransitionState("IdleState");
    }
    else if (_canCancelJump && !p.Jump)
    {
      _stateMachine.TransitionState("FallingState");
    }
  }
}
