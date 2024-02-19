using System.Collections.Generic;
using System.Linq;

namespace Centari.Navigation;

public static class NavPatternDeserializer
{
  public static NavPattern Deserialize(NavPatternPath patternPath)
  {
    List<NavPathItem[]> res = new();
    string[] rows = patternPath.path.Split('\n');

    foreach (string row in rows)
    {
      string r = row.Trim();
      if (r.Length == 0)
      {
        continue;
      }

      NavPathItem[] paths = r.ToCharArray().Select((c) =>
      {
        return c switch
        {
          '.' => NavPathItem.Ignore,
          'e' => NavPathItem.PathEnd,
          's' => NavPathItem.PathStart,
          '=' => NavPathItem.Platform,
          '^' => NavPathItem.EmptySpace,
          'y' => NavPathItem.RaycastDown,
          _ => NavPathItem.Ignore,
        };
      })
      .ToArray();

      res.Add(paths);
    }

    NavPathItem[][] resArray = res.ToArray();
    return new NavPattern()
    {
      mirror = patternPath.mirror,
      path = resArray,
      id = patternPath.id,
    };
  }
}
