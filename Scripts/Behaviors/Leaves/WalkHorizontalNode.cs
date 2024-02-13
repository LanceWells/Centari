using System;
using System.Collections.Generic;
using Centari.Behaviors.Common;
using Centari.Behaviors.Contexts;
using Godot;

namespace Centari.Behaviors.Leaves;

public class WalkHorizontalNode : INode<INavContext>
{
  private INavContext _navContext;

  private Queue<Vector2> _path;

  public void Init(ref INavContext contextRef)
  {
    _navContext = contextRef;
  }

  public WalkHorizontalNode(ref Queue<Vector2> path)
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
    float walkSpeed = _navContext.ThisMonster.WalkSpeed;

    if (thisPos.Y > nextPos.Y)
    {
      return NodeState.SUCCESS;
    }
    else if (Math.Abs(thisPos.X - nextPos.X) < 0.1)
    {
      return NodeState.SUCCESS;
    }

    _navContext.ThisMonster.Position = thisPos.MoveToward(nextPos, (float)delta * walkSpeed);

    return NodeState.RUNNING;
  }
}
