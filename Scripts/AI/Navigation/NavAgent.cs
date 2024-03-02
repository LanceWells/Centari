using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Centari.Navigation;

/// <summary>
/// This agent is used as a reference to a given movement mode. Any creature can implement several
/// modes of movement, but each must be discrete. For example, if a movement mode indicates that a
/// creature can walk left-to-right, then that cannot path into a flying mode.
/// 
/// The intention is that, for example, a creature can walk directly towards a target, or if they
/// have the ability to fly they are able to do so if walking provides a less ideal path.
/// 
/// It's important to note that the nav agent is a wrapper around an <see cref="AStar2D"/> Godot
/// object, but designed to work more easily with a platformer-oriented tilemap.
/// </summary>
public class NavAgent
{
  private readonly AStar2D _nav;

  private Rect2I _rect;

  private Dictionary<long, long> _tileIdToCoord;

  private Dictionary<long, long> _coordToTileId;

  private Dictionary<long, TileInfo> _coordToTileInfo;

  /// <summary>
  /// Creates a new instance of a <see cref="NavAgent"/>.
  /// </summary>
  /// <param name="rect">The map-coordinates rectangle occupied by the associated TileMap.</param>
  public NavAgent(Rect2I rect)
  {
    _nav = new();
    _tileIdToCoord = new();
    _coordToTileId = new();
    _coordToTileInfo = new();

    _rect = rect;
  }

  /// <summary>
  /// Adds a point to the underlying AStar2D implementation.
  /// </summary>
  /// <param name="mapPosition">The map coordinates for the given point.</param>
  /// <param name="localPosition">The local coordinates for the given point.</param>
  /// <param name="tileData">The tiledata associated with the given point.</param>
  /// <param name="weightScale">The weight scale to use for the given point.</param>
  public void AddPoint(
    Vector2I mapPosition,
    Vector2 localPosition,
    TileData tileData,
    float weightScale = 1
  )
  {
    long tileId = _nav.GetAvailablePointId();
    TileInfo tileInfo = new(tileData, mapPosition);

    _tileIdToCoord.Add(tileId, GetTileIndex(mapPosition));
    _coordToTileId.Add(GetTileIndex(mapPosition), tileId);
    _coordToTileInfo.Add(GetTileIndex(mapPosition), tileInfo);

    _nav.AddPoint(tileId, localPosition, weightScale);
  }

  /// <summary>
  /// Connects two points for use with navigation in the AStar2D implementation.
  /// </summary>
  /// <param name="fromPos">The "from" position to connect.</param>
  /// <param name="toPos">The "to" position to connect.</param>
  /// <param name="bidirectional">If true, this connection is bi-directional.</param>
  public void ConnectPoints(Vector2I fromPos, Vector2I toPos, bool bidirectional = false)
  {
    bool hasId1 = _coordToTileId.TryGetValue(GetTileIndex(fromPos), out long id1);
    bool hasId2 = _coordToTileId.TryGetValue(GetTileIndex(toPos), out long id2);

    if (hasId1 && hasId2)
    {
      _nav.ConnectPoints(id1, id2, bidirectional);
    }
  }

  /// <summary>
  /// Gets the tile for the given map-coordinates.
  /// </summary>
  /// <param name="mapPos">The map coordinates for the tile to fetch.</param>
  /// <returns></returns>
  public TileInfo GetTile(Vector2I mapPos)
  {
    bool hasTile = _coordToTileInfo.TryGetValue(GetTileIndex(mapPos), out TileInfo tileInfo);

    if (hasTile)
    {
      return tileInfo;
    }

    return new TileInfo(null, new Vector2I(-1, -1));
  }

  /// <summary>
  /// Gets the neighbor of a given tile. This is mostly just a safety check around fetching a
  /// neighbor tile to ensure that we're fine if we look outside of the tilemap.
  /// </summary>
  /// <param name="thisPos"></param>
  /// <param name="relativePos"></param>
  /// <returns></returns>
  public TileInfo Neighbor(Vector2I thisPos, Vector2I relativePos)
  {
    Vector2I neighborPos = thisPos + relativePos;
    bool hasTile = _coordToTileInfo.TryGetValue(GetTileIndex(neighborPos), out TileInfo tileInfo);

    if (hasTile)
    {
      return tileInfo;
    }

    return new TileInfo(null, new Vector2I(-1, -1));
  }

  /// <summary>
  /// Clears the entire underlying AStar2D navigation. Shouldn't be used too often.
  /// </summary>
  public void Clear()
  {
    _nav.Clear();
  }

  /// <summary>
  /// Gets a point connection between two points.
  /// </summary>
  /// <param name="from">The "from" point.</param>
  /// <param name="to">The "to" point.</param>
  /// <returns>The connection, if any.</returns>
  public Vector2[] GetPointConnection(Vector2 from, Vector2 to)
  {
    var closestPosInSegment = _nav.GetClosestPositionInSegment(from);

    long fromId = _nav.GetClosestPoint(closestPosInSegment);
    long toId = _nav.GetClosestPoint(to);

    Vector2[] steps = _nav.GetPointPath(fromId, toId);

    if (steps.Length > 0 && steps[0].DistanceTo(closestPosInSegment) < 32)
    {
      steps = steps.Skip(1).ToArray();
    }

    return steps;
  }

  private long GetTileIndex(Vector2I mapCoords)
  {
    return mapCoords.Y + (mapCoords.X * _rect.Size.Y);
  }
}
