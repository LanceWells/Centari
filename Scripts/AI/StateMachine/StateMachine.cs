using Godot;

namespace Centari.State;

/// <summary>
/// A manager-type node that coordinates states; which one is active and when they desire to
/// transition to another state.
/// </summary>
public partial class StateMachine : Node
{
	/// <summary>
	/// The name of the default state node. Note that this should be consistent across all entities
	/// that utilize this state machine.
	/// </summary>
	public static string DEFAULT_STATE = "IdleState";

	private StateNode _activeState = null;

	private AnimationPlayer _animationPlayer;

	private Node _owner;

	/// <summary>
	/// Called when the node enters the scene tree for the first time.
	/// </summary>
	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("../AnimationPlayer");
		_owner = Owner;

		_activeState = new StateNode(GetNode(DEFAULT_STATE));
		_activeState.Transition(this, _animationPlayer, _owner);
	}

	/// <summary>
	/// Called every frame. 'delta' is the elapsed time since the previous frame.
	/// </summary>
	/// <param name="delta">The elapsed time since the previous frame.</param>
	public override void _Process(double delta)
	{
		if (_activeState != null)
		{
			_activeState.Process(delta);
		}
	}

	/// <summary>
	/// Called every frame. Used for physics processing.
	/// </summary>
	/// <param name="delta">The elapsed time since the previous frame.</param>
	public override void _PhysicsProcess(double delta)
	{
		if (_activeState != null)
		{
			_activeState.PhysicsProcess(delta);
		}
	}

	/// <summary>
	/// Should be called by states in order to transition to another state.
	/// </summary>
	/// <param name="stateName">
	/// The name of the state to transition to. NOTE: This is some declarative shenaniganry, so ensure
	/// that state transitions are spelled correctly.
	/// </param>
	public void TransitionState(string stateName)
	{
		_activeState?.Detransition();

		Node newStateNode = GetNodeOrNull(stateName);

		if (newStateNode != null)
		{
			StateNode newState = new StateNode(newStateNode);
			newState.Transition(this, _animationPlayer, _owner);
			_activeState = newState;
		}
		else
		{
			GD.PrintErr($"WARNING! Attempted to transition to state {stateName}, which could not be found");
		}
	}
}
