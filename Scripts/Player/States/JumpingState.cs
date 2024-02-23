using Centari.State;
using Godot;

namespace Centari.Player.States;

public partial class JumpingState : AbstractPlayerState
{
  /// <summary>
  /// This timer defines how long the player can be moving upwards in their jump. Once this has
  /// timed out, then the player should transition into a falling state.
  /// </summary>
  public Timer JumpEnabledTimer;

  /// <summary>
  /// This timer defines how long before the player can "cancel" the jump. This lets a jump always
  /// have some lift off the ground, but still lets the player cancel the top portion of the jump if
  /// they want a lower trajectory.
  /// </summary>
  public Timer JumpCancelAfterTimer;

  /// <summary>
  /// If true, the player can cancel the jump. Set via <see cref="JumpCancelAfterTimer"/>.
  /// </summary>
  private bool _canCancelJump;

  protected override StateCapabilities Capabilities => new()
  {
    CanWalk = true,
    CanJump = true,
    CanAttack = true,
    GravityAffected = true,
  };

  /// <summary>
  /// Jumping can only go up for so long. Stop go up when timer done. If we're back on the floor
  /// though, jump again.
  /// </summary>
  public void OnJumpEnabledTimeout()
  {
    if (_player.IsOnFloor())
    {
      _stateMachine.TransitionState("JumpingState");
    }
    else
    {
      _stateMachine.TransitionState("FallingState");
    }
  }

  /// <summary>
  /// A jump can't be canceled instantly. This timer lets the user cancel their jump after they have
  /// already risen for some time. This makes the jumping feel more consistent.
  /// </summary>
  public void OnJumpCancelAfterTimeout()
  {
    _canCancelJump = true;
  }

  /// <inheritdoc/>
  protected override Vector2 GetWalking(Vector2 direction, double delta)
  {
    PlayerInputs p = GetPlayerInputs();

    Vector2 walkDirection = Vector2.Zero;
    if (p.MoveLeft)
    {
      walkDirection.X -= _player.MaxSpeed;
    }
    if (p.MoveRight)
    {
      walkDirection.X += _player.MaxSpeed;
    }

    Vector2 lerpedDir = direction.Lerp(
      walkDirection,
      (float)delta * _player.Friction * 0.2f
    );

    return lerpedDir;
  }

  /// <inheritdoc/>
  protected override Vector2 GetJumping(Vector2 direction, double delta)
  {
    PlayerInputs p = GetPlayerInputs();

    Vector2 vel = direction;
    if (!_canCancelJump || p.Jump)
    {
      vel.Y = -_player.JumpStrength;
    }

    return vel;
  }

  /// <inheritdoc/>
  public override void Transition(
    StateMachine stateMachine,
    AnimationPlayer animationPlayer,
    Node owner,
    string previousState
  )
  {
    base.Transition(stateMachine, animationPlayer, owner, previousState);

    JumpEnabledTimer = GetNode<Timer>("JumpEnabledTimer");
    JumpEnabledTimer.Timeout += OnJumpEnabledTimeout;
    JumpEnabledTimer.Start();

    JumpCancelAfterTimer = GetNode<Timer>("JumpCancelAfterTimer");
    JumpCancelAfterTimer.Timeout += OnJumpCancelAfterTimeout;
    JumpCancelAfterTimer.Start();

    _canCancelJump = false;

    _animationPlayer.Play("Jump Up");
  }

  /// <inheritdoc/>
  public override void Detransition(string nextState)
  {
    base.Detransition(nextState);

    JumpEnabledTimer.Timeout -= OnJumpEnabledTimeout;
    JumpCancelAfterTimer.Timeout -= OnJumpCancelAfterTimeout;
  }

  /// <inheritdoc/>
  public override void PhysicsProcess(double delta)
  {
    base.PhysicsProcess(delta);
    PlayerInputs p = GetPlayerInputs();

    if (_player.IsOnFloor())
    {
      _stateMachine.TransitionState("IdleState");
    }
    else if (_canCancelJump && !p.Jump)
    {
      _stateMachine.TransitionState("FallingState");
    }
  }
}
