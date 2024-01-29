using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MonsterManager : Node
{
	private List<CharacterBody2D> _monsters = new List<CharacterBody2D>();

	private Player _player;

	private LevelTiles _tiles;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void Prepare(Player player, LevelTiles tiles)
	{
		_player = player;
		_tiles = tiles;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_player == null || _tiles == null)
		{
			return;
		}

		base._PhysicsProcess(delta);
		_monsters.ForEach((monster) =>
		{
			Vector2 pos1 = monster.Position;
			Vector2 pos2 = _player.Position;

			Vector2[] path = _tiles.GetWalkingPath(pos1, pos2);
			Vector2 nextPoint = pos1;

			if (path.Length > 1 && nextPoint.DistanceSquaredTo(pos1) < 5f)
			{
				nextPoint = path[1];
			}
			else if (path.Length > 0)
			{
				nextPoint = path[0];
			}

			monster.Position = monster.Position.MoveToward(nextPoint, (float)delta * 50f);
		});
	}

	public void RegisterMonster(CharacterBody2D monster)
	{
		_monsters.Add(monster);
	}
}
