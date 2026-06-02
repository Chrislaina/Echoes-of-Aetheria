using Godot;
using System.Collections.Generic;

public partial class GameManager : Node2D
{
    [Export] public PackedScene ZombieScene;
    [Export] public NodePath PlayerPath;
    [Export] public NodePath WaveLabelPath;
    [Export] public NodePath UpgradePanelPath;
    [Export] public NodePath Option1Path;
    [Export] public NodePath Option2Path;
    [Export] public NodePath Option3Path;
    [Export] public int BaseZombieCount = 5;
    [Export] public float BaseZombieSpeed = 100.0f;
    [Export] public float SpawnRadius = 560.0f;
    [Export] public float SpawnRadiusVariance = 80.0f;

    private Player? player;
    private Label? waveLabel;
    private Panel? upgradePanel;
    private Button[] optionButtons = new Button[3];
    private RandomNumberGenerator rng = new();
    private int wave = 0;
    private bool awaitingUpgrade = false;

    private readonly string[] upgradeIds = { "fire_rate", "damage", "move_speed" };
    private readonly Dictionary<string, string> upgradeNames = new()
    {
        { "fire_rate", "Faster Fire Rate" },
        { "damage", "Increased Damage" },
        { "move_speed", "Faster Movement" }
    };

    public override void _Ready()
    {
        rng.Randomize();
        player = GetNodeOrNull<Player>(PlayerPath);
        waveLabel = GetNodeOrNull<Label>(WaveLabelPath);
        upgradePanel = GetNodeOrNull<Panel>(UpgradePanelPath);
        optionButtons[0] = GetNode<Button>(Option1Path);
        optionButtons[1] = GetNode<Button>(Option2Path);
        optionButtons[2] = GetNode<Button>(Option3Path);

        optionButtons[0].Pressed += () => OnUpgradeSelected(0);
        optionButtons[1].Pressed += () => OnUpgradeSelected(1);
        optionButtons[2].Pressed += () => OnUpgradeSelected(2);

        if (upgradePanel != null)
        {
            upgradePanel.Visible = false;
        }

        StartWave();
    }

    public override void _Process(double delta)
    {
        if (awaitingUpgrade || ZombieScene == null || player == null)
        {
            return;
        }

        if (GetTree().GetNodesInGroup("zombies").Count == 0)
        {
            ShowUpgradeMenu();
        }
    }

    private void StartWave()
    {
        if (player == null || waveLabel == null || ZombieScene == null)
        {
            return;
        }

        wave++;
        awaitingUpgrade = false;
        waveLabel.Text = $"Wave {wave}";

        int spawnCount = BaseZombieCount + (wave - 1) * 2;
        float zombieSpeed = BaseZombieSpeed + (wave - 1) * 12.0f;

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnZombie(zombieSpeed);
        }
    }

    private void SpawnZombie(float speed)
    {
        if (player == null || ZombieScene == null)
        {
            return;
        }

        var zombie = ZombieScene.Instantiate<Zombie>();
        AddChild(zombie);

        float angle = rng.RandfRange(0.0f, Mathf.Tau);
        float distance = SpawnRadius + rng.RandfRange(-SpawnRadiusVariance, SpawnRadiusVariance);
        zombie.GlobalPosition = player.GlobalPosition + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
        zombie.Speed = speed;
        zombie.PlayerPath = PlayerPath;
    }

    private void ShowUpgradeMenu()
    {
        awaitingUpgrade = true;
        if (upgradePanel != null)
        {
            upgradePanel.Visible = true;
        }

        var available = new List<string>(upgradeIds);
        for (int i = 0; i < optionButtons.Length; i++)
        {
            string choice = available[rng.RandiRange(0, available.Count - 1)];
            available.Remove(choice);
            optionButtons[i].Text = upgradeNames[choice];
            optionButtons[i].SetMeta("upgrade_id", choice);
        }
    }

    private void OnUpgradeSelected(int index)
    {
        if (player == null || upgradePanel == null || index < 0 || index >= optionButtons.Length)
        {
            return;
        }

        var button = optionButtons[index];
        if (!button.HasMeta("upgrade_id"))
        {
            return;
        }

        string upgradeId = (string)button.GetMeta("upgrade_id");
        player.ApplyUpgrade(upgradeId);
        upgradePanel.Visible = false;
        StartWave();
    }
}
