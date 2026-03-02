using Godot;
using System;

public partial class MainBackground : TextureRect
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Vector2 screenSize = GetViewport().GetVisibleRect().Size;
		Position = Vector2.Zero;
		Size = screenSize;
	}
}
