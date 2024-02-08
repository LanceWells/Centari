using System;
using Centari.BehaviorTree;
using Centari.Navigation;
using Centari.Player;
using Godot;

namespace Centari.Monsters;

public class TaskAirFollow : AbstractNavTask
{
  public TaskAirFollow(
    TestBall ball,
    NavCoordinator nav,
    PlayerCharacter player
  ) : base(ball, nav, player)
  { }

  public override NodeState Evaluate(double delta)
  {
    // Only do something if the creature is falling, or near the apex of its jump.
    if (_thisCreature.IsOnFloor() || _thisCreature.Position.Y > TrackedCreature.Position.Y)
    {
      return NodeState.SUCCESS;
    }

    // If we're above the target, don't move.
    if (Math.Abs(TrackedCreature.Position.X - _thisCreature.Position.X) < _thisCreature.WalkSpeed)
    {
      return NodeState.RUNNING;
    }

    if (TrackedCreature.Position.X > _thisCreature.Position.X)
    {
      // Go left.
      Vector2 vel = new(_thisCreature.WalkSpeed, _thisCreature.Velocity.Y);
      _thisCreature.Velocity = vel;
    }
    else
    {
      // Go right.
      Vector2 vel = new(-_thisCreature.WalkSpeed, _thisCreature.Velocity.Y);
      _thisCreature.Velocity = vel;
    }

    return NodeState.RUNNING;
  }
}
