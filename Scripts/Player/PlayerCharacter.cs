using System.Collections.Generic;
using Centari.Common;
using Godot;

namespace Centari.Player;

/// <summary>
/// This class is used to refer to the player scene itself.
/// </summary>
public partial class PlayerCharacter : CharacterBody2D
{
  private Sprite2D _bodySprite;

  private FlippableSprite<Sprite2D> _sprites;

  /// <summary>
  /// A configurable walking speed for the player.
  /// </summary>
  [Export]
  public float MaxSpeed = 300.0f;

  /// <summary>
  /// The strength of the player's jump.
  /// </summary>
  [Export]
  public float JumpStrength = 500.0f;

  /// <summary>
  /// The strength of gravity on the player.
  /// </summary>
  [Export]
  public float Gravity = 3000.0f;

  [Export]
  public float Momentum = 0.0f;

  [Export]
  public bool ProjectileThrown = false;

  /// <summary>
  /// The effect of friction on the player.
  /// </summary>
  public float Friction = 8.0f;

  /// <summary>
  /// The input queue to use with the player. Keeps track of intended player movements for use
  /// primarily between states.
  /// </summary>
  public InputQueue InputQueue = new(new Dictionary<PlayerInput, double>()
  {
    { PlayerInput.Jump, 0.1 },
    { PlayerInput.Attack, 0.2 },
  });

  /// <summary>
  /// A reference to the head ray on the player.
  /// </summary>
  public RayCast2D HeadRay;

  public RayCast2D BodyRay;

  public RayCast2D FeetRay;

  private CollisionShape2D _hitBox;

  public Node2D MantleCornerPoint;

  public Node2D ProjectileOriginPoint;

  public bool IsFlipped => _isFlipped;

  private bool _isFlipped = false;

  /// <summary>
  /// A reference to the ArmSprite node for the player.
  /// </summary>
  private Sprite2D ArmSprite;

  // /// <summary>
  // /// A reference to the <see cref="AimArm" /> node for the player.
  // /// </summary>
  // private AimArm AimArm;

  /// <summary>
  /// A signal used to indicate that the player wants to fire a projectile. This informs the
  /// projectile manager in the level that it needs to create the projectile and to add it to the
  /// current scene.
  /// </summary>
  /// <param name="projectile">The projectile to add to the scene.</param>
  /// <param name="origin">The origin point for the projectile.</param>
  /// <param name="target">The point at which to fire the projectile</param>
  [Signal]
  public delegate void FireProjectileEventHandler(
    PackedScene projectile,
    Vector2 origin,
    bool isFlipped
  );

  /// <summary>
  /// This should be called when the player indicates that they want to fire a projectile at the
  /// standard targeting point.
  /// </summary>
  /// <param name="projectile">The projectile to fire.</param>
  public void HandleFireProjectile(PackedScene projectile)
  {
    Vector2 projectileOrigin = ProjectileOriginPoint.Position + Position;

    EmitSignal(
      SignalName.FireProjectile,
      projectile,
      projectileOrigin,
      _isFlipped
    );
  }

  /// <summary>
  /// This should be called when the player sprite should be flipped in a given direction.
  /// </summary>
  /// <param name="isFlipped">
  /// If true, flip the default direction for the sprite. Otherwise, make the sprite face the
  /// default direction.
  /// </param>
  public void HandleFlip(bool isFlipped)
  {
    _isFlipped = isFlipped;

    _sprites.SetFlipped(isFlipped);
  }

  public float GetLeftEdge()
  {
    return -((CapsuleShape2D)_hitBox.Shape).Radius / 2;
  }

  public float GetRightEdge()
  {
    return ((CapsuleShape2D)_hitBox.Shape).Radius / 2;
  }

  /// <inheritdoc/>
  public override void _Ready()
  {
    _bodySprite = GetNode<Sprite2D>("Sprites/BodySprite");
    ArmSprite = GetNode<Sprite2D>("Sprites/ArmSprite");

    var sprites = GetNode<Sprite2D>("Sprites");
    _sprites = new FlippableSprite<Sprite2D>(sprites);

    HeadRay = GetNode<RayCast2D>("Sprites/HeadRay");
    BodyRay = GetNode<RayCast2D>("Sprites/BodyRay");
    FeetRay = GetNode<RayCast2D>("Sprites/FeetRay");

    MantleCornerPoint = GetNode<Node2D>("Sprites/MantleCornerPoint");
    ProjectileOriginPoint = GetNode<Node2D>("Sprites/ProjectileOriginPoint");

    _hitBox = GetNode<CollisionShape2D>("Hitbox");
  }
}
