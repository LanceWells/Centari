using Godot;

namespace Centari.Navigation;

public class TileInfo
{
  private TileData _tile;

  private Vector2I _coords;

  public bool CanJumpThrough => (bool)(_tile?.GetCustomData("CanJumpThrough") ?? false);

  public bool IsPlatform => (bool)(_tile?.GetCustomData("IsPlatform") ?? false);

  public bool IsPassable => CanJumpThrough || !IsPlatform;

  public Vector2I Coords => _coords;

  public TileInfo(TileData tile, Vector2I coords)
  {
    _tile = tile;
    _coords = coords;
  }
}
