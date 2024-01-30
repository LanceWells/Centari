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
  public abstract void Process(double delta);

  /// <inheritdoc/>
  public abstract void PhysicsProcess(double delta);
}
