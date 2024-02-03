using System.Collections.Generic;
using Centari.Navigation;
using Centari.Navigation.Rules;
using Godot;

namespace Centari.Navigation.Rules;

public abstract class AbstractNavRules : INavRules
{
  abstract public void SetValidPaths(NavMapping nav, TileMap tiles);

  public List<Vector2I> RectToVect(Rect2I rect)
  {
    Vector2I start = rect.Position;
    Vector2I end = rect.End;

    List<Vector2I> vects = new();

    for (int i = start.X; i < end.X; i++)
    {
      for (int j = start.Y; j < end.Y; j++)
      {
        vects.Add(new Vector2I(i, j));
      }
    }

    return vects;
  }
}