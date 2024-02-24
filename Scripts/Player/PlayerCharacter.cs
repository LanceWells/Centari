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
  public float MaxSpeed = 400.0f;

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

  /// <summary>
  /// The effect of friction on the player.
  /// </summary>
  [Export]
  public float Friction = 8.0f;

  /// <summary>
  /// The input queue to use with the player. Keeps track of intended player movements for use
  /// primarily between states.
  /// </summary>
  public InputQueue InputQueue = new(new Dictionary<PlayerInput, double>()
  {
    { PlayerInput.Jump, 0.1 }
  });

  /// <summary>
  /// A reference to the head ray on the player.
  /// </summary>
  public FlippableRayCast HeadRay;

  public FlippableRayCast BodyRay;

  public FlippableRayCast FeetRay;

  private CollisionShape2D _hitBox;

  public FlippableNode<Node2D> MantleCornerPoint;

  public bool IsFlipped => _isFlipped;

  private bool _isFlipped = false;

  /// <summary>
  /// A reference to the ArmSprite node for the player.
  /// </summary>
  private Sprite2D ArmSprite;

  /// <summary>
  /// A reference to the <see cref="AimArm" /> node for the player.
  /// </summary>
  private AimArm AimArm;

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
    Vector2 target
  );

  /// <summary>
  /// This should be called when the player indicates that they want to fire a projectile at the
  /// standard targeting point.
  /// </summary>
  /// <param name="projectile">The projectile to fire.</param>
  public void HandleFireProjectile(PackedScene projectile)
  {
    AimArm.Visible = true;
    ArmSprite.Visible = false;
    AimArm.StartAimStance();

    Vector2 mouse = GetViewport().GetMousePosition();
    Vector2 projectileOrigin = AimArm.GetProjectileOrigin();

    EmitSignal(
      SignalName.FireProjectile,
      projectile,
      projectileOrigin,
      mouse
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


    HeadRay.SetFlipped(IsFlipped);
    BodyRay.SetFlipped(IsFlipped);
    FeetRay.SetFlipped(IsFlipped);
    MantleCornerPoint.SetFlipped(isFlipped);

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

  /// <summary>
  /// This is called when the aim arm has "timed out". In this scenario, it means that the player is
  /// no longer aiming for any given reason. This should be largely a visual change.
  /// </summary>
  public void OnAimArmTimerTimeout()
  {
    AimArm.Visible = false;
    ArmSprite.Visible = true;
  }

  /// <inheritdoc/>
  public override void _Ready()
  {
    _bodySprite = GetNode<Sprite2D>("Sprites/BodySprite");
    ArmSprite = GetNode<Sprite2D>("Sprites/ArmSprite");

    var sprites = GetNode<Sprite2D>("Sprites");
    _sprites = new FlippableSprite<Sprite2D>(sprites);

    var _headRay = GetNode<RayCast2D>("HeadRay");
    var _bodyRay = GetNode<RayCast2D>("BodyRay");
    var _feetRay = GetNode<RayCast2D>("FeetRay");

    HeadRay = new FlippableRayCast(_headRay);
    BodyRay = new FlippableRayCast(_bodyRay);
    FeetRay = new FlippableRayCast(_feetRay);

    var _mantleCornerPoint = GetNode<Node2D>("MantleCornerPoint");

    MantleCornerPoint = new FlippableNode<Node2D>(_mantleCornerPoint);

    AimArm = GetNode<AimArm>("Sprites/AimArm");
    AimArm.OnAimTimerTimeout += OnAimArmTimerTimeout;

    _hitBox = GetNode<CollisionShape2D>("Hitbox");
  }
}
