using Godot;
using System.Collections.Generic;

public partial class LevelTiles : TileMap
{
	private NavMapping nav;

	private Dictionary<long, Vector2> tileIdToCoord;

	private Dictionary<Vector2, long> coordToTileId;

	private class TileInfo
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

	private class NavMapping
	{
		private AStar2D _nav;

		private Rect2I _rect;

		private Dictionary<long, long> _tileIdToCoord;

		private Dictionary<long, long> _coordToTileId;

		private Dictionary<long, TileInfo> _coordToTileInfo;

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

			// if (!_nav.ArePointsConnected(toId, fromId))
			// {
			// 	return new Vector2[0];
			// }

			Vector2[] steps = _nav.GetPointPath(fromId, toId);
			return steps;
		}

		private long GetTileIndex(Vector2I mapCoords)
		{
			return mapCoords.Y + (mapCoords.X * _rect.Size.Y);
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Rect2I usedRect = GetUsedRect();
		nav = new NavMapping(usedRect);
		RemapNavs();
	}

	private IEnumerable<Vector2I> TileIterator()
	{
		Rect2I usedRect = GetUsedRect();
		for (int i = usedRect.Position.X; i < usedRect.End.X; i++)
		{
			for (int j = usedRect.Position.Y; j < usedRect.End.Y; j++)
			{
				yield return new Vector2I(i, j);
			}
		}
	}

	private void RemapNavPoints()
	{
		foreach (Vector2I mapCoords in TileIterator())
		{
			TileData tileData = GetCellTileData(0, mapCoords);
			Vector2 localCoords = MapToLocal(mapCoords);
			nav.AddPoint(mapCoords, localCoords, tileData);
		}
	}

	private void RemapNavSegments()
	{
		var cellCoords = GetUsedCells(0);
		foreach (Vector2I coords in cellCoords)
		{
			TileInfo
				thisTile = nav.GetTile(coords),
				up = nav.Neighbor(coords, new Vector2I(0, -1)),
				upUp = nav.Neighbor(coords, new Vector2I(0, -2)),
				upUpRight = nav.Neighbor(coords, new Vector2I(1, -2)),
				upUpLeft = nav.Neighbor(coords, new Vector2I(-1, -2)),
				upLeft = nav.Neighbor(coords, new Vector2I(-1, -1)),
				upRight = nav.Neighbor(coords, new Vector2I(1, -1)),
				down = nav.Neighbor(coords, new Vector2I(0, 1)),
				downDown = nav.Neighbor(coords, new Vector2I(0, 2)),
				downLeft = nav.Neighbor(coords, new Vector2I(-1, 1)),
				downRight = nav.Neighbor(coords, new Vector2I(1, 1)),
				right = nav.Neighbor(coords, new Vector2I(1, 0)),
				rightRight = nav.Neighbor(coords, new Vector2I(2, 0)),
				left = nav.Neighbor(coords, new Vector2I(-1, 0)),
				leftLeft = nav.Neighbor(coords, new Vector2I(-2, 0));

			if (!thisTile.IsPlatform || !up.IsPassable)
			{
				continue;
			}

			if (right.IsPlatform && upRight.IsPassable)
			{
				nav.ConnectPoints(up.Coords, upRight.Coords);
			}

			if (left.IsPlatform && upLeft.IsPassable)
			{
				nav.ConnectPoints(up.Coords, upLeft.Coords);
			}

			// // There is a tile to the right
			// // There is space above the tile to the right
			// // ->> Connect tile above to tile above to the right.

			// // There is a tile to the left
			// // There is space above the tile to the left
			// // ->> Connect tile above to tile above to the left.

			// // There is a tile above and to the right
			// // There is space above that tile
			// // There is a space two above this tile
			// // ->> Connect tile above to that tile

			// // There is a tile above and to the left
			// // There is space above that tile
			// // ->> Connect tile above to that tile

			// // There is no tile to the right
			// // Continue to look down until we run out of points
			// // If we find a tile down there, connect it.

			// // There is no tile to the left
			// // Continue to look down until we run out of points
			// // If we find a tile down there, connect it.
		}
	}

	public void RemapNavs()
	{
		nav.Clear();
		RemapNavPoints();
		RemapNavSegments();
	}

	public Vector2[] GetWalkingPath(Vector2 fromPoint, Vector2 toPoint)
	{
		return nav.GetPointConnection(fromPoint, toPoint);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
