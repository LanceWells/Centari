namespace Centari.Navigation;

public enum NavModes
{
  Cat,
}

public enum NavPathItem
{
  Ignore,

  PathEnd,

  PathStart,

  Platform,

  EmptySpace,

  RaycastDown,

  OneDirection,
}

public struct NavPatternPath
{
  public string id;

  public bool mirror;

  public string path;
}

public struct NavPatternPathing
{
  public NavPatternPath[] paths;
}

public struct NavPatternOptions
{
  public NavPatternPathing cat;
}

public struct NavPattern
{
  public bool mirror;

  public string id;

  public NavPathItem[][] path;
}
