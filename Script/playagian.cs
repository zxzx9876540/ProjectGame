using Godot;
using System;

public partial class playagian : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += onpressedagain;
	}

private void onpressedagain()
{
	// Reset stat กลับเป็นค่าเริ่มต้น
	CharacterStats.DeleteSave();
 	GameManager.Instance.SetStats(CharacterStats.CreateMage());

	GetTree().ChangeSceneToFile("res://choice1.tscn");
}
}
