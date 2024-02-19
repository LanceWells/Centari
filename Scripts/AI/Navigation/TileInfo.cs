using Godot;

namespace Centari.Navigation;

/// <summary>
/// A helper class used to represent a given tile. Includes some shortcuts to get normalize custom
/// data into meaningful checks.
/// </summary>
public class TileInfo
{
  private TileData _tile;

  private Vector2I _coords;

  /// <summary>
  /// If true, the "CanJumpThrough" custom data is set to true for this given tile.
  /// </summary>
  public bool CanJumpThrough => (bool)(_tile?.GetCustomData("CanJumpThrough") ?? false);

  /// <summary>
  /// If true, the "IsPlatform" custom data is set to true for this given tile.
  /// </summary>
  public bool IsPlatform => (bool)(_tile?.GetCustomData("IsPlatform") ?? false);

  /// <summary>
  /// If true, this tile is considered to be "passable", which means that a navigation agent
  /// considers this tile to be one that a creature can travel through.
  /// </summary>
  public bool IsPassable => CanJumpThrough || !IsPlatform;

  /// <summary>
  /// Gets the map coords that are associated with this given tile.
  /// </summary>
  public Vector2I Coords => _coords;

  /// <summary>
  /// Creates a new instance of a <see cref="TileInfo"/> .
  /// </summary>
  /// <param name="tile">The tile whose info should be wrapped in this instance.</param>
  /// <param name="coords">
  /// The map coords that are associated with this tile. This is mostly used to fetch the coords
  /// later via <see cref="Coords"/>
  /// </param>
  public TileInfo(TileData tile, Vector2I coords)
  {
    _tile = tile;
    _coords = coords;
  }
}
