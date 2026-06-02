using Godot;

public partial class Projectile : Area2D
{
    [Export] public float Speed = 900.0f;
    [Export] public float Lifetime = 2.0f;
    [Export] public int Damage = 1;

    public Vector2 Direction = Vector2.Zero;
    private float lifeTimer = 0.0f;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    public void Launch()
    {
        // Direction should already be set by the player.
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += Direction * Speed * (float)delta;
        lifeTimer += (float)delta;
        if (lifeTimer >= Lifetime)
        {
            QueueFree();
        }
    }

    private void OnBodyEntered(Node body)
    {
        if (body is Zombie zombie)
        {
            zombie.TakeDamage(Damage);
            QueueFree();
        }
    }
}
