using Godot;

namespace Centari.Navigation.Rules;

public class SnailNavRules : INavRules
{
  public void SetValidPaths(NavMapping nav, TileMap tiles)
  {
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

      if (right.IsPlatform && upRight.IsPassable)
      {
        nav.ConnectPoints(up.Coords, upRight.Coords);
      }

      if (left.IsPlatform && upLeft.IsPassable)
      {
        nav.ConnectPoints(up.Coords, upLeft.Coords);
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
