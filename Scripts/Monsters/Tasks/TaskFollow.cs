using Centari.BehaviorTree;
using Centari.Navigation;
using Centari.Player;
using Godot;

namespace Centari.Monsters;

public class TaskFollow : AbstractNavTask
{
  public TaskFollow(
    TestBall ball,
    NavCoordinator nav,
    PlayerCharacter player
  ) : base(ball, nav, player)
  { }

  public override NodeState Evaluate(double delta)
  {
    if (NextPoint.X > _thisCreature.Position.X)
    {
      // Go right.
      // Vector2 vel = new(_thisCreature.WalkSpeed * 10 * (float)delta, _thisCreature.Velocity.Y);
      Vector2 vel = new(_thisCreature.WalkSpeed, _thisCreature.Velocity.Y);
      _thisCreature.Velocity = vel;
    }
    else
    {
      // Go left.
      // Vector2 vel = new(-_thisCreature.WalkSpeed * 10 * (float)delta, _thisCreature.Velocity.Y);
      Vector2 vel = new(-_thisCreature.WalkSpeed, _thisCreature.Velocity.Y);
      _thisCreature.Velocity = vel;
    }

    return NodeState.RUNNING;
  }
}
