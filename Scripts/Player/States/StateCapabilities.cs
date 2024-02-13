namespace Centari.Player.States;

/// <summary>
/// Defines the capabilities of the current state. This is used mostly when determining the movement
/// capabilities of the given state.
/// </summary>
public readonly record struct StateCapabilities
{
  /// <summary>
  /// If true, the player can move left and right in this state.
  /// </summary>
  public bool CanWalk { get; init; }

  /// <summary>
  /// If true, this state is affected by gravity. The player will fall when not on ground.
  /// </summary>
  public bool GravityAffected { get; init; }

  /// <summary>
  /// If true, the player can attack in this state.
  /// </summary>
  public bool CanAttack { get; init; }

  /// <summary>
  /// If true, the player can jump in this state.
  /// </summary>
  public bool CanJump { get; init; }

  /// <summary>
  /// If true, the player can visually flip horizontally in this state.
  /// </summary>
  public bool CanFlip { get; init; }
}
