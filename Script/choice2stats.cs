using Godot;
using System;

public partial class choice2stats : TextureButton
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += OnStatUpgradePressed;
	}

		private void OnStatUpgradePressed()
	{
		GameManager.Instance.PreviousScene = "res://choice2.tscn";
		GetTree().ChangeSceneToFile("res://Upgrade.tscn");
	}
}
