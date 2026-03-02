using Godot;
using System;

public partial class StatsScene : TextureButton
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += onclick;
	}
	public void onclick()
	{
		GameManager.Instance.PreviousScene = "res://choice1.tscn";
		GetTree().ChangeSceneToFile("res://Upgrade.tscn");
	}
}
