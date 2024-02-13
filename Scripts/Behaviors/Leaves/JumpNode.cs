using System.Collections.Generic;
using Centari.Behaviors.Common;
using Centari.Behaviors.Contexts;
using Godot;

namespace Centari.Behaviors.Leaves;

public class JumpNode : INode<INavContext>
{
  private INavContext _navContext;

  private Queue<Vector2> _path;

  public void Init(ref INavContext contextRef)
  {
    _navContext = contextRef;
  }

  public JumpNode(ref Queue<Vector2> path)
  {
    _path = path;
  }

  public NodeState Process(double delta)
  {
    bool hasNextPoint = _path.TryPeek(out var nextPos);
    if (!hasNextPoint)
    {
      return NodeState.SUCCESS;
    }

    Vector2 thisPos = _navContext.ThisMonster.Position;
    float gravity = _navContext.ThisMonster.Gravity;

    if (thisPos.Y < nextPos.Y)
    {
      return NodeState.FAILURE;
    }

    // Factor in our gravity. The multiplier here is because there's no 1:1 relationship between
    // gravity and how much we should jump. It's an estimated amount that looks natural.
    float jumpStrength = (nextPos.Y - thisPos.Y) * (gravity * 0.015f);

    Vector2 targetVel = new(0, jumpStrength);

    _navContext.ThisMonster.Velocity = targetVel;

    return NodeState.SUCCESS;
  }
}
