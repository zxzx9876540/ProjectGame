using Godot;
using System.Collections.Generic;

public partial class BossPriest : Control
{
	// ── Enemy HUD ───────────────────────────────────────────────────────────
	[Export] private Label       _labelEnemyName;
	[Export] private ProgressBar _hpBar;
	[Export] private Label       _labelHpVal;

	// ── Player HUD ──────────────────────────────────────────────────────────
	[Export] private Label       _labelPlayerName;
	[Export] private ProgressBar _playerHpBar;
	[Export] private Label       _labelPlayerHpVal;
	[Export] private ProgressBar _playerMpBar;
	[Export] private Label       _labelPlayerMpVal;

	// ── Battle Log ──────────────────────────────────────────────────────────
	[Export] private RichTextLabel _battleLog;

	// ── Dice Panel ──────────────────────────────────────────────────────────
	[Export] private Control _dicePanel;
	[Export] private Label   _diceFaceLabel;
	[Export] private Label   _diceResultLabel;
	[Export] private Button  _rollButton;
	[Export] private Button  _startBattleButton;

	// ── Battle result buttons ────────────────────────────────────────────────
	[Export] private Button  _btnUpgrade;
	[Export] private Button  _btnReturnMap;

	// ── Internal State ───────────────────────────────────────────────────────
	private CharacterStats _enemy;
	private CharacterStats _player;

	// ── Stats ชั่วคราว (เก็บค่าจริงไว้ แล้วสร้างตัวแทนสำหรับด่านนี้) ──────
	private float _statMultiplier = 1.0f;

	// stats ที่ apply multiplier แล้ว — ใช้ในการต่อสู้แทน _player โดยตรง
	private int _battleStr;
	private int _battleAgi;
	private int _battleInt;
	private int _battleDef;
	private int _battleAtk;   // Str*2 หลัง multiplier
	private int _battleMAtk;  // Int*6 หลัง multiplier
	private int _battlHp;
	private int _battleMaxHp;
	private int _originalHp;
	private int _originalMaxHp;

	// Auto-battle
	private bool  _battleStarted  = false;
	private bool  _battleFinished = false;
	private float _turnTimer      = 0f;
	private const float TurnInterval = 1.2f;
	private bool  _playerTurn     = true;

	// Dice animation
	private bool  _diceRolling  = false;
	private float _diceTimer    = 0f;
	private float _diceSpeed    = 0.05f;
	private float _diceElapsed  = 0f;
	private const float DiceDuration = 2.0f;
	private int   _diceResult   = 0;
	private System.Random _rng  = new System.Random();

	private static readonly string[] DiceFaceSymbols = {
		"🎲 1","🎲 2","🎲 3","🎲 4","🎲 5","🎲 6",
		"🎲 7","🎲 8","🎲 9","🎲 10","🎲 11","🎲 12",
		"🎲 13","🎲 14","🎲 15","🎲 16","🎲 17","🎲 18",
		"🎲 19","⭐ 20"
	};

	// ── Ready ────────────────────────────────────────────────────────────────
	public override void _Ready()
	{
		_enemy  = GameManager.Instance.Enemy ?? CharacterStats.CreateBosspriest();
		_player = GameManager.Instance.Stats;

		RefreshEnemyHUD();
		RefreshPlayerHUD();

		if (_btnUpgrade    != null) _btnUpgrade.Visible    = false;
		if (_btnReturnMap  != null) _btnReturnMap.Visible  = false;
		if (_diceResultLabel   != null) _diceResultLabel.Text      = "";
		if (_startBattleButton != null) _startBattleButton.Visible = false;
		if (_dicePanel         != null) _dicePanel.Visible         = true;

		AppendLog("[color=yellow]🎲 ทอยเต๋าเพื่อกำหนดพลังในด่านนี้![/color]");
	}

	// ── Process ──────────────────────────────────────────────────────────────
	public override void _Process(double delta)
	{
		float dt = (float)delta;

		if (_diceRolling)
		{
			_diceElapsed += dt;
			_diceTimer   -= dt;

			if (_diceTimer <= 0f)
			{
				int face = _rng.Next(1, 21);
				if (_diceFaceLabel != null)
					_diceFaceLabel.Text = DiceFaceSymbols[face - 1];

				float progress = _diceElapsed / DiceDuration;
				_diceSpeed = Mathf.Lerp(0.04f, 0.25f, progress);
				_diceTimer = _diceSpeed;
			}

			if (_diceElapsed >= DiceDuration)
			{
				_diceRolling = false;
				ShowDiceResult(_diceResult);
			}
			return;
		}

		if (!_battleStarted || _battleFinished) return;

		_turnTimer -= dt;
		if (_turnTimer > 0f) return;
		_turnTimer = TurnInterval;

		ExecuteTurn();
	}

	// ── Dice ─────────────────────────────────────────────────────────────────
	public void OnRollButtonPressed()
	{
		if (_diceRolling) return;

		_diceResult  = _rng.Next(1, 21);
		_diceRolling = true;
		_diceElapsed = 0f;
		_diceTimer   = 0.04f;
		_diceSpeed   = 0.04f;

		if (_rollButton        != null) _rollButton.Disabled        = true;
		if (_diceResultLabel   != null) _diceResultLabel.Text       = "";
		if (_startBattleButton != null) _startBattleButton.Visible  = false;
	}

	private void ShowDiceResult(int result)
	{
		if (_diceFaceLabel != null)
			_diceFaceLabel.Text = DiceFaceSymbols[result - 1];

		string effectText;
		if      (result <= 5)  { _statMultiplier = 0.80f; effectText = "[color=red]Stats −20%![/color]"; }
		else if (result <= 9)  { _statMultiplier = 0.90f; effectText = "[color=orange]Stats −10%[/color]"; }
		else if (result == 10) { _statMultiplier = 1.00f; effectText = "[color=white]Stats ปกติ[/color]"; }
		else if (result <= 13) { _statMultiplier = 1.25f; effectText = "[color=cyan]Stats +25%[/color]"; }
		else if (result <= 16) { _statMultiplier = 1.50f; effectText = "[color=lime]Stats +50%[/color]"; }
		else if (result <= 19) { _statMultiplier = 2.00f; effectText = "[color=green]Stats +100%[/color]"; }
		else                   { _statMultiplier = 3.00f; effectText = "[color=gold]⭐ Stats +200%![/color]"; }

		if (_diceResultLabel != null)
			_diceResultLabel.Text = $"ได้ {result} → {StripColor(effectText)}";

		AppendLog($"🎲 ทอยได้ [b]{result}[/b] → {effectText}");

		if (_startBattleButton != null) _startBattleButton.Visible = true;
	}

	// ── Apply Temporary Stats ────────────────────────────────────────────────
	/// คำนวณ stats ชั่วคราวจาก multiplier และเก็บไว้ใน _battle* variables
	/// ค่าจริงของ _player ไม่ถูกแตะเลย ปลอดภัย 100%
	private void ApplyBattleStats()
	{
		_originalHp    = _player.Hp;
		_originalMaxHp = _player.MaxHp;
		_battleStr  = Mathf.RoundToInt(_player.Str  * _statMultiplier);
		_battleAgi  = Mathf.RoundToInt(_player.Agi  * _statMultiplier);
		_battleInt  = Mathf.RoundToInt(_player.Int  * _statMultiplier);
		_battleDef  = Mathf.RoundToInt(_player.Def  * _statMultiplier);
		_battleAtk  = _battleStr * 2;   // สูตรเดิม Atk = Str*2
		_battleMAtk = _battleInt * 5;   // สูตรเดิม MAtk = Int*6
			int battleMaxHp = 50 + (_battleStr * 10);  // สูตรเดียวกับ CreateWarrior
	// คงสัดส่วน HP เดิมไว้
		float hpRatio    = (float)_player.Hp / _player.MaxHp;
		int   battleHp   = Mathf.RoundToInt(battleMaxHp * hpRatio);

		// อัปเดต HP จริงชั่วคราว
		_player.MaxHp = battleMaxHp;
		_player.Hp    = battleHp;
		// แสดง stats ที่ได้รับใน log
		AppendLog($"[color=aqua] Stats ในด่านนี้: STR {_battleStr} | AGI {_battleAgi} | INT {_battleInt} | DEF {_battleDef}[/color]");
	}

	// ── Start Battle ─────────────────────────────────────────────────────────
	public void OnStartBattlePressed()
	{
		if (_dicePanel != null) _dicePanel.Visible = false;

		// Apply stats ชั่วคราวก่อนเริ่ม
		ApplyBattleStats();

		// ใครไปก่อน — ใช้ _battleAgi (ที่ apply multiplier แล้ว)
		bool playerGoesFirst = _battleAgi >= _enemy.Agi;
		_playerTurn = playerGoesFirst;

		string first = playerGoesFirst ? _player.CharName : _enemy.CharName;
		AppendLog($"[color=yellow]⚔️ เริ่มการต่อสู้! {first} เริ่มก่อน (AGI: {_battleAgi} vs {_enemy.Agi})[/color]");

		_battleStarted = true;
		_turnTimer     = 0.3f;
	}

	// ── Battle Logic ─────────────────────────────────────────────────────────
	private void ExecuteTurn()
	{
		if (_playerTurn)
			PlayerAttackEnemy();
		else
			EnemyAttackPlayer();

		_playerTurn = !_playerTurn;

		if (!_enemy.IsAlive)  { OnBattleWin();  return; }
		if (!_player.IsAlive) { OnBattleLose(); }
	}

	private void PlayerAttackEnemy()
	{
		// dodge ศัตรู ใช้ Agi ศัตรู
		float dodgeChance = _enemy.Agi * 0.003f;
		if ((float)_rng.NextDouble() < dodgeChance)
		{
			AppendLog($"[color=cyan]{_enemy.CharName} หลบการโจมตี![/color]");
			return;
		}

		// ใช้ _battleAtk / _battleMAtk (stats ชั่วคราวแล้ว)
		int rawAtk = _player.CharacterType == "mage"
			? Mathf.Max(_battleMAtk - _enemy.Def, 1)
			: Mathf.Max(_battleAtk  - _enemy.Def, 1);

		_enemy.TakeDamage(rawAtk);
		RefreshEnemyHUD();
		AppendLog($"[color=white]{_player.CharName}[/color] โจมตี [color=red]{rawAtk}[/color] ดาเมจ! (HP ศัตรู: {_enemy.Hp}/{_enemy.MaxHp})");
	}

	private void EnemyAttackPlayer()
	{
		// dodge ผู้เล่น ใช้ _battleAgi (ที่ apply multiplier แล้ว)
		float dodgeChance = _battleAgi * 0.003f;
		if ((float)_rng.NextDouble() < dodgeChance)
		{
			AppendLog($"[color=lime]{_player.CharName} หลบการโจมตี![/color]");
			return;
		}

		// DEF ผู้เล่นก็ใช้ค่าชั่วคราวด้วย
		int dmg = Mathf.Max(_enemy.Atk - _battleDef, 1);
		_player.TakeDamage(dmg);
		RefreshPlayerHUD();
		AppendLog($"[color=orange]{_enemy.CharName}[/color] โจมตี [color=red]{dmg}[/color] ดาเมจ! (HP ผู้เล่น: {_player.Hp}/{_player.MaxHp})");
	}

	private void OnBattleWin()
	{
		_battleFinished = true;
		int exp  = _enemy.MaxHp / 2;
		int gold = _rng.Next(5, 20);
		bool levelUp = _player.AddExp(exp);
		_player.AddGold(gold);

		AppendLog($"[color=gold]🏆 ชนะ! ได้ {exp} EXP, {gold} Gold[/color]");
		if (levelUp)
			AppendLog($"[color=yellow]⬆️ Level Up! เลเวล {_player.Level}![/color]");
		
		// ใส่อันนี้แทน
		var s = GameManager.Instance.Stats;
		var temp = CharacterStats.Load();
		temp.Exp          = s.Exp;
		temp.Level        = s.Level;
		temp.ExpToNext    = s.ExpToNext;
		temp.StatusPoints = s.StatusPoints;
		// เปลี่ยนจาก _originalHp/_originalMaxHp เป็นแบบนี้
		temp.Hp           = _originalHp;
		temp.MaxHp        = _originalMaxHp;
		temp.Save();
		ShowResultButtons();
		GetTree().ChangeSceneToFile("res://ending1.tscn");
	}

	private void OnBattleLose()
{
	_battleFinished = true;
	AppendLog("[color=red]คุณตาย[/color]");
	AppendLog("[color=red]การกระทำของคุณมันไม่ใข่ความกล้าหาญเลยมันคือความโง่เขลา[/color]");

	// โหลดข้อมูลดั้งเดิมกลับมา (ไม่เอาค่าที่ถูก multiplier แก้ไป)
	_player.Hp    = _originalHp;
	_player.MaxHp = _originalMaxHp;

	ShowResultButtons();
	GetTree().ChangeSceneToFile("res://diescene.tscn");
}

	private void ShowResultButtons()
	{
		if (_btnUpgrade   != null) _btnUpgrade.Visible   = true;
		if (_btnReturnMap != null) _btnReturnMap.Visible = true;
	}

	// ── HUD Refresh ──────────────────────────────────────────────────────────
	private void RefreshEnemyHUD()
	{
		if (_labelEnemyName != null) _labelEnemyName.Text = _enemy.CharName;
		if (_hpBar          != null) { _hpBar.MaxValue = _enemy.MaxHp; _hpBar.Value = _enemy.Hp; }
		if (_labelHpVal     != null) _labelHpVal.Text = $"{_enemy.Hp} / {_enemy.MaxHp}";
		UpdateHpBarColor(_hpBar, (float)_enemy.Hp / _enemy.MaxHp);
	}

	private void RefreshPlayerHUD()
	{
		if (_labelPlayerName  != null) _labelPlayerName.Text  = _player.CharName;
		if (_playerHpBar      != null) { _playerHpBar.MaxValue = _player.MaxHp; _playerHpBar.Value = _player.Hp; }
		if (_labelPlayerHpVal != null) _labelPlayerHpVal.Text  = $"{_player.Hp} / {_player.MaxHp}";
		if (_playerMpBar      != null) { _playerMpBar.MaxValue = _player.MaxMp; _playerMpBar.Value = _player.Mp; }
		if (_labelPlayerMpVal != null) _labelPlayerMpVal.Text  = $"{_player.Mp} / {_player.MaxMp}";
		UpdateHpBarColor(_playerHpBar, (float)_player.Hp / _player.MaxHp);
	}

	private static void UpdateHpBarColor(ProgressBar bar, float ratio)
	{
		if (bar == null) return;
		if      (ratio > 0.5f)  bar.Modulate = new Color(0.2f, 0.9f, 0.2f);
		else if (ratio > 0.25f) bar.Modulate = new Color(1f,   0.75f, 0f);
		else                    bar.Modulate = new Color(0.9f, 0.1f,  0.1f);
	}

	// ── Log Helper ───────────────────────────────────────────────────────────
	private void AppendLog(string bbText)
	{
		if (_battleLog == null) return;
		_battleLog.AppendText("\n" + bbText);
		_battleLog.ScrollToLine(_battleLog.GetLineCount());
	}

	private static string StripColor(string s)
	{
		return System.Text.RegularExpressions.Regex.Replace(s, @"\[/?[^\]]+\]", "");
	}

	// ── Button Callbacks ─────────────────────────────────────────────────────
	public void OnStatUpgradePressed()
	{
		GameManager.Instance.PreviousScene = "res://BossPriest.tscn";
		GetTree().ChangeSceneToFile("res://Upgrade.tscn");
	}

	public void OnReturnMapPressed()
	{
		GetTree().ChangeSceneToFile("res://Map.tscn");
	}
}
