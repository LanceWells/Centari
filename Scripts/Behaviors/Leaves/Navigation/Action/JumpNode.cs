using System;
using Centari.Behaviors.Common;
using Godot;

namespace Centari.Behaviors.Leaves.Navigation;

public class JumpNode : INode<INavContext>
{
  private INavContext _context;

  public void Init(ref INavContext contextRef)
  {
    _context = contextRef;
  }

  public NodeState Process(double delta)
  {
    float gravity = _context.ThisMonster.Gravity;

    Vector2 nextPos = _context.Path[0];
    Vector2 thisPos = _context.ThisMonster.Position;

    // Factor in our gravity. The multiplier here is because there's no 1:1 relationship between
    // gravity and how much we should jump. It's an estimated amount that looks natural.
    float jumpStrength = (nextPos.Y - thisPos.Y) * (gravity * 0.015f);

    Vector2 targetVel = new(0, jumpStrength);

    _context.ThisMonster.Velocity = targetVel;

    return NodeState.SUCCESS;
  }
}
