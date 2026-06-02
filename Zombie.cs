using Godot;

public partial class Zombie : CharacterBody2D
{
    [Export] public float Speed = 120.0f;
    [Export] public int Health = 1;
    [Export] public NodePath PlayerPath;

    private Player? player;

    public override void _Ready()
    {
        AddToGroup("zombies");
        player = GetNodeOrNull<Player>(PlayerPath);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (player == null)
        {
            return;
        }

        Vector2 direction = player.GlobalPosition - GlobalPosition;
        if (direction.Length() > 8.0f)
        {
            Velocity = direction.Normalized() * Speed;
            MoveAndSlide();
        }
        else
        {
            Velocity = Vector2.Zero;
        }
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            QueueFree();
        }
    }
}
