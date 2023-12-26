using System;
using Godot;

public partial class ProjectileManager : Node
{
  private Pool<Node> _projectilePool;

  public override void _Ready()
  {
    _projectilePool = new Pool<Node>(200);
    base._Ready();
  }

  public void ManageProjectile(Node projectile)
  {
    _projectilePool.AddResource(projectile);
  }
}
