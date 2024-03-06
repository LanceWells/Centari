using Godot;
using System;

namespace Centari.Projectiles;

public partial class MagicShrapnel : RigidBody2D
{
	private Timer _deathTimer;

	private Timer _smolderTimer;

	private GpuParticles2D _particles;

	public void OnDeathTimeout()
	{
		CallDeferred(Node.MethodName.QueueFree);
	}

	public void OnSmolderTimeout()
	{
		_particles.Emitting = false;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_deathTimer = GetNode<Timer>("DeathTimer");
		_smolderTimer = GetNode<Timer>("SmolderTimer");
		_particles = GetNode<GpuParticles2D>("Particles");

		_deathTimer.Timeout += OnDeathTimeout;
		_smolderTimer.Timeout += OnSmolderTimeout;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{ }
}
