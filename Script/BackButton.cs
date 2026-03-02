using Godot;
using System;

public partial class BackButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += onclick;
	}

	private void onclick()
	{
		GetTree().ChangeSceneToFile("res://CreateCharacter.tscn");
	}
}
