using Godot;

namespace Centari.State;

/// <summary>
/// An abstraction of <see cref="IState"/>. Used to add common methods and setup.
/// </summary>
public abstract partial class AbstractState : Node, IState
{
  /// <summary>
  /// A reference to the state machine that this state belongs to.
  /// </summary>
  protected StateMachine _stateMachine;

  /// <summary>
  /// A reference to the animation player that this state has reference to.
  /// </summary>
  protected AnimationPlayer _animationPlayer;

  /// <summary>
  /// A reference to the owner for this node.
  /// </summary>
  protected Node _owner;

  /// <summary>
  /// When called, sets up some base variables. This is separated from a constructor method so that
  /// we are able to fully satisfy Godot node creation without managing their construction. Should
  /// be called in the <see cref="Node._Ready()"/> method.
  /// </summary>
  /// <param name="stateMachine">The state machine this state refers to.</param>
  /// <param name="animationPlayer">The animation player used in the owner's animations.</param>
  /// <param name="owner">The owner of this state's scene.</param>
  protected virtual void Prepare(
    StateMachine stateMachine,
    AnimationPlayer animationPlayer,
    Node owner
  )
  {
    _stateMachine = stateMachine;
    _animationPlayer = animationPlayer;
    _owner = owner;
  }

  /// <inheritdoc/>
  public abstract void Transition(
    StateMachine stateMachine,
    AnimationPlayer animationPlayer,
    Node owner
  );

  /// <inheritdoc/>
  public abstract void Detransition();

  /// <inheritdoc/>
  public virtual void Process(double delta)
  {
    _Process(delta);
  }

  /// <inheritdoc/>
  public virtual void PhysicsProcess(double delta)
  {
    _PhysicsProcess(delta);
  }

  /// <summary>
  /// Calls the base <see cref="_PhysicsProcess"/>. This is sealed so that all processing and
  /// physics processing runs through the state machine, which ensures that only the relevant state
  /// is processing at any given point.
  /// </summary>
  /// <param name="delta">The time delta since the last frame.</param>
  public sealed override void _PhysicsProcess(double delta)
  {
    base._PhysicsProcess(delta);
  }

  /// <summary>
  /// Calls the base <see cref="_Process"/>. This is sealed so that all processing and physics
  /// processing runs through the state machine, which ensures that only the relevant state is
  /// processing at any given point.
  /// </summary>
  /// <param name="delta">The time delta since the last frame.</param>
  public sealed override void _Process(double delta)
  {
    base._Process(delta);
  }
}
