using Godot;

public partial class Choicemage : TextureButton
{
	public override void _Ready()
	{
		// ยังไม่ต้องทำอะไรตอนนี้
	}

	// ผูกกับปุ่มเลือกเมจ
	public void OnMagePressed()
	{
		GameManager.Instance.SetStats(CharacterStats.CreateMage());
		GetTree().ChangeSceneToFile("res://choice1.tscn");
	}
}
