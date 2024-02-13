namespace Centari.Behaviors.Common;

/// <summary>
/// Describes the state of a given node after processing.
/// </summary>
public enum NodeState
{
  /// <summary>
  /// The node is currently running, and will need more frames to process.
  /// </summary>
  RUNNING,

  /// <summary>
  /// The node failed in some way. This does not necessarily connote an error, but it does indicate
  /// that some "failing" behavior occurred for the given node. This usually means that the parent
  /// node should stop processing.
  /// </summary>
  FAILURE,

  /// <summary>
  /// The node succeeded in some way. This usually means that the next node in a chain should be
  /// run.
  /// </summary>
  SUCCESS,
}
