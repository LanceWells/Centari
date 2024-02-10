using System;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using Godot;

namespace Centari.Navigation.Rules;

public class SnailNavRules : AbstractNavRules
{
  public override void SetValidPaths(NavMapping nav, TileMap tiles)
  {
    var world = tiles.GetWorld2D();
    var cellCoords = tiles.GetUsedCells(0);
    foreach (Vector2I coords in cellCoords)
    {
      TileInfo
        thisTile = nav.GetTile(coords),
        up = nav.Neighbor(coords, new Vector2I(0, -1)),
        upUp = nav.Neighbor(coords, new Vector2I(0, -2)),
        upUpRight = nav.Neighbor(coords, new Vector2I(1, -2)),
        upUpLeft = nav.Neighbor(coords, new Vector2I(-1, -2)),
        upLeft = nav.Neighbor(coords, new Vector2I(-1, -1)),
        upRight = nav.Neighbor(coords, new Vector2I(1, -1)),
        down = nav.Neighbor(coords, new Vector2I(0, 1)),
        downDown = nav.Neighbor(coords, new Vector2I(0, 2)),
        downLeft = nav.Neighbor(coords, new Vector2I(-1, 1)),
        downRight = nav.Neighbor(coords, new Vector2I(1, 1)),
        right = nav.Neighbor(coords, new Vector2I(1, 0)),
        rightRight = nav.Neighbor(coords, new Vector2I(2, 0)),
        left = nav.Neighbor(coords, new Vector2I(-1, 0)),
        leftLeft = nav.Neighbor(coords, new Vector2I(-2, 0));

      if (!thisTile.IsPlatform || !up.IsPassable)
      {
        continue;
      }

      var lJumpVects = RectToVect(
        new Rect2I(-1, -5, 2, 4)
      ).Concat(RectToVect(
        new Rect2I(-2, -5, 1, 2)
      ));

      var rJumpVects = RectToVect(
        new Rect2I(0, -5, 2, 4)
      ).Concat(RectToVect(
        new Rect2I(2, -5, 1, 2)
      ));

      // Platformer logic
      // ================

      // ._.
      // /.\
      // =.|
      // ?.|
      // ??|
      // ==X

      var leftJumpBoxV = lJumpVects.Select((vect) => nav.Neighbor(coords, vect)).ToList();
      var rightJumpBoxV = rJumpVects.Select((vect) => nav.Neighbor(coords, vect)).ToList();

      if (right.IsPlatform && upRight.IsPassable)
      {
        nav.ConnectPoints(up.Coords, upRight.Coords);
      }

      if (left.IsPlatform && upLeft.IsPassable)
      {
        nav.ConnectPoints(up.Coords, upLeft.Coords);
      }

      var leftLanding = nav.Neighbor(coords, new Vector2I(-2, -3));
      if (leftJumpBoxV.TrueForAll((vect) => vect.IsPassable) && leftLanding.IsPlatform)
      {
        nav.ConnectPoints(up.Coords, leftLanding.Coords + new Vector2I(0, -1));
      }

      var rightLanding = nav.Neighbor(coords, new Vector2I(2, -3));
      if (rightJumpBoxV.TrueForAll((vect) => vect.IsPassable) && rightLanding.IsPlatform)
      {
        nav.ConnectPoints(up.Coords, rightLanding.Coords + new Vector2I(0, -1));
      }

      if (left.IsPassable && upLeft.IsPassable)
      {
        var spaceState = world.DirectSpaceState;

        // use global coordinates, not local to node
        Vector2 fromRay = tiles.MapToLocal(left.Coords);
        Vector2 ToRay = tiles.MapToLocal(new Vector2I(left.Coords.X, left.Coords.Y + 25));
        var query = PhysicsRayQueryParameters2D.Create(fromRay, ToRay, 1);
        query.CollideWithBodies = true;
        var result = spaceState.IntersectRay(query);

        if (result.Keys.Count > 0)
        {
          Console.WriteLine(result);
        }

        if (result.ContainsKey("position"))
        {
          Vector2I landingTileCoords = tiles.LocalToMap((Vector2)result["position"].Obj);
          Vector2I landingTileCoordsSpace = new Vector2I(landingTileCoords.X, landingTileCoords.Y - 1);
          // nav.ConnectPoints(up.Coords, upLeft.Coords, true);
          // nav.ConnectPoints(upLeft.Coords, landingTileCoordsSpace);
          nav.ConnectPoints(up.Coords, landingTileCoordsSpace);
        }
      }

      if (right.IsPassable && upRight.IsPassable)
      {
        var spaceState = world.DirectSpaceState;

        // use global coordinates, not local to node
        Vector2 fromRay = tiles.MapToLocal(right.Coords);
        Vector2 ToRay = tiles.MapToLocal(new Vector2I(right.Coords.X, right.Coords.Y + 25));
        var query = PhysicsRayQueryParameters2D.Create(fromRay, ToRay, 1);
        query.CollideWithBodies = true;
        var result = spaceState.IntersectRay(query);

        if (result.Keys.Count > 0)
        {
          Console.WriteLine(result);
        }

        if (result.ContainsKey("position"))
        {
          Vector2I landingTileCoords = tiles.LocalToMap((Vector2)result["position"].Obj);
          Vector2I landingTileCoordsSpace = new Vector2I(landingTileCoords.X, landingTileCoords.Y - 1);
          nav.ConnectPoints(up.Coords, landingTileCoordsSpace);
          // nav.ConnectPoints(up.Coords, upRight.Coords, true);
          // nav.ConnectPoints(upRight.Coords, landingTileCoordsSpace);
        }
      }

      // There is a tile to the right
      // There is space above the tile to the right
      // ->> Connect tile above to tile above to the right.

      // There is a tile to the left
      // There is space above the tile to the left
      // ->> Connect tile above to tile above to the left.

      // There is a tile above and to the right
      // There is space above that tile
      // There is a space two above this tile
      // ->> Connect tile above to that tile

      // There is a tile above and to the left
      // There is space above that tile
      // ->> Connect tile above to that tile

      // There is no tile to the right
      // Continue to look down until we run out of points
      // If we find a tile down there, connect it.

      // There is no tile to the left
      // Continue to look down until we run out of points
      // If we find a tile down there, connect it.
    }
  }
}
