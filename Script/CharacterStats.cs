using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

public class CharacterStats
{
	public string CharName      { get; set; } = "นักผจญภัย";
	public string CharClass     { get; set; } = "นักรบ";
	public string CharacterType { get; set; } = "";
	public int    Level         { get; set; } = 1;
	public int    Exp           { get; set; } = 0;
	public int    ExpToNext     { get; set; } = 100;
	public int    Gold          { get; set; } = 0;
	public int    StatusPoints  { get; set; } = 5;

	public int Hp    { get; set; } = 100;
	public int MaxHp { get; set; } = 100;
	public int Mp    { get; set; } = 50;
	public int MaxMp { get; set; } = 50;

	public int Str { get; set; } = 0;
	public int Agi { get; set; } = 5;
	public int Int { get; set; } = 0;
	public int Def { get; set; } = 0;

	[JsonIgnore] public int   Atk      => Str * 2;
	[JsonIgnore] public int   MAtk     => Int * 5;
	[JsonIgnore] public int   Speed    => Agi;

	public List<string> StatusEffects { get; set; } = new();

	private const string SavePath = "user://save_data.json";

	// ── HP / MP ────────────────────────────────────
	public void Heal(int amount)       => Hp = Math.Min(Hp + amount, MaxHp);
	public void RestoreMp(int amount)  => Mp = Math.Min(Mp + amount, MaxMp);
	public void TakeDamage(int damage) => Hp = Math.Max(Hp - damage, 0);
	public void FullRestore()          { Hp = MaxHp; Mp = MaxMp; }
	public bool IsAlive                => Hp > 0;

	// ── EXP / Level ────────────────────────────────
public bool AddExp(int amount)
{
	Exp += amount;
	bool didLevelUp = false;
	while (Exp >= ExpToNext)
	{
		Exp      -= ExpToNext;
		Level++;
		ExpToNext     = (int)(ExpToNext * 1.5f);
		StatusPoints += 3;
		MaxHp        += 10;
		MaxMp        += 5;
		Hp            = MaxHp;
		Mp            = MaxMp;
		didLevelUp    = true;
	}
	return didLevelUp;
}

	// ── Stat Upgrade ───────────────────────────────
	public bool UpgradeStat(string stat)
	{
		if (StatusPoints <= 0) return false;
		switch (stat.ToLower())
		{
			case "str": 
				Str++; 
				MaxHp+=10;
				Hp=Math.Min(Hp+10,MaxHp);
				break;
			case "agi": 
				Agi++;
			 	break;
			case "int": Int++; 
			MaxMp += 10;
			Mp = Math.Min(Mp + 10, MaxMp); 
			break;
			default: return false;
		}
		StatusPoints--;
		return true;
	}


	// ── Gold ───────────────────────────────────────
	public void AddGold(int amount) => Gold += amount;
	public bool SpendGold(int amount)
	{
		if (Gold < amount) return false;
		Gold -= amount;
		return true;
	}

	// ── Preset ตัวละคร ─────────────────────────────
	public static CharacterStats CreateMage()
	{
		var mage = new CharacterStats
		{
			CharName      = "เมจ",
			CharacterType = "mage",
			MaxMp  = 100, Mp  = 100,
			Str    = 2,
			Agi    = 5,
			Int    = 10,
		};
		mage.MaxHp = 50 + (mage.Str * 10);
		mage.Hp    = mage.MaxHp;
		mage.MaxMp =100 +(mage.Int*5);
		mage.Mp = mage.MaxMp;
		return mage;
	}

	public static CharacterStats CreateBosspriest()
	{
		return new CharacterStats
		{
			CharName      = "เจ้าลัทธิผู้บูชาปีศาจ",
			CharacterType = "ิboss",
			MaxHp  = 400, Hp  = 400,
			Str  = 20, Def = 10,Agi = 10,
		};
	}
	public static CharacterStats CreateWolf()
{
	return new CharacterStats
	{
		CharName      = "หมาป่า",
		CharacterType = "wolf",
		MaxHp  = 30, Hp  = 30,
		Str    = 5, Def = 1,
	};
}

public static CharacterStats CreateGoblin()
{
	return new CharacterStats
	{
		CharName      = "โกบลิน",
		CharacterType = "goblin",
		MaxHp  = 60, Hp  = 60,
		Str    = 12, Def = 3,
	};
}

public static CharacterStats CreateOrc()
{
	return new CharacterStats
	{
		CharName      = "ออร์ค",
		CharacterType = "orc",
		MaxHp  = 300, Hp = 300,
		Str    = 20, Def = 10,
	};
}

	// ── Save / Load ────────────────────────────────
	public void Save()
	{
		string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
		using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
		file?.StoreString(json);
	}

	public static CharacterStats Load()
	{
		if (!FileAccess.FileExists(SavePath)) return new CharacterStats();
		using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
		if (file == null) return new CharacterStats();
		return JsonSerializer.Deserialize<CharacterStats>(file.GetAsText()) ?? new CharacterStats();
	}

	public static void DeleteSave()
	{
		if (FileAccess.FileExists(SavePath))
			DirAccess.RemoveAbsolute(ProjectSettings.GlobalizePath(SavePath));
	}
}
