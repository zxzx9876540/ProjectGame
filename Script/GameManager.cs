using Godot;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }

	public CharacterStats Stats { get; private set; } = new CharacterStats();

	public override void _Ready()
	{
		Instance = this;
	}

	public void SetStats(CharacterStats stats)
	{
		Stats = stats;
	}
	public CharacterStats Enemy { get; private set; }

public void SetEnemy(CharacterStats enemy)
{
	Enemy = enemy;
}

public void SetRandomEnemy()
{
	var list = new System.Collections.Generic.List<CharacterStats>
	{
		CharacterStats.CreateWolf(),
		CharacterStats.CreateGoblin(),
		CharacterStats.CreateOrc(),
	};
	var rng = new System.Random();
	Enemy = list[rng.Next(list.Count)];
}
public string PreviousScene { get; set; } = "";
}
