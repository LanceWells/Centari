using Godot;

public partial class Player : CharacterBody2D
{
  [Export]
  public float MaxSpeed = 14.0f;

  [Export]
  public float FallAcceleration = 75f;

  [Export]
  public float Acceleration = 0.15f;

  [Export]
  public float Friction = 0.75f;

  public Sprite2D _sprite;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
	_sprite = GetNode<Sprite2D>("Sprite");
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _PhysicsProcess(double delta)
  {
	// // https://godotengine.org/qa/48404/make-character-face-cursors-direction-even-when-moving-around
	// Vector2 charPos = GetViewport().GetCamera3D().UnprojectPosition(this.GlobalTransform.Origin);
	// Vector2 mousePos = GetViewport().GetMousePosition();
	// // float angle = charPos.AngleToPoint(mousePos);

	// if (!this._sprite.FlipH && mousePos.X > charPos.X) {
	// 	this._sprite.FlipH = true;
	// } else if (this._sprite.FlipH && mousePos.X < charPos.X) {
	// 	this._sprite.FlipH = false;
	// }

	MoveAndSlide();
  }
}
