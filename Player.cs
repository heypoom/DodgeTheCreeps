using Godot;

public class Player : Area2D {
    [Export] private int Speed = 400;

    [Signal]
    public delegate void Hit();

    private Vector2 _screenSize;

    public override void _Ready() {
        _screenSize = GetViewport().GetSize();

        Connect("body_entered", this, nameof(OnPlayerBodyEntered));

        Hide();
    }

    public void Start(Vector2 position) {
        GD.Print("Player.Start()");

        Position = position;
        Show();

        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }

    private void OnPlayerBodyEntered(PhysicsBody2D body) {
        Hide();

        EmitSignal("Hit");

        GetNode<CollisionShape2D>("CollisionShape2D")
            .SetDeferred("disabled", true);
    }

    public override void _Process(float delta) {
        var velocity = new Vector2();

        if (Input.IsActionPressed("ui_right")) {
            velocity.x += 1;
        }

        if (Input.IsActionPressed("ui_left")) {
            velocity.x -= 1;
        }

        if (Input.IsActionPressed("ui_down")) {
            velocity.y += 1;
        }

        if (Input.IsActionPressed("ui_up")) {
            velocity.y -= 1;
        }

        var animatedSprite = (AnimatedSprite) GetNode("AnimatedSprite");

        if (velocity.Length() > 0) {
            velocity = velocity.Normalized() * Speed;
            animatedSprite.Play();
        }
        else {
            animatedSprite.Stop();
        }

        Position += velocity * delta;
        Position = new Vector2(
            Mathf.Clamp(Position.x, 0, _screenSize.x),
            Mathf.Clamp(Position.y, 0, _screenSize.y)
        );

        if (velocity.x != 0) {
            animatedSprite.Animation = "right";
            animatedSprite.FlipH = velocity.x < 0;
            animatedSprite.FlipV = false;
        }
        else if (velocity.y != 0) {
            animatedSprite.Animation = "up";
            animatedSprite.FlipV = velocity.y > 0;
        }
    }
}