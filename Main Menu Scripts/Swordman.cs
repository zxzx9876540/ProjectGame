using Godot;
using System;

public partial class Swordman : Button
{

	public override void _Ready()
	{
		Pressed += onclick;
	}

	private void onclick()
	{
		GetTree().ChangeSceneToFile("res://SwordmanDetail.tscn");
	}
}
