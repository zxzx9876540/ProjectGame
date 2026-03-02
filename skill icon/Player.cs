using Godot;

public partial class Player : Node2D
{
	private float time = 0f;
	private Vector2 startPos;

	public override void _Ready()
	{
		startPos = Position;
	}

	public override void _Process(double delta)
	{
		time += (float)delta;

		// ขยับขึ้น-ลงเบา ๆ (หายใจ)
		float yOffset = Mathf.Sin(time * 2f) * 1.5f;
		Position = startPos + new Vector2(0, yOffset);
	}
}
