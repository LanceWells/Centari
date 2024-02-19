using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Centari.Navigation;

public static class NavAgentFactory
{
  private struct NavPatternGrid
  {
    public Vector2I startPoint;

    public Vector2I? endPoint;

    public List<Vector2I> emptyPoints;

    public List<Vector2I> platformPoints;

    public List<Vector2I> raycastPoints;

    public string id;
  }

  public static NavAgent Create(
    TileMap tiles,
    IEnumerable<NavPattern> patterns
  )
  {
    Rect2I rect = tiles.GetUsedRect();
    NavAgent nav = new(rect);

    foreach (Vector2I mapCoords in TileIterator(rect))
    {
      TileData tileData = tiles.GetCellTileData(0, mapCoords);
      Vector2 localCoords = tiles.MapToLocal(mapCoords);
      nav.AddPoint(mapCoords, localCoords, tileData);
    }

    var patternGrids = patterns.SelectMany((p) =>
    {
      static NavPatternGrid patternToGrid(NavPathItem[][] path, string id) =>
      new()
      {
        startPoint = GetPoint(path, NavPathItem.PathStart) ?? new Vector2I(),
        endPoint = GetPoint(path, NavPathItem.PathEnd),
        emptyPoints = GetPoints(path, NavPathItem.EmptySpace),
        platformPoints = GetPoints(path, NavPathItem.Platform),
        raycastPoints = GetPoints(path, NavPathItem.RaycastDown),
        id = id,
      };

      List<NavPatternGrid> grids = new()
      {
        patternToGrid(p.path, p.id)
      };

      if (p.mirror)
      {
        NavPathItem[][] reversedPattern = p.path.Select((path) => path.Reverse().ToArray()).ToArray();
        grids.Add(patternToGrid(reversedPattern, p.id));
      }

      return grids;
    }).Where((p) =>
    {
      if (!p.endPoint.HasValue && p.raycastPoints.Count == 0)
      {
        return false;
      }

      return true;
    });

    var world = tiles.GetWorld2D();
    var cellCoords = tiles.GetUsedCells(0);
    var spaceState = world.DirectSpaceState;

    foreach (Vector2I coords in cellCoords)
    {
      TileInfo thisTile = nav.GetTile(coords);

      foreach (NavPatternGrid grid in patternGrids)
      {
        // Assume that "thisTile" is one below the start tile.
        TileInfo startTile = nav.Neighbor(thisTile.Coords, new Vector2I(0, -1));

        if (!startTile.IsPassable)
        {
          continue;
        }

        // Any tile that was marked as needing to be empty should be "passable". The point is that
        // any tile can exist in this space, but we must be able to walk through it.
        bool pointsPassable = grid.emptyPoints.All((p) =>
        {
          Vector2I relativePoint = p - grid.startPoint;
          TileInfo emptyTile = nav.Neighbor(startTile.Coords, relativePoint);
          return emptyTile.IsPassable;
        });

        if (!pointsPassable)
        {
          continue;
        }

        // Any movement that require a platform to stand on needs to ensure that the associated tile
        // is a functioning platform.
        bool pointsPlatforms = grid.platformPoints.All((p) =>
        {
          Vector2I relativePoint = p - grid.startPoint;
          TileInfo platformTile = nav.Neighbor(startTile.Coords, relativePoint);
          return platformTile.IsPlatform;
        });

        if (!pointsPlatforms)
        {
          continue;
        }

        // If we have an endpoint, then test just that one point to see if it's empty. If it is,
        // connect the points (we have a path!). Otherwise, try the next pattern.
        if (grid.endPoint.HasValue)
        {
          Vector2I relativePoint = grid.endPoint.Value - grid.startPoint;
          TileInfo endPoint = nav.Neighbor(startTile.Coords, relativePoint);

          if (endPoint.IsPassable)
          {
            nav.ConnectPoints(startTile.Coords, endPoint.Coords);
          }
        }

        foreach (Vector2I v in grid.raycastPoints)
        {
          Vector2I relativePoint = v - grid.startPoint;
          TileInfo raycastPoint = nav.Neighbor(startTile.Coords, relativePoint);

          if (!raycastPoint.IsPassable)
          {
            continue;
          }

          Vector2 fromRay = tiles.MapToLocal(raycastPoint.Coords);
          Vector2 toRay = tiles.MapToLocal(new Vector2I(
            raycastPoint.Coords.X,
            raycastPoint.Coords.Y + 25
          ));

          using var query = PhysicsRayQueryParameters2D.Create(fromRay, toRay, 1);
          query.CollideWithBodies = true;

          using var result = spaceState.IntersectRay(query);
          if (!result.ContainsKey("position"))
          {
            continue;
          }

          Vector2I landingTileCoords = tiles.LocalToMap((Vector2)result["position"].Obj);
          Vector2I landingTileCoordsSpace = new(
            landingTileCoords.X,
            landingTileCoords.Y - 1
          );

          nav.ConnectPoints(startTile.Coords, raycastPoint.Coords);
          nav.ConnectPoints(raycastPoint.Coords, landingTileCoordsSpace);
        }
      }
    }

    return nav;
  }

  private static Vector2I? GetPoint(NavPathItem[][] path, NavPathItem pointType)
  {
    for (int y = 0; y < path.Length; y++)
    {
      for (int x = 0; x < path[y].Length; x++)
      {
        if (path[y][x] == pointType)
        {
          return new Vector2I(x, y);
        }
      }
    }

    return null;
  }

  private static List<Vector2I> GetPoints(NavPathItem[][] path, NavPathItem pointType)
  {
    List<Vector2I> points = new();
    for (int y = 0; y < path.Length; y++)
    {
      for (int x = 0; x < path[y].Length; x++)
      {
        if (path[y][x] == pointType)
        {
          points.Add(new Vector2I(x, y));
        }
      }
    }

    return points;
  }

  private static IEnumerable<Vector2I> TileIterator(Rect2I usedRect)
  {
    for (int i = usedRect.Position.X; i < usedRect.End.X; i++)
    {
      for (int j = usedRect.Position.Y; j < usedRect.End.Y; j++)
      {
        yield return new Vector2I(i, j);
      }
    }
  }
}
