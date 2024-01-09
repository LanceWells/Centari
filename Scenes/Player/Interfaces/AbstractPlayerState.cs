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

  /// <summary>
  /// Handler for movement. This can be called in the process frame for those states that allow for
  /// the player to fire.
  /// </summary>
  /// <param name="delta">The delta since the last process was called.</param>
  /// <returns>The adjustment to the current direction.</returns>
  protected Vector2 _handleMovement(double delta)
  {
    Vector2 direction = Vector2.Zero;

    if (Input.IsActionPressed("move_up"))
    {
      direction.Y -= 1;
    }
    if (Input.IsActionPressed("move_down"))
    {
      direction.Y += 1;
    }
    if (Input.IsActionPressed("move_left"))
    {
      direction.X -= 1;
    }
    if (Input.IsActionPressed("move_right"))
    {
      direction.X += 1;
    }

    if (direction != Vector2.Zero)
    {
      direction = direction.Normalized() * _player.MaxSpeed;
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
}
