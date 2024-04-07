namespace Interfaces.Tiles;

public class InterfaceTile
{
  public static TileDrawingDetails TileDetails
  {
    get => new()
    {
      TileIndex = new(0, 0),
      Layer = 1,
      SourceAtlas = 0,
    };
  }
}
