using Centari.Monsters;
using Centari.Navigation;
using Centari.Player;
using Godot;

namespace Centari.Common;

public partial class Level : Node2D
{
  private ProjectileManager _projectileManager;

  private TileMap _tiles;

  private TestBall _ball;

  private bool didSetNav = false;

  private int GetTileIndex(float x, float y, float MapHeight)
  {
    return (int)(y + (x * MapHeight));
  }

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    base._Ready();

    _projectileManager = GetNode<ProjectileManager>("ProjectileManager");
    _tiles = GetNode<TileMap>("TileMap");
    _ball = GetNode<TestBall>("Ball");
    var player = GetNode<PlayerCharacter>("Player");

    NavCoordinator nav = new NavCoordinator(_tiles);

    player.FireProjectile += OnPlayerFireProjectile;

    _ball.Prepare(nav, player);
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  { }

  private void OnPlayerFireProjectile(PackedScene projectile, Vector2 origin, Vector2 target)
  {
    Fireball projectileInstance = projectile.Instantiate<Fireball>();
    AddChild(projectileInstance);

    projectileInstance.Position = origin;
    projectileInstance.Velocity = projectileInstance.Velocity.Normalized() * projectileInstance.Speed;
    projectileInstance.Velocity = projectileInstance.Velocity.Rotated(origin.AngleToPoint(target));

    _projectileManager.ManageProjectile(projectileInstance);
  }
}
