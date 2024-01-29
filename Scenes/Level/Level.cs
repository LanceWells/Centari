using Godot;
using System;

public partial class Level : Node2D
{
  private ProjectileManager _projectileManager;

  private MonsterManager _monsterManager;

  private LevelTiles _tiles;

  private CharacterBody2D _ball;

  private int GetTileIndex(float x, float y, float MapHeight)
  {
    return (int)(y + (x * MapHeight));
  }

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    base._Ready();

    Player player = GetNode<Player>("Player");
    player.FireProjectile += OnPlayerFireProjectile;

    _tiles = GetNode<LevelTiles>("TileMap");
    _ball = GetNode<CharacterBody2D>("Ball");

    _projectileManager = GetNode<ProjectileManager>("ProjectileManager");
    _monsterManager = GetNode<MonsterManager>("MonsterManager");
    _monsterManager.Prepare(player, _tiles);
    _monsterManager.RegisterMonster(_ball);

    Console.WriteLine("test");
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
