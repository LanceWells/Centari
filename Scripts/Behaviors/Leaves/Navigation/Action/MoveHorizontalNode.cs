using Centari.Behaviors.Common;
using Godot;

namespace Centari.Behaviors.Leaves.Navigation;

public class MoveHorizontalNode : INode<INavContext>
{
  private INavContext _context;

  public void Init(ref INavContext contextRef)
  {
    _context = contextRef;
  }

  public NodeState Process(double delta)
  {
    if (_context.Path.Length == 0)
    {
      return NodeState.SUCCESS;
    }

    float walkSpeed = _context.ThisMonster.WalkSpeed;

    Vector2 nextPos = _context.Path[0];
    Vector2 thisPos = _context.ThisMonster.Position;

    _context.ThisMonster.Position = thisPos.MoveToward(nextPos, (float)delta * walkSpeed);

    return NodeState.SUCCESS;
  }
}
