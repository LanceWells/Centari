using System;
using Godot;

/// <summary>
/// An abstraction of the player's state. Used to add things like player reference.
/// </summary>
public abstract partial class AbstractPlayerState : AbstractState
{
  /// <summary>
  /// A reference to the player that this state refers to.
  /// </summary>
  protected Player _player;

  protected PackedScene _activeProjectile;

  abstract protected bool CanWalk { get; }

  abstract protected bool CanJump { get; }

  abstract protected bool GravityAffected { get; }

  virtual protected Vector2 GetWalking(Vector2 direction, double delta)
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
      (float)delta * _player.Friction
    );

    return lerpedDir;
  }

  virtual protected Vector2 GetGravity(Vector2 direction, double delta)
  {
    Vector2 gravity = new Vector2(0, _player.Gravity);
    direction = direction.Lerp(gravity, (float)delta);
    return direction;
  }

  protected Vector2 CalculateDirection(double delta)
  {
    Vector2 direction = _player.Velocity;

    if (GravityAffected)
    {
      direction = GetGravity(direction, delta);
    }

    if (CanWalk)
    {
      direction = GetWalking(direction, delta);
    }

    return direction;
  }

  /// <summary>
  /// Handler for projectile fire input. This can be called in the process frame for those states
  /// that allow the player to fire.
  /// </summary>
  protected void _handleFireProjectile()
  {
    if (Input.IsActionJustPressed("fire_projectile"))
    {
      PackedScene projectile = GD.Load<PackedScene>("res://Scenes/Projectiles/Fireball.tscn");
      _player.HandleFireProjectile(projectile);
    }
  }

  /// <summary>
  /// Base handler for determining if some movement should cause the player sprite to flip in a
  /// given direction.
  /// </summary>
  /// <returns>True if the player should be flipped from its original position.</returns>
  protected bool _shouldFlip()
  {
    bool doFlip = _player.BodySprite.FlipH;

    if (Input.IsActionPressed("move_left"))
    {
      doFlip = true;
    }
    if (Input.IsActionPressed("move_right"))
    {
      doFlip = false;
    }

    return doFlip;
  }

  /// <summary>
  /// Called when the node enters the scene tree for the first time. 
  /// </summary>
  public override void _Ready()
  { }

  /// <summary>
  /// Called every frame. 'delta' is the elapsed time since the previous frame.
  /// </summary>
  /// <param name="delta">The time elapsed since last frame.</param>
  public override void _Process(double delta)
  { }

  /// <inheritdoc/>
  protected override void Prepare(StateMachine stateMachine, AnimationPlayer animationPlayer, Node owner)
  {
    base.Prepare(stateMachine, animationPlayer, owner);
    _player = (Player)owner;
  }

  /// <inheritdoc/>
  public override void Transition(
    StateMachine stateMachine,
    AnimationPlayer animationPlayer,
    Node owner
  )
  {
    Prepare(stateMachine, animationPlayer, owner);
  }

  /// <inheritdoc/>
  public override void Detransition()
  { }

  /// <inheritdoc/>
  public override void Process(double delta)
  { }

  /// <inheritdoc/>
  public override void PhysicsProcess(double delta)
  { }

  public static float Lerp(float firstFloat, float secondFloat, float by)
  {
    return firstFloat * (1 - by) + secondFloat * by;
  }

  public static Vector2 QuadraticBezier(Vector2 p0, Vector2 p1, Vector2 p2, double delta)
  {
    Vector2 q0 = p0.Lerp(p1, (float)delta);
    Vector2 q1 = p1.Lerp(p2, (float)delta);

    Vector2 r = q0.Lerp(q1, (float)delta);
    return r;
  }
}
