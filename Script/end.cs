using Godot;
using System;

public partial class end : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += onpressedfinalstory;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	private void onpressedfinalstory()
	{
		GetTree().ChangeSceneToFile("res://finalstory.tscn");
	}
}
