using Godot;

public partial class Upgrade : Control
{
	[Export] private Label _labelName;
	[Export] private Label _labelPoints;
	[Export] private Label _labelStr;
	[Export] private Label _labelAgi;
	[Export] private Label _labelInt;

	private CharacterStats _player;

	public override void _Ready()
	{
		_player = GameManager.Instance.Stats;
		RefreshAll();
	}

	private void RefreshAll()
	{
		_labelName.Text   = _player.CharName;
		_labelPoints.Text = string.Format("แต้มคงเหลือ: {0}", _player.StatusPoints);
		_labelStr.Text    = string.Format("STR: {0}  (ATK: {1})", _player.Str, _player.Atk);
		_labelAgi.Text    = string.Format("AGI: {0}  (SPD: {1})", _player.Agi, _player.Speed);
		_labelInt.Text    = string.Format("INT: {0}  (MATK: {1})", _player.Int, _player.MAtk);
	}

	// ── ปุ่ม STR ───────────────────────────────────
	public void OnStrPressed()
	{
		if (_player.UpgradeStat("str"))
		{
			RefreshAll();
			GD.Print("อัพ STR สำเร็จ!");
		}
		else
		{
			GD.Print("แต้มไม่พอ!");
		}
	}

	// ── ปุ่ม AGI ───────────────────────────────────
	public void OnAgiPressed()
	{
		if (_player.UpgradeStat("agi"))
		{
			RefreshAll();
			GD.Print("อัพ AGI สำเร็จ!");
		}
		else
		{
			GD.Print("แต้มไม่พอ!");
		}
	}

	// ── ปุ่ม INT ───────────────────────────────────
	public void OnIntPressed()
	{
		if (_player.UpgradeStat("int"))
		{
			RefreshAll();
			GD.Print("อัพ INT สำเร็จ!");
		}
		else
		{
			GD.Print("แต้มไม่พอ!");
		}
	}
	public void OnBackPressed()
{
	string prev = GameManager.Instance.PreviousScene;

	if (prev != "")
		GetTree().ChangeSceneToFile(prev);
	else
		GetTree().ChangeSceneToFile("res://Main Menu.tscn");
}
}
