using Centari.Navigation;
using Godot;

namespace Centari.Monsters;

public abstract partial class AbstractMonster : CharacterBody2D, IMonster
{
  private float _gravity = 400;

  private float _walkSpeed = 200;

  private float _meleeRange = 30;

  private float _projectileRange = 100;

  private AnimationPlayer _animationPlayer;

  private Sprite2D _sprite;

  private NavModes[] _navModes = new NavModes[]
  {
    NavModes.Cat,
  };

  public float Gravity
  {
    get => _gravity;
    set => _gravity = value;
  }

  public float WalkSpeed
  {
    get => _walkSpeed;
    set => _walkSpeed = value;
  }

  public float MeleeRange
  {
    get => _meleeRange;
    set => _meleeRange = value;
  }

  public float ProjectileRange
  {
    get => _projectileRange;
    set => _projectileRange = value;
  }

  public NavModes[] NavOptions
  {
    get => _navModes;
  }

  public abstract Rect2 HitBox
  {
    get;
  }

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    _animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
    _sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
    SetAnimation(MonsterAnimation.Idle, false);
  }

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

  public virtual void SetAnimation(MonsterAnimation animation, bool isFlipped)
  {
    if (_sprite != null && isFlipped != _sprite.FlipH)
    {
      _sprite.FlipH = isFlipped;
    }

    if (_animationPlayer == null)
    {
      return;
    }

    string currentAnimation = _animationPlayer.CurrentAnimation;
    string nextAnimation = animation switch
    {
      MonsterAnimation.Walk => "Walk",
      MonsterAnimation.Idle => "Idle",
      MonsterAnimation.Run => "Run",
      _ => "Idle"
    };

    if (currentAnimation != nextAnimation)
    {
      _animationPlayer.Play(nextAnimation);
    }
  }

  protected static Rect2 CalcBoundingBox(CollisionShape2D collision)
  {
    Shape2D shape = collision.Shape;

    if (shape is CapsuleShape2D capsule)
    {
      Vector2 p1 = new(-capsule.Radius, -capsule.Height / 2);
      Vector2 p2 = new(capsule.Radius, capsule.Height / 2);

      p1 = p1.Rotated(collision.Rotation);
      p2 = p2.Rotated(collision.Rotation);

      return new Rect2(p1, p2);
    }

    return new Rect2();
  }

  public override void _PhysicsProcess(double delta)
  {
    base._PhysicsProcess(delta);

    Vector2 movement = GetMovement(delta);
    Velocity = movement;

    MoveAndSlide();
  }
}
