namespace Centari.Behaviors.Common;

/// <summary>
/// A general interface for behavior tree nodes.
/// </summary>
/// <typeparam name="TContext">The context that will be passed through this tree.</typeparam>
public interface INode<TContext>
{
  /// <summary>
  /// Run when intiailizing this node in the tree. Should also initialize all children nodes.
  /// </summary>
  /// <param name="contextRef">
  /// A reference to the context. The context must be shared throughout the (sub)tree.
  /// </param>
  public void Init(ref TContext contextRef);

  /// <summary>
  /// Run when the parent object performs its own _Process method.
  /// </summary>
  /// <param name="delta">The time delta between this frame and the last.</param>
  /// <returns>The state of the node after processing.</returns>
  public NodeState Process(double delta);
}
