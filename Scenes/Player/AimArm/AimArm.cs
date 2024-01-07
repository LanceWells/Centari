using Godot;

public partial class AimArm : Node2D
{
  public bool IsAiming = false;

  public Sprite2D ArmSprite;

  private Timer AimTimer;

  private AnimationPlayer Animation;

  private Timer FadeTimer;

  private GpuParticles2D _smokeParticles;

  [Signal]
  public delegate void OnAimTimerTimeoutEventHandler();

  public void StartAimStance()
  {
    IsAiming = true;

    AimTimer.Stop();
    AimTimer.Start();

    FadeTimer.Stop();
    FadeTimer.Start(AimTimer.WaitTime - 0.5);

    Animation.Stop();
    Animation.Play("Fire");
    _smokeParticles.Emitting = true;
  }

  private void _onFadeTimerTimeout()
  {
    Animation.Play("FadeOut");
  }

  private void _onAimTimerTimeout()
  {
    IsAiming = false;
    EmitSignal(SignalName.OnAimTimerTimeout);
  }

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    _smokeParticles = GetNode<GpuParticles2D>("SmokeParticles");

    ArmSprite = GetNode<Sprite2D>("ArmSprite");
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
