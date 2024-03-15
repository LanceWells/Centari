using System;
using System.Collections.Generic;
using Godot;

namespace Centari.Player;

/// <summary>
/// An enum used to refer to a given input that the player intends to use.
/// </summary>
public enum PlayerInput
{
  /// <summary>
  /// The player intends to move left.
  /// </summary>
  MoveLeft,

  /// <summary>
  /// The player intends to move right.
  /// </summary>
  MoveRight,

  /// <summary>
  /// The player intends to jump.
  /// </summary>
  Jump,

  Attack
}

/// <summary>
/// A class used to keep track of player inputs, each with a distinct timeout. After an input's
/// timeout, it is no longer considered to be pressed. If there is no maximum timeout for a
/// given input, only the live check is used.
/// 
/// The intention is for situations like input queueing, where the player might press the spacebar
/// right before landing, but the button won't be fully down on that land. By using this queue, we
/// can keep track of the player pressing the spacebar, and can then check when transitioning to an
/// idle/running state, if we need to jump right again.
/// </summary>
public class InputQueue
{
  private Dictionary<PlayerInput, double> _inputMaxLife = new();

  private Dictionary<PlayerInput, double> _inputLifeRemaining = new();

  /// <summary>
  /// Creates a new instance of a <see cref="InputQueue">.
  /// </summary>
  /// <param name="maxLife">
  /// A relationship between each input type and the maximum life (in time, think delta) for that
  /// given input to be valid after the player has stopped pressing that button.
  /// </param>
  public InputQueue(Dictionary<PlayerInput, double> maxLife)
  {
    foreach (var kvp in maxLife)
    {
      _inputMaxLife.Add(kvp.Key, kvp.Value);
      _inputLifeRemaining.Add(kvp.Key, 0);
    }
  }

  /// <summary>
  /// Refreshes the player's inputs. This is critical to the functioning of this class, and should
  /// be used in Process (most likely, I think).
  /// </summary>
  /// <param name="delta">The time elapsed since the last frame.</param>
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

    foreach (PlayerInput input in Enum.GetValues(typeof(PlayerInput)))
    {
      updateInput(input, delta);
    }
  }

  /// <summary>
  /// Takes a look at a given input to see if the queue thinks that the input is pressed. Does not
  /// reset the value.
  /// </summary>
  /// <param name="input">The input to check.</param>
  /// <returns>True if the input is pressed.</returns>
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

  /// <summary>
  /// Looks at the given input, and ensures that any future <see cref="Peek"/> or
  /// <see cref="Dequeue"/> calls will return false until the input is refreshed and pressed again.
  /// 
  /// This is usefulto ensure that the input is not recognized as being pressed for the rest of the
  /// frame.
  /// </summary>
  /// <param name="input">The input to check.</param>
  /// <returns>True if the input is pressed.</returns>
  public bool Dequeue(PlayerInput input)
  {
    bool isQueued = Peek(input);

    if (_inputLifeRemaining.ContainsKey(input))
    {
      _inputLifeRemaining.Remove(input);
    }

    return isQueued;
  }

  /// <summary>
  /// Bypasses the queue entirely and checks if the input is currently pressed.
  /// </summary>
  /// <param name="input">The input to check.</param>
  /// <returns>True if the input is pressed.</returns>
  public static bool LivePeek(PlayerInput input)
  {
    return input switch
    {
      PlayerInput.Jump => Input.IsActionPressed("jump"),
      PlayerInput.MoveLeft => Input.IsActionPressed("move_left"),
      PlayerInput.MoveRight => Input.IsActionPressed("move_right"),
      PlayerInput.Attack => Input.IsActionPressed("fire_projectile"),
      _ => false
    };
  }

  /// <summary>
  /// Bypsased the queue entirely and checks if the input was just pressed this frame.
  /// </summary>
  /// <param name="input">The input to check.</param>
  /// <returns>True if the input is pressed.</returns>
  public static bool LiveJustPressed(PlayerInput input)
  {
    return input switch
    {
      PlayerInput.Jump => Input.IsActionJustPressed("jump"),
      PlayerInput.MoveLeft => Input.IsActionJustPressed("move_left"),
      PlayerInput.MoveRight => Input.IsActionJustPressed("move_right"),
      PlayerInput.Attack => Input.IsActionJustPressed("fire_projectile"),
      _ => false
    };
  }
}
