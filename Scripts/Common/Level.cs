using System;
using Centari.Monsters;
using Centari.Navigation;
using Centari.Player;
using Godot;

namespace Centari.Common;

public partial class Level : Node2D
{
  private enum NavState
  {
    NOT_READY,
    NOT_STARTED,
    SET_TILE,
    SET,
  }

  private ProjectileManager _projectileManager;

  private TileMap _tiles;

  private TestBall _ball;

  private NavState didSetNav = NavState.NOT_READY;

  private Vector2I raycastReadyTile;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    base._Ready();

    _projectileManager = GetNode<ProjectileManager>("ProjectileManager");
    _tiles = GetNode<TileMap>("TileMap");
    _ball = GetNode<TestBall>("Ball");
    var player = GetNode<PlayerCharacter>("Player");


    player.FireProjectile += OnPlayerFireProjectile;
    didSetNav = NavState.NOT_STARTED;
  }

  private void OnPlayerFireProjectile(PackedScene projectile, Vector2 origin, Vector2 target)
  {
    Fireball projectileInstance = projectile.Instantiate<Fireball>();
    AddChild(projectileInstance);

    projectileInstance.Position = origin;
    projectileInstance.Velocity = projectileInstance.Velocity.Normalized() * projectileInstance.Speed;
    projectileInstance.Velocity = projectileInstance.Velocity.Rotated(origin.AngleToPoint(target));

    _projectileManager.ManageProjectile(projectileInstance);
  }

  private void OnNavReady()
  {
    var player = GetNode<PlayerCharacter>("Player");
    NavCoordinator nav = new NavCoordinator(_tiles);
    _ball.Prepare(nav, player);
  }

  public override void _Process(double delta)
  {
    switch (didSetNav)
    {
      case NavState.NOT_STARTED:
        {
          Rect2I r = _tiles.GetUsedRect();
          raycastReadyTile = new Vector2I(0, r.End.Y + 1);
          _tiles.SetCell(0, raycastReadyTile, 0, new Vector2I(1, 0));
          didSetNav = NavState.SET_TILE;
          break;
        }
      case NavState.SET_TILE:
        {
          Vector2 tilePos = _tiles.MapToLocal(raycastReadyTile);
          Vector2 rayPos = new Vector2(tilePos.X + _tiles.TileSet.TileSize.X, tilePos.Y);

          var s = GetWorld2D();
          var q = PhysicsRayQueryParameters2D.Create(rayPos, tilePos);
          var r = s.DirectSpaceState.IntersectRay(q);

          if (!r.ContainsKey("position"))
          {
            break;
          }

          var pos = r["position"];
          if (pos.Obj == null)
          {
            break;
          }

          var vec = (Vector2)pos;

          if (vec.Y == tilePos.Y && vec.X == tilePos.X + _tiles.TileSet.TileSize.X / 2)
          {
            _tiles.EraseCell(0, raycastReadyTile);
            OnNavReady();
            didSetNav = NavState.SET;
          }
          break;
        }
      case NavState.SET:
        {
          break;
        }
      default:
        {
          break;
        }
    }
  }
}
