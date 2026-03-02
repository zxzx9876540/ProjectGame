using Godot;
using System;

public partial class choice2battleinforest : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += onpressedbattleinforest;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	private void onpressedbattleinforest()
	{
		GetTree().ChangeSceneToFile("res://BattleInForest.tscn");
	}
}
