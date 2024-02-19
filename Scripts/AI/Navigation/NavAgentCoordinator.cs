using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Godot;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Centari.Navigation;

public class NavCoordinator
{
  private Dictionary<NavModes, NavAgent> _navs;

  public NavCoordinator(TileMap tiles)
  {
    var yamlDeserializer = new DeserializerBuilder()
      .WithNamingConvention(CamelCaseNamingConvention.Instance)
      .Build();

    var assembly = typeof(NavCoordinator).GetTypeInfo().Assembly;
    using Stream resource = assembly.GetManifestResourceStream("Centari.Scripts.AI.Navigation.navpatterns.yml");
    using StreamReader reader = new(resource);

    var navRules = yamlDeserializer.Deserialize<NavPatternOptions>(reader);

    static IEnumerable<NavPattern> GetPattern(NavPatternPathing pathing) =>
      pathing
      .paths
      .Select((p) => NavPatternDeserializer.Deserialize(p));

    _navs = new()
    {
      {
        NavModes.Cat,
        NavAgentFactory.Create(tiles, GetPattern(navRules.cat))
      },
    };
  }

  public Vector2[] GetPath(NavModes[] navModes, Vector2 from, Vector2 to)
  {
    Vector2[] path = Array.Empty<Vector2>();

    foreach (NavModes mode in navModes)
    {
      Vector2[] thisPath = System.Array.Empty<Vector2>();

      if (!_navs.ContainsKey(mode))
      {
        continue;
      }

      thisPath = _navs[mode].GetPointConnection(from, to);

      if (path.Length == 0 || (thisPath.Length != 0 && thisPath.Length < path.Length))
      {
        path = thisPath;
      }
    }

    return path ?? System.Array.Empty<Vector2>();
  }
}
