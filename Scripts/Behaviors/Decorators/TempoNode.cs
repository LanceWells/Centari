using Centari.Behaviors;
using Centari.Behaviors.Common;

public class TempoNode<T> : INode<T>
{
  private INode<T> _child;

  private NodeTempo _tempo;

  private string _tempoKey;

  private NodeState _cachedState;

  public TempoNode(INode<T> child, ref NodeTempo tempo, string tempoKey)
  {
    _child = child;
    _tempo = tempo;
    _tempoKey = tempoKey;
    _cachedState = NodeState.RUNNING;
  }

  public void Init(ref T contextRef)
  {
    _child.Init(ref contextRef);
  }

  public NodeState Process(double delta)
  {
    _tempo.Update(delta);

    bool doProcessChild = _tempo.TimeFor(_tempoKey);
    if (doProcessChild)
    {
      _cachedState = _child.Process(delta);
    }

    return _cachedState;
  }
}
