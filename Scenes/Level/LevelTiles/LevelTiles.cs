using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

public partial class LevelTiles : TileMap
{
	private AStar2D nav;

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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		nav = new AStar2D();
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
		tileIdToCoord = new Dictionary<long, Vector2>();
		coordToTileId = new Dictionary<Vector2, long>();

		foreach (Vector2I mapCoords in TileIterator())
		{
			long tileId = nav.GetAvailablePointId();

			tileIdToCoord.Add(tileId, mapCoords);
			coordToTileId.Add(mapCoords, tileId);

			Vector2 localCoords = MapToLocal(mapCoords);
			nav.AddPoint(tileId, localCoords);
		}
	}

	// private TileInfo GetNeighborInfo(Vector2I coords, TileSet.CellNeighbor neighbor)
	// {
	// 	Vector2I neighborCoords = GetNeighborCell(coords, neighbor);
	// 	TileData neighborTileData = GetCellTileData(0, neighborCoords);
	// 	TileInfo neighborTileInfo = new(neighborTileData);

	// 	return neighborTileInfo;
	// }

	private TileInfo GetNeighborInfo(Vector2I coords, Vector2I neighborCoords)
	{
		Vector2I relativeCoords = coords + neighborCoords;
		TileData neighborTileData = GetCellTileData(0, relativeCoords);
		TileInfo neighborTileInfo = new(neighborTileData);

		return neighborTileInfo;
	}

	private void RemapNavSegments()
	{
		var cellCoords = GetUsedCells(0);
		foreach (Vector2I coords in cellCoords)
		{
			TileData thisTileData = GetCellTileData(0, coords);
			TileInfo thisTileInfo = new(thisTileData);

			TileInfo spaceTopSide = GetNeighborInfo(coords, new Vector2I(0, -1));
			TileInfo spaceUpUpSide = GetNeighborInfo(coords, new Vector2I(0, -2));
			TileInfo spaceRightSide = GetNeighborInfo(coords, new Vector2I(1, 0));
			TileInfo spaceLeftSide = GetNeighborInfo(coords, new Vector2I(-1, 0));
			TileInfo spaceTopRightSide = GetNeighborInfo(coords, new Vector2I(1, -1));
			TileInfo spaceTopLeftSide = GetNeighborInfo(coords, new Vector2I(-1, -1));

			// There is no space above
			// -XX Continue
			if (!thisTileInfo.IsPlatform || !spaceTopSide.IsPassable)
			{
				continue;
			}

			// There is a tile to the right
			// There is space above the tile to the right
			// ->> Connect tile above to tile above to the right.
			if (spaceRightSide.IsPlatform && spaceTopRightSide.IsPassable)
			{
				Vector2I upRightTileCoords = coords + new Vector2I(1, -1);
				Vector2I upTileCoords = coords + new Vector2I(0, -1);

				bool haveKeys = true;
				long upTileId = -1, upRightTileId = -1;
				haveKeys = haveKeys && coordToTileId.TryGetValue(upTileCoords, out upTileId);
				haveKeys = haveKeys && coordToTileId.TryGetValue(upRightTileCoords, out upRightTileId);

				if (haveKeys)
				{
					nav.ConnectPoints(upTileId, upRightTileId);
				}
			}

			// There is a tile to the left
			// There is space above the tile to the left
			// ->> Connect tile above to tile above to the left.
			if (spaceLeftSide.IsPlatform && spaceTopLeftSide.IsPassable)
			{
				Vector2I upLeftTileCoords = coords + new Vector2I(-1, -1);
				Vector2I upTileCoords = coords + new Vector2I(0, -1);

				bool haveKeys = true;
				long upTileId = -1, upLeftTileId = -1;
				haveKeys = haveKeys && coordToTileId.TryGetValue(upTileCoords, out upTileId);
				haveKeys = haveKeys && coordToTileId.TryGetValue(upLeftTileCoords, out upLeftTileId);

				if (haveKeys)
				{
					nav.ConnectPoints(upTileId, upLeftTileId);
				}
			}

			// There is a tile above and to the right
			// There is space above that tile
			// There is a space two above this tile
			// ->> Connect tile above to that tile
			// if (GetNeighborInfo(coords, new Vector2I()))
			// {

			// }

			// There is a tile above and to the left
			// There is space above that tile
			// ->> Connect tile above to that tile

			// There is no tile to the right
			// Continue to look down until we run out of points
			// If we find a tile down there, connect it.

			// There is no tile to the left
			// Continue to look down until we run out of points
			// If we find a tile down there, connect it.
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
