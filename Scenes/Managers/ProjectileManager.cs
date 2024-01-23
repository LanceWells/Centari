using System;
using Godot;

public partial class ProjectileManager : Node
{
  private Pool<AbstractProjectile> _projectilePool;

  public override void _Ready()
  {
    _projectilePool = new Pool<AbstractProjectile>(200);
    base._Ready();
  }

  public void ManageProjectile(AbstractProjectile projectile)
  {
    projectile.Collide += OnCollide;
    _projectilePool.AddResource(projectile);
  }

  private void OnCollide(KinematicCollision2D collision, AbstractProjectile projectile)
  {
    Console.WriteLine("collision", collision);
    GodotObject collilder = collision.GetCollider();

    if (((Node)collilder).IsInGroup("Surfaces"))
    {
      Vector2 collisionPosition = collision.GetPosition();
      Vector2 colliderVelocity = collision.GetColliderVelocity();
      Console.WriteLine(collisionPosition);
      Console.WriteLine(colliderVelocity);

      // asplode
      Console.WriteLine(collilder);
      Vector2 velocityAtImpact = collision.GetRemainder();
      Vector2 normalFromImpact = collision.GetNormal();
      float angleAtImpact = collision.GetAngle();

      Console.WriteLine(normalFromImpact.ToString());
      Console.WriteLine(velocityAtImpact.ToString());
      Console.WriteLine(angleAtImpact);
    }

    projectile.Collide -= OnCollide;

    // If the projectile had any particles, ensure that they finish particle-ing before we delete
    // them. If we get rid of them outright, it makes any particle trains disappear in a jarring
    // fashion.
    GpuParticles2D particles = projectile.GetNodeOrNull<GpuParticles2D>("GPUParticles2D");
    if (particles != null)
    {
      projectile.RemoveChild(particles);
      Vector2 p = particles.GlobalPosition;

      AddChild(particles);
      particles.Position = p;

      particles.OneShot = true;
      particles.Emitting = false;

      // There are a few issues with the "Finished" signal for GPU particles. Instead, perform a
      // cleanup some time after we've moved the particles to the level's tree.
      Timer t = new Timer();
      AddChild(t);
      t.OneShot = true;
      t.Timeout += () =>
      {
        RemoveChild(particles);
        particles.QueueFree();

        RemoveChild(t);
        t.QueueFree();
      };

      t.Start(0.5);
    }

    projectile.CallDeferred(Node.MethodName.QueueFree);
  }
}
