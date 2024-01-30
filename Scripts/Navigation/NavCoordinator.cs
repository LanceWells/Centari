using Centari.Navigation.Rules;
using Godot;

namespace Centari.Navigation;

public class NavCoordinator
{
  private NavMapping _snailNav;

  public NavCoordinator(TileMap tiles)
  {
    _snailNav = NavMappingFactory.CreateNavMapping(tiles, new SnailNavRules());
  }

  public Vector2[] GetPath(NavModes[] navModes, Vector2 from, Vector2 to)
  {
    Vector2[] path = null;

    foreach (NavModes mode in navModes)
    {
      Vector2[] thisPath = System.Array.Empty<Vector2>();

      switch (mode)
      {
        case NavModes.SNAIL:
          thisPath = _snailNav.GetPointConnection(from, to);
          break;
        default:
          thisPath = _snailNav.GetPointConnection(from, to);
          break;
      }

      if (path == null || thisPath.Length < path.Length)
      {
        path = thisPath;
      }
    }

    return path ?? System.Array.Empty<Vector2>();
  }
}
