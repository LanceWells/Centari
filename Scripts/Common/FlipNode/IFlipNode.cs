/// <summary>
/// Describes a class that acts as a "flipping" container for a given item. This is used for
/// handling items that might be attached to a given character body that must be flipped to an 
/// opposing side when the character's image flips horizontally.
/// 
/// For example, an item such as a raycast that faces away from a player's face might need to be
/// flipped to the opposing side once a player changes their walking direction.
/// 
/// The intention is that a player container contain a reference to this container. That player
/// container controls the "flipped" state of the object. This way, so long as an object contains a
/// reference to the player container, that object can get this object, flipped appropriately.
/// </summary>
/// <typeparam name="T">The type of node that is held and flipped.</typeparam>
public interface IFlipNode<T>
{
  /// <summary>
  /// The item that is either flipped (or not).
  /// </summary>
  public T Item
  {
    get;
  }

  /// <summary>
  /// If true, the item is currently flipped horizontally.
  /// </summary>
  public bool IsFlipped
  {
    get;
  }

  /// <summary>
  /// Sets the flipped state for the object. Note that this is not an inversion of a current state.
  /// This is in order to make the flipping deterministic.
  /// </summary>
  /// <param name="isFlipped">
  /// If true, flip the object. Otherwise, make it set to its original direction.
  /// </param>
  public void SetFlipped(bool isFlipped);
}
