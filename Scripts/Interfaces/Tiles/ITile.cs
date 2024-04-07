using Godot;

namespace Interfaces.Tiles;

public struct TileDrawingDetails
{
  public int Layer;

  public int SourceAtlas;

  public Vector2I TileIndex;
}

public interface ITile
{
  public TileDrawingDetails GetDrawingDetails();
}
