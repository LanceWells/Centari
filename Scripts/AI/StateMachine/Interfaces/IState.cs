using Godot;

namespace Centari.State;

/// <summary>
/// Class used to add an wrapper to nodes fetched via <see cref="Node.GetNode(Godot.NodePath)"/>.
/// The idea is that state nodes fetched via that method can be wrapped into this type, which then
/// gives us the ability to call methods natively via Godot's
/// <see cref="GodotObject.Call(Godot.StringName, Godot.Variant[])"/>.
/// </summary>
public class StateNode : IState
{
  private Node _node;

  /// <summary>
  /// Creates a new instance of a <see cref="StateNode"/>.
  /// </summary>
  /// <param name="node">The node that we should wrap.</param>
  public StateNode(Node node)
  {
    _node = node;
  }

  /// <inheritdoc/>
  public void Detransition()
  {
    _node.Call("Detransition");
  }

  /// <inheritdoc/>
  public void Transition(
    StateMachine stateMachine,
    AnimationPlayer animationPlayer,
    Node entity
  )
  {
    _node.Call("Transition", stateMachine, animationPlayer, entity);
  }

  /// <inheritdoc/>
  public void Process(double delta)
  {
    _node.Call("Process", delta);
  }

  /// <inheritdoc/>
  public void PhysicsProcess(double delta)
  {
    _node.Call("PhysicsProcess", delta);
  }
}

/// <summary>
/// An interface that should be implemented by each state. This is utilized by
/// <seealso cref="StateMachine"/>.
/// </summary>
public interface IState
{
  /// <summary>
  /// Called when we are transitioning to this node. Useful for immediate transitions like
  /// animations.
  /// </summary>
  public void Transition(
    StateMachine stateMachine,
    AnimationPlayer animationPlayer,
    Node player
  );

  /// <summary>
  /// Called when we are transitioning away from this node. Useful for cleanup processes.
  /// </summary>
  public void Detransition();

  // /// <summary>
  // /// Called for each logical processing.
  // /// </summary>
  // /// <param name="delta">
  // /// The number of frames that have passed since last calling this method.
  // /// </param>
  // public void Process(double delta);

  // /// <summary>
  // /// Called for each physics processing.
  // /// </summary>
  // /// <param name="delta">
  // /// The number of frames that have passed since last calling this method.
  // /// </param>
  // public void PhysicsProcess(double delta);
}
