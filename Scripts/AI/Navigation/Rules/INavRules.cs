using Godot;

namespace Centari.Navigation.Rules;

public interface INavRules
{
  void SetValidPaths(NavMapping nav, TileMap tiles);
}
