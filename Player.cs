using Godot;

public partial class Player : CharacterBody2D
{
    [Export] public PackedScene ProjectileScene;
    [Export] public float MoveSpeed = 320.0f;
    [Export] public float FireRate = 4.0f;
    [Export] public int Damage = 1;
    [Export] public float FireCooldown = 0.0f;
    [Export] public NodePath MuzzlePath = "Muzzle";

    private Node2D? muzzle;

    public override void _Ready()
    {
        muzzle = GetNodeOrNull<Node2D>(MuzzlePath);
    }

    public override void _Process(double delta)
    {
        LookAt(GetGlobalMousePosition());
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 inputVector = new Vector2(
            Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
            Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
        );

        if (inputVector.Length() > 0.0f)
        {
            inputVector = inputVector.Normalized() * MoveSpeed;
        }

        Velocity = inputVector;
        MoveAndSlide();

        FireCooldown -= (float)delta;
        if (Input.IsActionPressed("ui_accept") && FireCooldown <= 0.0f)
        {
            FireProjectile();
        }
    }

    private void FireProjectile()
    {
        if (ProjectileScene == null)
        {
            return;
        }

        var projectile = ProjectileScene.Instantiate<Projectile>();
        if (projectile == null)
        {
            return;
        }

        GetParent()?.AddChild(projectile);
        projectile.GlobalPosition = muzzle?.GlobalPosition ?? GlobalPosition;
        projectile.Direction = (GetGlobalMousePosition() - projectile.GlobalPosition).Normalized();
        projectile.Damage = Damage;
        projectile.Launch();
        FireCooldown = 1.0f / FireRate;
    }

    public void ApplyUpgrade(string upgradeId)
    {
        switch (upgradeId)
        {
            case "fire_rate":
                FireRate += 1.5f;
                break;
            case "damage":
                Damage += 1;
                break;
            case "move_speed":
                MoveSpeed += 40.0f;
                break;
        }
    }
}
