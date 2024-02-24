using System.Collections.Generic;
using Godot;

namespace Centari.Player;

public enum PlayerInput
{
  MoveLeft,

  MoveRight,

  Jump
}

public class InputQueue
{
  private Dictionary<PlayerInput, double> _inputMaxLife = new();

  private Dictionary<PlayerInput, double> _inputLifeRemaining = new();

  public InputQueue(Dictionary<PlayerInput, double> maxLife)
  {
    foreach (var kvp in maxLife)
    {
      _inputMaxLife.Add(kvp.Key, kvp.Value);
      _inputLifeRemaining.Add(kvp.Key, 0);
    }
  }

  public void Refresh(double delta)
  {
    void updateInput(PlayerInput input, double delta)
    {
      // If there's a decay, new value is equal to the last value minus decay
      // If there's no decay, new value is equal to the max, if there is one.

      if (!_inputMaxLife.ContainsKey(input))
      {
        return;
      }

      if (LivePeek(input))
      {
        _inputLifeRemaining[input] = _inputMaxLife[input];
      }
      else if (_inputLifeRemaining.TryGetValue(input, out double value))
      {
        _inputLifeRemaining[input] = value - delta;
      }
    }

    updateInput(PlayerInput.Jump, delta);
    updateInput(PlayerInput.MoveLeft, delta);
    updateInput(PlayerInput.MoveRight, delta);
  }

  public bool Peek(PlayerInput input)
  {
    if (!_inputMaxLife.ContainsKey(input))
    {
      return LivePeek(input);
    }

    if (_inputLifeRemaining.TryGetValue(input, out double remaining) && remaining > 0)
    {
      return true;
    }

    return false;
  }

  public bool Dequeue(PlayerInput input)
  {
    bool isQueued = Peek(input);

    if (_inputLifeRemaining.ContainsKey(input))
    {
      _inputLifeRemaining.Remove(input);
    }

    return isQueued;
  }

  public static bool LivePeek(PlayerInput input)
  {
    return input switch
    {
      PlayerInput.Jump => Input.IsActionPressed("jump"),
      PlayerInput.MoveLeft => Input.IsActionPressed("move_left"),
      PlayerInput.MoveRight => Input.IsActionPressed("move_right"),
      _ => false
    };
  }

  public static bool LiveJustPressed(PlayerInput input)
  {
    return input switch
    {
      PlayerInput.Jump => Input.IsActionJustPressed("jump"),
      PlayerInput.MoveLeft => Input.IsActionJustPressed("move_left"),
      PlayerInput.MoveRight => Input.IsActionJustPressed("move_right"),
      _ => false
    };
  }
}
