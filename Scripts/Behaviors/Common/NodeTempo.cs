using System;
using System.Collections.Generic;

namespace Centari.Behaviors;

public class NodeTempo
{
  private Dictionary<string, double> _timeSinceCalled;

  private double _actAfterDeltaMs;

  private double _throttleUntilDeltaMs;

  private Queue<string> _itemQueue;

  public NodeTempo(double actAfterDeltaMs, double throttleUntilDeltaMs)
  {
    _timeSinceCalled = new();
    _itemQueue = new();

    _actAfterDeltaMs = actAfterDeltaMs / 1000;
    _throttleUntilDeltaMs = throttleUntilDeltaMs / 1000;
  }

  public void Register(string key)
  {
    _timeSinceCalled.Add(key, 0);
    _itemQueue.Enqueue(key);
  }

  public void Update(double delta)
  {
    foreach (var kvp in _timeSinceCalled)
    {
      double lastVal = _timeSinceCalled[kvp.Key];
      _timeSinceCalled[kvp.Key] += lastVal + delta;
    }
  }

  public bool TimeFor(string key)
  {
    bool haveKey = _timeSinceCalled.TryGetValue(key, out double timeSinceLastRun);
    if (!haveKey)
    {
      Console.Error.WriteLine($"Tried to get a key {key}, which does not exist");
      return false;
    }

    if (timeSinceLastRun > _actAfterDeltaMs)
    {
      _timeSinceCalled[key] = 0;
      return true;
    }

    if (timeSinceLastRun < _throttleUntilDeltaMs)
    {
      return false;
    }

    string nextItemToRun = _itemQueue.Peek();

    if (nextItemToRun != key)
    {
      return false;
    }

    _timeSinceCalled[key] = 0;

    _itemQueue.Dequeue();
    _itemQueue.Enqueue(nextItemToRun);

    return true;
  }
}
