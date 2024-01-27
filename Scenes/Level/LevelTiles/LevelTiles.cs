using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

public partial class LevelTiles : TileMap
{
	// private AStar2D nav;

	private NavMapping nav;

	private Dictionary<long, Vector2> tileIdToCoord;

	private Dictionary<Vector2, long> coordToTileId;

	private class TileInfo
	{
		private TileData _tile;

		public bool CanJumpThrough => (bool)(_tile?.GetCustomData("CanJumpThrough") ?? false);

		public bool IsPlatform => (bool)(_tile?.GetCustomData("IsPlatform") ?? false);

		public bool IsPassable => CanJumpThrough || !IsPlatform;

		public TileInfo(TileData tile)
		{
			_tile = tile;
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
			TileInfo tileInfo = new TileInfo(tileData);

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

			return new TileInfo(null);
		}

		public TileInfo Neighbor(Vector2I thisPos, Vector2I relativePos)
		{
			Vector2I neighborPos = thisPos + relativePos;
			bool hasTile = _coordToTileInfo.TryGetValue(GetTileIndex(neighborPos), out TileInfo tileInfo);

			if (hasTile)
			{
				return tileInfo;
			}

			return new TileInfo(null);
		}

		public void Clear()
		{
			_nav.Clear();
		}

		private long GetTileIndex(Vector2I mapCoords)
		{
			return mapCoords.X + mapCoords.Y * _rect.Size.Y;
		}
	}

	// private TileInfo GetNeighbor(TileInfo tileInfo, Vector2I neighborCoords)
	// {
	// 	Vector2I relativeCoords = tileInfo.Coords + neighborCoords;
	// 	TileData tileData = GetCellTileData(0, relativeCoords);

	// 	return new TileInfo(tileData, relativeCoords);
	// }

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

	// private void RemapNavPoints()
	// {
	// 	tileIdToCoord = new Dictionary<long, Vector2>();
	// 	coordToTileId = new Dictionary<Vector2, long>();

	// 	foreach (Vector2I mapCoords in TileIterator())
	// 	{
	// 		long tileId = nav.GetAvailablePointId();

	// 		tileIdToCoord.Add(tileId, mapCoords);
	// 		coordToTileId.Add(mapCoords, tileId);

	// 		Vector2 localCoords = MapToLocal(mapCoords);
	// 		nav.AddPoint(tileId, localCoords);
	// 	}
	// }

	private void RemapNavSegments()
	{
		var cellCoords = GetUsedCells(0);
		foreach (Vector2I coords in cellCoords)
		{
			if (!nav.GetTile(coords).IsPlatform || !nav.Neighbor(coords, new Vector2I(0, -1)).IsPassable)
			{
				continue;
			}

			// TileData thisTileData = GetCellTileData(0, coords);
			// TileInfo thisTile = new(thisTileData, coords);

			// // There is no space above
			// // -XX Continue
			// if (
			// 	!thisTile.IsPlatform ||
			// 	!GetNeighbor(thisTile, new Vector2I(0, -1)).IsPassable
			// )
			// {
			// 	continue;
			// }

			// // There is a tile to the right
			// // There is space above the tile to the right
			// // ->> Connect tile above to tile above to the right.
			// if (
			// 	GetNeighbor(thisTile, new Vector2I(1, 0)).IsPlatform &&
			// 	GetNeighbor(thisTile, new Vector2I(1, -1)).IsPassable
			// )
			// {
			// 	Vector2I upRightTileCoords = coords + new Vector2I(1, -1);
			// 	Vector2I upTileCoords = coords + new Vector2I(0, -1);

			// 	bool haveKeys = true;
			// 	long upTileId = -1, upRightTileId = -1;
			// 	haveKeys = haveKeys && coordToTileId.TryGetValue(upTileCoords, out upTileId);
			// 	haveKeys = haveKeys && coordToTileId.TryGetValue(upRightTileCoords, out upRightTileId);

			// 	if (haveKeys)
			// 	{
			// 		nav.ConnectPoints(upTileId, upRightTileId);
			// 	}
			// }

			// // There is a tile to the left
			// // There is space above the tile to the left
			// // ->> Connect tile above to tile above to the left.
			// if (
			// 	GetNeighbor(thisTile, new Vector2I(-1, 0)).IsPlatform &&
			// 	GetNeighbor(thisTile, new Vector2I(-1, -1)).IsPassable
			// )
			// {
			// 	Vector2I upLeftTileCoords = coords + new Vector2I(-1, -1);
			// 	Vector2I upTileCoords = coords + new Vector2I(0, -1);

			// 	bool haveKeys = true;
			// 	long upTileId = -1, upLeftTileId = -1;
			// 	haveKeys = haveKeys && coordToTileId.TryGetValue(upTileCoords, out upTileId);
			// 	haveKeys = haveKeys && coordToTileId.TryGetValue(upLeftTileCoords, out upLeftTileId);

			// 	if (haveKeys)
			// 	{
			// 		nav.ConnectPoints(upTileId, upLeftTileId);
			// 	}
			// }

			// // There is a tile above and to the right
			// // There is space above that tile
			// // There is a space two above this tile
			// // ->> Connect tile above to that tile
			// // if (GetNeighborInfo(coords, new Vector2I()))
			// // {

			// // }

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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
