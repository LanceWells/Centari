using Centari.BehaviorTree;
using Centari.Navigation;
using Centari.Navigation.Rules;
using Godot;

namespace Centari.Monsters;

public class TaskFollow : AbstractBallTask
{
  public TaskFollow(
    TestBall ball,
    NavCoordinator nav,
    Player player
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

    if (path.Length > 1 && nextPoint.DistanceSquaredTo(nextPoint) < _thisCreature.WalkSpeed)
    {
      nextPoint = path[1];
    }

    _thisCreature.Position = _thisCreature.Position.MoveToward(
      nextPoint,
      (float)delta * _thisCreature.WalkSpeed
    );

    return NodeState.RUNNING;
  }
}
