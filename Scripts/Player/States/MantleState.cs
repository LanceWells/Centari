using Centari.State;
using Godot;

namespace Centari.Player.States;

public partial class MantleState : AbstractPlayerState
{
  protected override StateCapabilities Capabilities => new()
  { };

  public override void Transition(StateMachine stateMachine, AnimationPlayer animationPlayer, Node owner)
  {
    base.Transition(stateMachine, animationPlayer, owner);

  }

  public override void Detransition()
  { }
}
