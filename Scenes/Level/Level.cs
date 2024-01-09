using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class Level : Node2D
{
  private ProjectileManager _projectileManager;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    base._Ready();

    Player player = GetNode<Player>("Player");
    player.FireProjectile += OnPlayerFireProjectile;

    _projectileManager = GetNode<ProjectileManager>("ProjectileManager");
    Console.WriteLine("test");
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
  }

  private async void OnPlayerFireProjectile(PackedScene projectile, Vector2 origin, Vector2 target)
  {
    Fireball projectileInstance = projectile.Instantiate<Fireball>();
    AddChild(projectileInstance);

    projectileInstance.Position = origin;
    // projectileInstance.Rotation = origin.AngleToPoint(target);
    projectileInstance.Velocity = projectileInstance.Velocity.Normalized() * projectileInstance.Speed;
    projectileInstance.Velocity = projectileInstance.Velocity.Rotated(origin.AngleToPoint(target));

    _projectileManager.ManageProjectile(projectileInstance);

    // await Task.Run(async () =>
    // {
    //   // await Task.Delay(50);
    //   // Vector2 mouse = GetViewport().GetMousePosition();
    //   // Vector2 vChange = projectileInstance.Velocity.Lerp(mouse, 0.5f);
    //   // projectileInstance.Velocity += vChange;
    // });
  }
}
