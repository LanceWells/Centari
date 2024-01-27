using Godot;
using System;
using System.Collections.Generic;

public partial class Level : Node2D
{
  private ProjectileManager _projectileManager;

  private TileMap _tiles;

  private AStar2D nav;

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

    _projectileManager = GetNode<ProjectileManager>("ProjectileManager");
    _tiles = GetNode<TileMap>("TileMap");

    // Rect2I usedRect = _tiles.GetUsedRect();
    // int mapHeight = usedRect.End.Y * _tiles.TileSet.TileSize.Y;

    // nav = new AStar2D();

    // var usedCells = _tiles.GetUsedCells(0);
    // foreach (Vector2I tileCoords in usedCells)
    // {
    //   TileData tile = _tiles.GetCellTileData(0, tileCoords);
    //   Vector2I aboveNeighborCoords = _tiles.GetNeighborCell(tileCoords, TileSet.CellNeighbor.TopSide);
    //   TileData aboveNeighbor = _tiles.GetCellTileData(0, aboveNeighborCoords);

    //   bool thisCanJumpThrough = true;
    //   bool thisIsPlatform = false;

    //   if (tile != null)
    //   {
    //     thisCanJumpThrough = (bool)tile.GetCustomData("CanJumpThrough");
    //     thisIsPlatform = (bool)tile.GetCustomData("IsPlatform");
    //   }

    //   bool aboveCanJumpThrough = true;
    //   bool aboveIsPlatform = false;

    //   if (aboveNeighbor != null)
    //   {
    //     aboveCanJumpThrough = (bool)aboveNeighbor.GetCustomData("CanJumpThrough");
    //     aboveIsPlatform = (bool)aboveNeighbor.GetCustomData("IsPlatform");
    //   }

    //   if (thisIsPlatform && (aboveCanJumpThrough || !aboveIsPlatform))
    //   {
    //     Vector2 worldCoords = _tiles.MapToLocal(tileCoords);
    //     int tileIndex = GetTileIndex(worldCoords.X, worldCoords.Y, mapHeight);
    //     nav.AddPoint(tileIndex, worldCoords);
    //   }

    //   Console.WriteLine("testing");
    // }

    // nav = new AStarGrid2D
    // {
    //   Region = usedRect,
    //   CellSize = _tiles.TileSet.TileSize
    // };

    // nav.Update();

    // Godot.Collections.Array<Vector2I> usedCells = _tiles.GetUsedCells(0);
    // foreach (Vector2I tileCoords in usedCells)
    // {
    //   TileData tile = _tiles.GetCellTileData(0, tileCoords);
    //   Console.WriteLine("testing");
    // }

    // nav = new AStar2D();

    // Rect2I usedRect = _tiles.GetUsedRect();
    // Vector2I tileSize = _tiles.TileSet.TileSize;

    // for (int i = usedRect.Position.X; i < usedRect.End.X; i += tileSize.X)
    // {
    //   for (int j = usedRect.Position.Y; j < usedRect.End.Y; j += tileSize.Y)
    //   {
    //     int tileIndex = GetTileIndex(i, j, usedRect.Size.Y);
    //     nav.AddPoint(tileIndex, new Vector2(i, j));
    //   }
    // }

    // // _tiles.GetSurroundingCells()

    // for (int i = usedRect.Position.X; i < usedRect.End.X; i += tileSize.X)
    // {
    //   for (int j = usedRect.Position.Y; j < usedRect.End.Y; j += tileSize.Y)
    //   {
    //     Vector2I coords = new(i, j);

    //     int tileIndex = GetTileIndex(i, j, usedRect.Size.Y);
    //     TileData tile = _tiles.GetCellTileData(0, coords);

    //     // int tileIndex = GetTileIndex(i, j, usedRect.Size.Y);
    //     // TileData tile = _tiles.GetCellTileData(0, coords);

    //     // List<Vector2I> neighbors = new List<Vector2I> {
    //     //   _tiles.GetNeighborCell(coords, TileSet.CellNeighbor.LeftSide),
    //     //   _tiles.GetNeighborCell(coords, TileSet.CellNeighbor.RightSide),
    //     // };

    //     if (tile == null)
    //     {
    //       // Empty space, we can go there (in theory)!
    //       // Also verify that there is a tile beneath this one.

    //       _tiles.Local
    //     }
    //   }
    // }

    Console.WriteLine("test");
    // foreach (Vector2I cell in usedCells)
    // {
    //   cell.
    // }
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
