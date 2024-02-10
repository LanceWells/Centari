using System;
using System.Collections.Generic;
using System.Linq;
using Centari.BehaviorTree;
using Centari.Navigation;
using Godot;

namespace Centari.Monsters;

/// <summary>
/// A node used to decorate all children nodes with a nav path. This nav path typically assumes that
/// both creatures are on the ground.
/// </summary>
public class PathfindNode : OrNode<INavNode>
{
  private AbstractMonster _thisCreature;

  private CharacterBody2D _trackedCreature;

  private NavCoordinator _nav;

  private Vector2 _lastKnownPoint = Vector2.Zero;

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

    // if (path.Length == 0)
    // {
    //   Vector2 newVel = new(0, _thisCreature.Velocity.Y);
    //   _thisCreature.Velocity = newVel;
    //   return NodeState.SUCCESS;
    // }

    // if (path.Length > 0)
    // {
    //   Vector2 nextPoint = path[0];
    //   float distTo = pos1.DistanceTo(nextPoint);
    //   if (path.Length > 1 && distTo < _thisCreature.WalkSpeed)
    //   {
    //     path = path.Skip(1).ToArray();
    //   }
    // }

    // If there is a next point, use it.

    // Otherwise, if we remember the last point, use that.

    // Ultimately, if neither of those are available, use the creature's current position.

    Vector2 nextPoint = Vector2.Zero;

    if (path.Length > 0)
    {
      nextPoint = path[0];
      if (
        path.Length > 1
        && Math.Abs(_thisCreature.Position.X - nextPoint.X) < (_thisCreature.WalkSpeed * 0.05)
      )
      {
        nextPoint = path[1];
      }
    }
    else if (_lastKnownPoint.X != 0 || _lastKnownPoint.Y != 0)
    {
      nextPoint = _lastKnownPoint;
    }
    else
    {
      nextPoint = _thisCreature.Position;
    }

    _children.ForEach((child) => child.SetNextPoint(nextPoint));
    _lastKnownPoint = nextPoint;

    return base.Evaluate(delta);
  }
}
