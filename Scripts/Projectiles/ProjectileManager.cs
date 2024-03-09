using System;
using System.Collections.Generic;
using Centari.Common;
using Godot;

public partial class ProjectileManager : Node
{
  private Pool<AbstractProjectile> _projectilePool;

  private Queue<AbstractProjectile> _projectileQueue = new();

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

  public override void _PhysicsProcess(double delta)
  {
    base._PhysicsProcess(delta);

    while (_projectileQueue.Count > 0)
    {
      AbstractProjectile projectile = _projectileQueue.Dequeue();

      Vector2 velocityOnContact = projectile.Velocity;
      projectile.Velocity = Vector2.Zero;

      CollisionShape2D hitbox = projectile.GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
      hitbox?.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

      Vector2 collisionPosition = projectile.Position;

      Random r = new();

      RigidBody2D shrapnel = projectile.GetNodeOrNull<RigidBody2D>("Shrapnel");

      if (shrapnel != null)
      {
        PackedScene shrapnelScene = GD.Load<PackedScene>(shrapnel.SceneFilePath);
        List<Node> shrapnelNodes = new();

        for (int i = 0; i < r.Next(10, 25); i++)
        {
          RigidBody2D n = shrapnelScene.Instantiate<RigidBody2D>();
          n.Position = collisionPosition;
          n.LinearVelocity = new(
            -velocityOnContact.X * 15 * (r.NextSingle() * 3),
            r.Next(-500, 500)
          );

          shrapnelNodes.Add(n);
          AddChild(n);
        }
      }

      projectile.Collide -= OnCollide;

      // If the projectile had any particles, ensure that they finish particle-ing before we delete
      // them. If we get rid of them outright, it makes any particle trains disappear in a jarring
      // fashion.
      GpuParticles2D particleBurst = projectile.GetNodeOrNull<GpuParticles2D>("Sprites/ExplosionParticles");
      GpuParticles2D trailParticles = projectile.GetNodeOrNull<GpuParticles2D>("Sprites/TrailParticles");
      Sprite2D fireballSprite = projectile.GetNodeOrNull<Sprite2D>("Sprites/Fireball");

      if (trailParticles != null)
      {
        trailParticles.Emitting = false;
      }

      if (particleBurst != null)
      {
        particleBurst.Emitting = true;
      }

      if (fireballSprite != null)
      {
        fireballSprite.Visible = false;
      }

      // There are a few issues with the "Finished" signal for GPU particles. Instead, perform a
      // cleanup some time after we've moved the particles to the level's tree.
      Timer t = new();
      AddChild(t);
      t.OneShot = true;
      t.Timeout += () =>
      {
        RemoveChild(t);
        t.CallDeferred(Node.MethodName.QueueFree);
        projectile.CallDeferred(Node.MethodName.QueueFree);
      };

      t.Start(1);
    }
  }

  private void OnCollide(Node2D collidingBody, AbstractProjectile projectile)
  {
    Console.WriteLine("collision");
    GodotObject collilder = collidingBody;

    if (!collidingBody.IsInGroup("Surfaces"))
    {
      return;
    }

    _projectileQueue.Enqueue(projectile);
  }
}
