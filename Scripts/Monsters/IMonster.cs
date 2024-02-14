using Godot;

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

  public Vector2 GetMovement(double delta);
}
