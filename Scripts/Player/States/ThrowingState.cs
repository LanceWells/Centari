using Centari.Player;
using Centari.Player.States;
using Centari.State;
using Godot;

public partial class ThrowingState : AbstractPlayerState
{
  protected override StateCapabilities Capabilities => new()
  {
    GravityAffected = true,
  };

  public void OnAnimationFinished(StringName animationName)
  {
    if (_inputQueue.Dequeue(PlayerInput.Attack))
    {
      if (animationName == "Throw1")
      {
        _animationPlayer.Play("Throw2");
      }
      else if (animationName == "Throw2")
      {
        _animationPlayer.Play("Throw1");
      }
    }
    else
    {
      _stateMachine.TransitionState("IdleState");
    }
  }

  public override void Transition(
    StateMachine stateMachine,
    AnimationPlayer animationPlayer,
    Node owner,
    string previousState
  )
  {
    base.Transition(stateMachine, animationPlayer, owner, previousState);

    animationPlayer.AnimationFinished += OnAnimationFinished;

    _animationPlayer.Play("Throw1");
  }

  public override void Process(double delta)
  {
    base.Process(delta);

    if (_player.ProjectileThrown)
    {
      _handleFireProjectile();
      _player.ProjectileThrown = false;
    }
  }
}
