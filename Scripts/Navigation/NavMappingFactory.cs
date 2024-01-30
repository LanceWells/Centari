using System.Collections.Generic;
using Centari.Navigation.Rules;
using Godot;

namespace Centari.Navigation;

public static class NavMappingFactory
{
  public static NavMapping CreateNavMapping(TileMap tiles, INavRules navRules)
  {
    Rect2I rect = tiles.GetUsedRect();
    NavMapping nav = new(rect);

    foreach (Vector2I mapCoords in TileIterator(rect))
    {
      TileData tileData = tiles.GetCellTileData(0, mapCoords);
      Vector2 localCoords = tiles.MapToLocal(mapCoords);
      nav.AddPoint(mapCoords, localCoords, tileData);
    }

    navRules.SetValidPaths(nav, tiles);
    return nav;
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
