using Godot;

namespace Centari.Monsters;

public abstract partial class AbstractMonster : CharacterBody2D
{
  private float _gravity = 400;

  private float _walkSpeed = 400;

  [Export]
  public float Gravity
  {
    get => _gravity;
    set => _gravity = value;
  }

  [Export]
  public float WalkSpeed
  {
    get => _walkSpeed;
    set => _walkSpeed = value;
  }

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  { }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  { }

  public virtual Vector2 GetMovement(double delta)
  {
    Vector2 direction = Velocity;

    Vector2 gravity = new Vector2(0, Gravity);
    direction = direction.Lerp(gravity, (float)delta);

    return direction;
  }

  public override void _PhysicsProcess(double delta)
  {
    base._PhysicsProcess(delta);

    Vector2 movement = GetMovement(delta);
    Velocity = movement;

    MoveAndSlide();
  }
}
