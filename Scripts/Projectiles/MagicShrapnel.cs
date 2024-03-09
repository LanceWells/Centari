using Godot;
using System;

namespace Centari.Projectiles;

public partial class MagicShrapnel : RigidBody2D
{
	private Color _magicColor = new();

	private Timer _deathTimer;

	private Timer _smolderTimer;

	private GpuParticles2D _particles;

	[Export]
	public float DeathTime = 1.5f;

	[Export]
	public float SmolderTime = 1.0f;

	[Export]
	public float TimeVariability = 0.5f;

	public Color MagicColor
	{
		get => _magicColor;
		set => _magicColor = value;
	}

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
		Random r = new();

		_deathTimer = GetNode<Timer>("DeathTimer");
		_smolderTimer = GetNode<Timer>("SmolderTimer");

		GpuParticles2D[] particleOptions = new GpuParticles2D[3];
		particleOptions[0] = GetNode<GpuParticles2D>("ParticlesSmall");
		particleOptions[1] = GetNode<GpuParticles2D>("ParticlesMedium");
		particleOptions[2] = GetNode<GpuParticles2D>("ParticlesLarge");

		int particleSizeOption = r.Next(0, particleOptions.Length);
		_particles = particleOptions[particleSizeOption];

		for (int i = 0; i < particleOptions.Length; i++)
		{
			particleOptions[i].Visible = i == particleSizeOption;
		}

		if (_particles.Material is ShaderMaterial shader)
		{
			Variant f = shader.GetShaderParameter("tint_color");

			Variant tintColor = new Color(0, 1, 1, 0.5f);
			shader.SetShaderParameter("tint_color", tintColor);
		}

		_deathTimer.Timeout += OnDeathTimeout;
		_smolderTimer.Timeout += OnSmolderTimeout;


		float variableTime = TimeVariability * r.NextSingle();
		float variableScale = r.NextSingle();

		_deathTimer.Start(DeathTime + variableTime);
		_smolderTimer.Start(SmolderTime + variableTime);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{ }
}
