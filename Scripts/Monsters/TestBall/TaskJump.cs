using Centari.BehaviorTree;
using Centari.Navigation;
using Centari.Player;
using Godot;

namespace Centari.Monsters;

public class TaskJump : AbstractNavTask
{
  public TaskJump(
    TestBall ball,
    NavCoordinator nav,
    PlayerCharacter player
  ) : base(ball, nav, player)
  { }

  public override NodeState Evaluate(double delta)
  {
    float distTo = _thisCreature.Position.DistanceTo(NextPoint);
    Vector2 dirTo = _thisCreature.Position.DirectionTo(NextPoint);

    if (_thisCreature.IsOnFloor() && dirTo.Y < 0)
    {
      Vector2 vel = _thisCreature.Velocity;
      vel.Y = -(distTo * _thisCreature.Gravity * (float)0.01);
      _thisCreature.Velocity = vel;
    }

    return NodeState.SUCCESS;
  }
}
