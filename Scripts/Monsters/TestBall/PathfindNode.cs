using System.Collections.Generic;
using System.Linq;
using Centari.BehaviorTree;
using Centari.Navigation;
using Godot;

namespace Centari.Monsters;

public class PathfindNode : OrNode<INavNode>
{
  private AbstractMonster _thisCreature;

  private CharacterBody2D _trackedCreature;

  private NavCoordinator _nav;

  public PathfindNode(
    List<INavNode> children,
    NavCoordinator nav,
    CharacterBody2D trackedCreature,
    AbstractMonster thisCreature
  ) : base(children)
  {
    _nav = nav;
    _thisCreature = thisCreature;
    _trackedCreature = trackedCreature;
  }

  public override NodeState Evaluate(double delta)
  {
    Vector2 pos1 = _thisCreature.Position;
    Vector2 pos2 = _trackedCreature.Position;
    Vector2[] path = _nav.GetPath(TestBall.Nav, pos1, pos2);

    if (path.Length == 0)
    {
      return NodeState.SUCCESS;
    }

    Vector2 nextPoint = path[0];
    float distTo = pos1.DistanceTo(nextPoint);
    if (path.Length > 1 && distTo < _thisCreature.WalkSpeed)
    {
      path = path.Skip(1).ToArray();
    }

    _children.ForEach((child) => child.SetNav(path));

    return base.Evaluate(delta);
  }
}
