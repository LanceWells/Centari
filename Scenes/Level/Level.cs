using Godot;
using System;

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

  private void OnPlayerFireProjectile(PackedScene projectile, Vector2 origin, Vector2 target, float velocity)
  {
    // Replace with function body.
    Console.WriteLine("Fire");

    Firebolt projectileInstance = projectile.Instantiate<Firebolt>();
    AddChild(projectileInstance);

    projectileInstance.Position = origin;
    projectileInstance.Rotation = origin.AngleToPoint(target);
    projectileInstance.Velocity = projectileInstance.Velocity.Rotated(projectileInstance.Rotation);

    _projectileManager.ManageProjectile(projectileInstance);
  }
}
