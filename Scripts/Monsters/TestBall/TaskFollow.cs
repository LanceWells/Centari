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
    // Vector2 thisPos = _thisCreature.Position;

    // if (_path.Length == 0)
    // {
    //   return NodeState.SUCCESS;
    // }

    // Vector2 nextPoint = _path[0];

    // float distTo = thisPos.DistanceTo(nextPoint);
    // if (_path.Length > 1 && distTo < _thisCreature.WalkSpeed)
    // {
    //   nextPoint = _path[1];
    // }

    // Vector2 dirTo = thisPos.DirectionTo(nextPoint);
    // if (dirTo.Y < 0)
    // {
    //   Vector2 vel = Vector2.Zero;
    //   vel.Y = -(distTo * _thisCreature.Gravity * (float)0.075);
    //   vel.X = dirTo.X * distTo * 5;
    //   _thisCreature.Velocity = vel;
    // }

    // else
    // {
    Vector2 newPos = new Vector2(NextPoint.X, _thisCreature.Position.Y);
    _thisCreature.Position = _thisCreature.Position.MoveToward(
      newPos,
      (float)delta * _thisCreature.WalkSpeed
    );
    // }

    return NodeState.RUNNING;
  }
}
