using Godot;

public partial class Player : CharacterBody2D
{
  [Export]
  public float MaxSpeed = 14.0f;

  [Export]
  public float Friction = 0.75f;

  public Sprite2D BodySprite;

  private Sprite2D ArmSprite;

  private AimArm AimArm;

  [Signal]
  public delegate void FireProjectileEventHandler(
    PackedScene projectile,
    Vector2 origin,
    Vector2 target
  );

  public void HandleFireProjectile(PackedScene projectile)
  {
    AimArm.Visible = true;
    ArmSprite.Visible = false;
    AimArm.StartAimStance();

    Vector2 mouse = GetViewport().GetMousePosition();
    Vector2 projectileOrigin = Vector2.Zero;

    projectileOrigin += new Vector2(24, -1);
    projectileOrigin = projectileOrigin.Rotated(Position.AngleToPoint(mouse));
    projectileOrigin += AimArm.Position;
    projectileOrigin += Position;

    EmitSignal(
      SignalName.FireProjectile,
      projectile,
      projectileOrigin,
      mouse
    );
  }

  public void HandleFlip(bool isFlipped)
  {
    ArmSprite.FlipH = isFlipped;
    BodySprite.FlipH = isFlipped;
  }

  public void OnAimArmTimerTimeout()
  {
    AimArm.Visible = false;
    ArmSprite.Visible = true;
  }

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    BodySprite = GetNode<Sprite2D>("BodySprite");
    ArmSprite = GetNode<Sprite2D>("ArmSprite");

    AimArm = GetNode<AimArm>("AimArm");
    AimArm.OnAimTimerTimeout += OnAimArmTimerTimeout;
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _PhysicsProcess(double delta)
  {
    MoveAndSlide();
  }
}
