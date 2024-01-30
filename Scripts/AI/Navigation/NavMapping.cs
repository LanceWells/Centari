using Godot;
using System.Collections.Generic;

namespace Centari.Navigation;

public class NavMapping
{
  public NavMapping(Rect2I rect)
  {
    _nav = new();
    _tileIdToCoord = new();
    _coordToTileId = new();
    _coordToTileInfo = new();

    _rect = rect;
  }

  public void AddPoint(
    Vector2I mapPosition,
    Vector2 localPosition,
    TileData tileData,
    float weightScale = 1
  )
  {
    long tileId = _nav.GetAvailablePointId();
    TileInfo tileInfo = new TileInfo(tileData, mapPosition);

    _tileIdToCoord.Add(tileId, GetTileIndex(mapPosition));
    _coordToTileId.Add(GetTileIndex(mapPosition), tileId);
    _coordToTileInfo.Add(GetTileIndex(mapPosition), tileInfo);

    _nav.AddPoint(tileId, localPosition, weightScale);
  }

  public void ConnectPoints(Vector2I pos1, Vector2I pos2, bool bidirectional = false)
  {
    bool hasId1 = _coordToTileId.TryGetValue(GetTileIndex(pos1), out long id1);
    bool hasId2 = _coordToTileId.TryGetValue(GetTileIndex(pos2), out long id2);

    if (hasId1 && hasId2)
    {
      _nav.ConnectPoints(id1, id2, bidirectional);
    }
  }

  public TileInfo GetTile(Vector2I thisPos)
  {
    bool hasTile = _coordToTileInfo.TryGetValue(GetTileIndex(thisPos), out TileInfo tileInfo);

    if (hasTile)
    {
      return tileInfo;
    }

    return new TileInfo(null, new Vector2I(-1, -1));
  }

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

  public void Clear()
  {
    _nav.Clear();
  }

  public Vector2[] GetPointConnection(Vector2 from, Vector2 to)
  {
    long fromId = _nav.GetClosestPoint(from);
    long toId = _nav.GetClosestPoint(to);

    Vector2[] steps = _nav.GetPointPath(fromId, toId);
    return steps;
  }

  private AStar2D _nav;

  private Rect2I _rect;

  private Dictionary<long, long> _tileIdToCoord;

  private Dictionary<long, long> _coordToTileId;

  private Dictionary<long, TileInfo> _coordToTileInfo;

  private long GetTileIndex(Vector2I mapCoords)
  {
    return mapCoords.Y + (mapCoords.X * _rect.Size.Y);
  }
}
