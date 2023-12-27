using Godot;

public partial class Player : CharacterBody2D
{
  [Export]
  public float MaxSpeed = 14.0f;

  [Export]
  public float Friction = 0.75f;

  public bool isAiming = false;

  public Sprite2D BodySprite;

  private Sprite2D ArmSprite;

  private Sprite2D AimSprite;

  private Timer AimArmTimer;

  [Signal]
  public delegate void FireProjectileEventHandler(
  PackedScene projectile,
  Vector2 origin,
  Vector2 target
  );

  public void HandleFireProjectile(PackedScene projectile)
  {
    isAiming = true;
    ArmSprite.Visible = false;
    AimSprite.Visible = true;
    AimArmTimer.Start();

    Vector2 mouse = GetViewport().GetMousePosition();

    EmitSignal(
      SignalName.FireProjectile,
      projectile,
      Position,
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
    isAiming = false;
    ArmSprite.Visible = true;
    AimSprite.Visible = false;
  }

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    BodySprite = GetNode<Sprite2D>("BodySprite");
    ArmSprite = GetNode<Sprite2D>("ArmSprite");
    AimSprite = GetNode<Sprite2D>("AimSprite");
    AimArmTimer = GetNode<Timer>("AimArmTimer");

    AimArmTimer.Timeout += OnAimArmTimerTimeout;
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
    if (isAiming)
    {
      Vector2 mousePos = GetViewport().GetMousePosition();
      AimSprite.LookAt(mousePos);
    }
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _PhysicsProcess(double delta)
  {
    // https://godotengine.org/qa/48404/make-character-face-cursors-direction-even-when-moving-around
    MoveAndSlide();
  }
}
