using Centari.BehaviorTree;
using Centari.Navigation;
using Centari.Navigation.Rules;
using Centari.Player;
using Godot;

namespace Centari.Monsters;

public class TaskFollow : AbstractBallTask
{
  public TaskFollow(
    TestBall ball,
    NavCoordinator nav,
    PlayerCharacter player
  ) : base(ball, nav, player)
  { }

  public override NodeState Evaluate(double delta)
  {
    Vector2 pos1 = _thisCreature.Position;
    Vector2 pos2 = _player.Position;

    Vector2[] path = _nav.GetPath(TestBall.Nav, pos1, pos2);

    if (path.Length == 0)
    {
      return NodeState.SUCCESS;
    }

    Vector2 nextPoint = path[0];

    float distTo = pos1.DistanceTo(nextPoint);
    if (path.Length > 1 && distTo < _thisCreature.WalkSpeed)
    {
      nextPoint = path[1];
    }

    Vector2 dirTo = pos1.DirectionTo(nextPoint);
    if (dirTo.Y < 0)
    {
      Vector2 vel = _thisCreature.Velocity;
      vel.Y += dirTo.Y * distTo * 9;
      vel.X += dirTo.X * distTo * 2.5f;
      _thisCreature.Velocity = vel;
    }

    else
    {
      _thisCreature.Position = _thisCreature.Position.MoveToward(
        nextPoint,
        (float)delta * _thisCreature.WalkSpeed
      );
    }

    return NodeState.RUNNING;
  }
}
