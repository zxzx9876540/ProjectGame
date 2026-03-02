using Godot;
using System;

public partial class NewGame : Button
{

	public override void _Ready()
	{
		Pressed += onclick;
	}

	private void onclick()
	{
		GD.Print("กดปุ่มแล้ว!");
		GetTree().ChangeSceneToFile("res://CreateCharacter.tscn");
	}
}
