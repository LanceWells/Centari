using Centari.Player.States;
using Centari.State;
using Godot;

namespace Centari.Player;

/// <summary>
/// An abstraction of the player's state. Used to add things like player reference.
/// </summary>
public abstract partial class AbstractPlayerState : AbstractState
{
  /// <summary>
  /// The input queue to be used for the player. Note that this is actually owned by the player, and
  /// is only referenced by this base class for ease of use. We keep it on the player so that the
  /// queue can cross states.
  /// </summary>
  protected InputQueue _inputQueue;

  /// <summary>
  /// A reference to the player that this state refers to.
  /// </summary>
  protected PlayerCharacter _player;

  /// <summary>
  /// A list of capabilities for the player. Should be set on every state.
  /// </summary>
  abstract protected StateCapabilities Capabilities { get; }

  /// <summary>
  /// Gets the horizontal movement vector based on the current player input.
  /// </summary>
  /// <param name="direction">
  /// The current direction of the player. This will be lerped against the derived movement, which
  /// is then returned.
  /// </param>
  /// <param name="delta">The time delta since the last frame.</param>
  /// <returns>The new vector, post-lerp.</returns>
  virtual protected Vector2 GetWalking(Vector2 direction, double delta)
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
      (float)delta * _player.Friction
    );

    return lerpedDir;
  }

  /// <summary>
  /// Gets the vertical movement vector based on player jump inputs.
  /// </summary>
  /// <param name="direction">
  /// The current direction of the player. This will be lerped against the derived movement, which
  /// is then returned.
  /// </param>
  /// <param name="delta">The time delta since the last frame.</param>
  /// <returns>The new vector, post-lerp.</returns>
  virtual protected Vector2 GetJumping(Vector2 direction, double delta)
  {
    Vector2 jumpDirection = Vector2.Zero;
    if (_inputQueue.Dequeue(PlayerInput.Jump))
    {
      jumpDirection.Y -= _player.JumpStrength;
    }

    return direction + jumpDirection;
  }

  /// <summary>
  /// Gets the gravity movement vector for the player.
  /// </summary>
  /// <param name="direction">
  /// The current direction of the player. This will be lerped against the derived movement, which
  /// is then returned.
  /// </param>
  /// <param name="delta">The time delta since the last frame.</param>
  /// <returns>The new vector, post-lerp.</returns>
  virtual protected Vector2 GetGravity(Vector2 direction, double delta)
  {
    Vector2 gravity = new(0, _player.Gravity);
    direction = direction.Lerp(gravity, (float)delta);
    direction = direction.Clamp(new(direction.X, -300), new(direction.X, 300));
    return direction;
  }

  virtual protected Vector2 GetMomentum(Vector2 direction, double delta)
  {
    Vector2 momentum = new(_player.Momentum, 0);
    momentum *= _player.IsFlipped ? -1 : 1;
    direction += momentum;

    _player.Momentum = 0;
    return direction;
  }

  /// <summary>
  /// Calculates the direction that the player should head in, given all of their inputs and
  /// external physics forces.
  /// </summary>
  /// <param name="delta">The time delta since the last frame.</param>
  /// <returns>The new velocity that should be used for the player.</returns>
  protected Vector2 CalculateDirection(double delta)
  {
    Vector2 direction = _player.Velocity;

    if (Capabilities.GravityAffected)
    {
      direction = GetGravity(direction, delta);
    }

    if (Capabilities.CanWalk)
    {
      direction = GetWalking(direction, delta);
    }

    if (Capabilities.CanJump)
    {
      direction = GetJumping(direction, delta);
    }

    direction = GetMomentum(direction, delta);

    return direction;
  }

  /// <summary>
  /// Handler for projectile fire input. This can be called in the process frame for those states
  /// that allow the player to fire.
  /// </summary>
  protected void _handleFireProjectile()
  {
    PackedScene projectile = GD.Load<PackedScene>("res://Scenes/Projectiles/Fireball.tscn");
    _player.HandleFireProjectile(projectile);
  }

  /// <summary>
  /// Base handler for determining if some movement should cause the player sprite to flip in a
  /// given direction.
  /// </summary>
  /// <returns>True if the player should be flipped from its original position.</returns>
  protected bool _shouldFlip()
  {
    // PlayerInputs p = GetPlayerInputs();

    bool moveLeft = _inputQueue.Peek(PlayerInput.MoveLeft);
    bool moveRight = _inputQueue.Peek(PlayerInput.MoveRight);

    bool doFlip = _player.IsFlipped;

    if (moveLeft)
    {
      doFlip = true;
    }
    if (moveRight)
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

  /// <inheritdoc/>
  protected override void Prepare(StateMachine stateMachine, AnimationPlayer animationPlayer, Node owner)
  {
    base.Prepare(stateMachine, animationPlayer, owner);
    _player = (PlayerCharacter)owner;
    _inputQueue = _player.InputQueue;
  }

  /// <inheritdoc/>
  public override void Transition(
    StateMachine stateMachine,
    AnimationPlayer animationPlayer,
    Node owner,
    string previousState
  )
  {
    Prepare(stateMachine, animationPlayer, owner);
  }

  /// <inheritdoc/>
  public override void Detransition(string nextState)
  {
    _animationPlayer.Play("RESET");
    _animationPlayer.Stop();
  }

  /// <inheritdoc/>
  public override void PhysicsProcess(double delta)
  {
    base.PhysicsProcess(delta);

    if (Capabilities.CanFlip)
    {
      _player.HandleFlip(_shouldFlip());
    }
    // if (Capabilities.CanAttack)
    // {
    //   _handleFireProjectile();
    // }

    Vector2 inputDir = CalculateDirection(delta);
    _player.Velocity = inputDir;
    _player.MoveAndSlide();
  }

  /// <inheritdoc/>
  public override void Process(double delta)
  {
    base.Process(delta);
    _inputQueue.Refresh(delta);
  }
}
