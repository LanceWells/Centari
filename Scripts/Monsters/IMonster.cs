using Godot;

namespace Centari.Monsters;

public enum MonsterAnimation
{
  Idle,
  Walk,
  Run,
}

public interface IMonster
{
  [Export]
  public float Gravity
  {
    get;
    set;
  }

  [Export]
  public float WalkSpeed
  {
    get;
    set;
  }

  [Export]
  public float MeleeRange
  {
    get;
    set;
  }

  [Export]
  public float ProjectileRange
  {
    get;
    set;
  }

  public CollisionShape2D HitBox
  {
    get;
  }


  public Vector2 GetMovement(double delta);

  public void SetAnimation(MonsterAnimation animation, bool isFlipped);
}
