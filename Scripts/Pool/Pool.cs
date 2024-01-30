using System.Collections.Generic;
using Godot;

namespace Centari.Common;

public class Pool<T> where T : Node
{
  private Queue<T> _resources;

  private readonly int _limit;

  public Queue<T> Resources => _resources;

  public Pool(int limit)
  {
    _limit = limit;
    _resources = new Queue<T>();
  }

  public void AddResource(T resource)
  {
    _resources.Enqueue(resource);
    if (_resources.Count > _limit)
    {
      T oldResource = _resources.Dequeue();
      if (oldResource != null)
      {
        // oldResource.Dispose();
        oldResource.QueueFree();
      }
    }
  }
}
