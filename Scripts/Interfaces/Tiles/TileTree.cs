using System;
using System.Collections.Generic;
using Godot;
using Interfaces.Tiles;

namespace Centari.Interfaces.Tiles;

public class TileTree
{
  private ITile[][] _tiles;

  public TileTree(int width, int height)
  {
    _tiles = new ITile[height][];
    for (int i = 0; i < height; i++)
    {
      _tiles[i] = new ITile[width];
      for (int j = 0; j < width; j++)
      {
        _tiles[i][j] = new EmptyTile();
      }
    }

    for (int i = 0; i < height; i++)
    {
      for (int j = 0; j < width; j++)
      {
        if (i > 0)
        {

        }
        if (i < height - 1)
        {

        }
        if (j > 0)
        {

        }
        if (j < width - 1)
        {

        }
        // top
        // left
        // right
        // down
      }
    }
  }
}

public struct MetalSection
{
  public List<ITile> Tiles;

  public Vector2I Fulcrum;
}

public interface ITile
{
  public TileDrawingDetails GetTileDrawingDetails();

  public int Heat
  {
    get;
    set;
  }

  public List<ITile> Neighbors
  {
    get;
    set;
  }
}

public class EmptyTile : Tile
{

}

public abstract class Tile : ITile
{
  private int _heat;

  private List<ITile> _neighbors;

  public Tile()
  { }

  public int Heat
  {
    get => _heat;
    set => _heat = value;
  }

  public List<ITile> Neighbors
  {
    get => _neighbors;
    set => _neighbors = value;
  }

  public TileDrawingDetails GetTileDrawingDetails()
  {
    throw new NotImplementedException();
  }
}

/*

Determine if the given hammered tile is heated or not.

If it is heated:
  * This means that we should morph the heated section.
Else if we do a BFS and we reach a heated section,
  And we get the entire item minus that heated section
    * This means that we should squash the heated section.
  Else
    * This means that we should rotate the found section along the heated,
      section using it as a fulcrum.

 */
