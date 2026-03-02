using Godot;
using System;

public partial class choice1stats : TextureButton
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += OnStatUpgradePressed;
	}

		private void OnStatUpgradePressed()
	{
		GameManager.Instance.PreviousScene = "res://choice1.tscn";
		GetTree().ChangeSceneToFile("res://Upgrade.tscn");
	}
}
