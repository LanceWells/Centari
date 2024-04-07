namespace Interfaces.Tiles;

public abstract class AbstractTile : ITile
{
  protected abstract TileDrawingDetails TileDetails { get; }

  public TileDrawingDetails GetDrawingDetails()
  {
    return TileDetails;
  }
}
