using Godot;

namespace Centari.Player;

/// <summary>
/// This refers to the AimArm node. This node is visible only when the player is aiming. It also
/// determines the visual point from which a projectile should emerge, so we use this node to
/// determine projectile origin points as well.
/// </summary>
public partial class AimArm : Node2D
{
  /// <summary>
  /// If true, we are currently aiming.
  /// </summary>
  private bool IsAiming = false;

  /// <summary>
  /// A reference to the Arm sprite that we use instead of the regularly animated arm.
  /// </summary>
  private Sprite2D ArmSprite;

  /// <summary>
  /// A reference to the magic circle that is rendered at the end of the arm.
  /// </summary>
  private AnimatedSprite2D MagicCircleSprite;

  /// <summary>
  /// A reference to the aiming timer. This is used to determine when the player is no longer
  /// aiming.
  /// </summary>
  private Timer AimTimer;

  /// <summary>
  /// A reference to the animation player for the arm.
  /// </summary>
  private AnimationPlayer Animation;

  /// <summary>
  /// A reference to when the timer for when the magic circle should begin to fade before the arm
  /// is "put away" and we stop aiming.
  /// </summary>
  private Timer FadeTimer;

  /// <summary>
  /// A signal that is emitted when the aim arm is no longer aiming. This should be consumed by the
  /// player node in particular to indicate that it should render the regular arm instead.
  /// </summary>
  [Signal]
  public delegate void OnAimTimerTimeoutEventHandler();

  // public void HandleFlip(bool isFlipped)
  // {
  //   Vector2 scale = Scale;
  //   Scale = new(
  //     isFlipped ? -1 : 1,
  //     scale.Y
  //   );
  // }

  /// <summary>
  /// Called when the arm is newly visible. This is the means by which to start "aiming".
  /// </summary>
  public void StartAimStance()
  {
    IsAiming = true;

    AimTimer.Stop();
    AimTimer.Start();

    FadeTimer.Stop();
    FadeTimer.Start(AimTimer.WaitTime - 0.5);

    Animation.Stop();
    Animation.Play("Fire");
  }

  /// <summary>
  /// Used to get the point at which projectiles created by the arm should originate.
  /// </summary>
  /// <returns></returns>
  public Vector2 GetProjectileOrigin()
  {
    return MagicCircleSprite.GlobalPosition;
  }

  /// <summary>
  /// Called when the <see cref="FadeTimer"=> expires.
  /// </summary>
  private void _onFadeTimerTimeout()
  {
    Animation.Play("FadeOut");
  }

  /// <summary>
  /// Called when the <see cref="AimTimer"/> expires.
  /// </summary>
  private void _onAimTimerTimeout()
  {
    IsAiming = false;
    EmitSignal(SignalName.OnAimTimerTimeout);
  }

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    ArmSprite = GetNode<Sprite2D>("ArmSprite");
    MagicCircleSprite = GetNode<AnimatedSprite2D>("ArmSprite/MagicCircleSprite");
    Animation = GetNode<AnimationPlayer>("Animation");

    AimTimer = GetNode<Timer>("AimTimer");
    FadeTimer = GetNode<Timer>("FadeTimer");
    AimTimer.Timeout += _onAimTimerTimeout;
    FadeTimer.Timeout += _onFadeTimerTimeout;
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
    if (IsAiming)
    {
      Vector2 mousePos = GetViewport().GetMousePosition();
      ArmSprite.LookAt(mousePos);
    }
  }
}
