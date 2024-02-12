using Godot;

namespace Centari.Player;

/// <summary>
/// This class is used to refer to the player scene itself.
/// </summary>
public partial class PlayerCharacter : CharacterBody2D
{
  /// <summary>
  /// A configurable walking speed for the player.
  /// </summary>
  [Export]
  public float MaxSpeed = 400.0f;

  [Export]
  public float JumpStrength = 500.0f;

  [Export]
  public int AimPartAnchorX = -4;

  [Export]
  public float Gravity = 3000.0f;

  [Export]
  public float Friction = 8.0f;

  /// <summary>
  /// A reference to the BodySprite node for the player.
  /// </summary>
  public Sprite2D BodySprite;

  public RayCast2D HeadRay;

  public RayCast2D BodyRay;

  public RayCast2D FeetRay;

  public bool IsFlipped => _isFlipped;

  private bool _isFlipped = false;

  private Vector2 _initialMantlePosition;

  public Node2D MantleCornerPoint;

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

    ArmSprite.FlipH = isFlipped;
    BodySprite.FlipH = isFlipped;

    if (isFlipped)
    {
      AimArm.Position = new Vector2(-AimPartAnchorX, AimArm.Position.Y);
      HeadRay.Position = new Vector2(
        -_initialMantlePosition.X,
        _initialMantlePosition.Y
      );
    }
    else
    {
      AimArm.Position = new Vector2(AimPartAnchorX, AimArm.Position.Y);
      HeadRay.Position = new Vector2(
        _initialMantlePosition.X,
        _initialMantlePosition.Y
      );
    }
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
    BodySprite = GetNode<Sprite2D>("BodySprite");
    ArmSprite = GetNode<Sprite2D>("ArmSprite");
    HeadRay = GetNode<RayCast2D>("HeadRay");
    BodyRay = GetNode<RayCast2D>("BodyRay");
    FeetRay = GetNode<RayCast2D>("FeetRay");
    MantleCornerPoint = GetNode<Node2D>("MantleCornerPoint");

    AimArm = GetNode<AimArm>("AimArm");
    AimArm.OnAimTimerTimeout += OnAimArmTimerTimeout;
    AimArm.Position = new Vector2(AimPartAnchorX, AimArm.Position.Y);

    _initialMantlePosition = new Vector2(HeadRay.Position.X, HeadRay.Position.Y);
  }
}
