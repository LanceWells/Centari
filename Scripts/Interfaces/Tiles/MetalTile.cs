namespace Interfaces.Tiles;

public class MetalTile
{
  public static TileDrawingDetails TileDetails
  {
    get => new()
    {
      TileIndex = new(0, 0),
      Layer = 0,
      SourceAtlas = 2,
    };
  }
}
