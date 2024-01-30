using Godot;
using System.Collections.Generic;

public partial class LevelTiles : TileMap
{
	// private NavMapping _nav;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Rect2I usedRect = GetUsedRect();
		// _nav = new NavMapping(usedRect);
		// RemapNavs();
	}

	// private IEnumerable<Vector2I> TileIterator()
	// {
	// 	Rect2I usedRect = GetUsedRect();
	// 	for (int i = usedRect.Position.X; i < usedRect.End.X; i++)
	// 	{
	// 		for (int j = usedRect.Position.Y; j < usedRect.End.Y; j++)
	// 		{
	// 			yield return new Vector2I(i, j);
	// 		}
	// 	}
	// }

	// private void RemapNavPoints()
	// {
	// 	foreach (Vector2I mapCoords in TileIterator())
	// 	{
	// 		TileData tileData = GetCellTileData(0, mapCoords);
	// 		Vector2 localCoords = MapToLocal(mapCoords);
	// 		_nav.AddPoint(mapCoords, localCoords, tileData);
	// 	}
	// }

	// private void RemapNavSegments()
	// {
	// 	var cellCoords = GetUsedCells(0);
	// 	foreach (Vector2I coords in cellCoords)
	// 	{
	// 		TileInfo
	// 			thisTile = _nav.GetTile(coords),
	// 			up = _nav.Neighbor(coords, new Vector2I(0, -1)),
	// 			upUp = _nav.Neighbor(coords, new Vector2I(0, -2)),
	// 			upUpRight = _nav.Neighbor(coords, new Vector2I(1, -2)),
	// 			upUpLeft = _nav.Neighbor(coords, new Vector2I(-1, -2)),
	// 			upLeft = _nav.Neighbor(coords, new Vector2I(-1, -1)),
	// 			upRight = _nav.Neighbor(coords, new Vector2I(1, -1)),
	// 			down = _nav.Neighbor(coords, new Vector2I(0, 1)),
	// 			downDown = _nav.Neighbor(coords, new Vector2I(0, 2)),
	// 			downLeft = _nav.Neighbor(coords, new Vector2I(-1, 1)),
	// 			downRight = _nav.Neighbor(coords, new Vector2I(1, 1)),
	// 			right = _nav.Neighbor(coords, new Vector2I(1, 0)),
	// 			rightRight = _nav.Neighbor(coords, new Vector2I(2, 0)),
	// 			left = _nav.Neighbor(coords, new Vector2I(-1, 0)),
	// 			leftLeft = _nav.Neighbor(coords, new Vector2I(-2, 0));

	// 		if (!thisTile.IsPlatform || !up.IsPassable)
	// 		{
	// 			continue;
	// 		}

	// 		if (right.IsPlatform && upRight.IsPassable)
	// 		{
	// 			_nav.ConnectPoints(up.Coords, upRight.Coords);
	// 		}

	// 		if (left.IsPlatform && upLeft.IsPassable)
	// 		{
	// 			_nav.ConnectPoints(up.Coords, upLeft.Coords);
	// 		}

	// 		// // There is a tile to the right
	// 		// // There is space above the tile to the right
	// 		// // ->> Connect tile above to tile above to the right.

	// 		// // There is a tile to the left
	// 		// // There is space above the tile to the left
	// 		// // ->> Connect tile above to tile above to the left.

	// 		// // There is a tile above and to the right
	// 		// // There is space above that tile
	// 		// // There is a space two above this tile
	// 		// // ->> Connect tile above to that tile

	// 		// // There is a tile above and to the left
	// 		// // There is space above that tile
	// 		// // ->> Connect tile above to that tile

	// 		// // There is no tile to the right
	// 		// // Continue to look down until we run out of points
	// 		// // If we find a tile down there, connect it.

	// 		// // There is no tile to the left
	// 		// // Continue to look down until we run out of points
	// 		// // If we find a tile down there, connect it.
	// 	}
	// }

	// public void RemapNavs()
	// {
	// 	_nav.Clear();
	// 	RemapNavPoints();
	// 	RemapNavSegments();
	// }

	// public Vector2[] GetWalkingPath(Vector2 fromPoint, Vector2 toPoint)
	// {
	// 	return _nav.GetPointConnection(fromPoint, toPoint);
	// }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
