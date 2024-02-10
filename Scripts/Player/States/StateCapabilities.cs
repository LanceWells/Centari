namespace Centari.Player.States;

public readonly record struct StateCapabilities
{
  public bool CanWalk { get; init; }

  public bool GravityAffected { get; init; }

  public bool CanAttack { get; init; }

  public bool CanJump { get; init; }
}
