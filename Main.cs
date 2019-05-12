using System;
using Godot;

public class Main : Node {
    [Export] private PackedScene Mob;

    private int _score;

    private readonly Random _random = new Random();

    public override void _Ready() {
        GetNode("Player").Connect("Hit", this, nameof(OnGameOver));

        GetNode("StartTimer").Connect("timeout", this, nameof(OnStartTimerTimeout));
        GetNode("ScoreTimer").Connect("timeout", this, nameof(OnScoreTimerTimeout));
        GetNode("MobTimer").Connect("timeout", this, nameof(OnMobTimerTimeout));

        GetNode("HUD").Connect("StartGame", this, nameof(OnStartGame));
    }

    private void OnStartGame() {
        var hud = (HUD) GetNode("HUD");
        if (hud == null) return;

        hud.UpdateScore(_score);
        hud.ShowMessage("Get Ready!");

        NewGame();
    }

    private void OnStartTimerTimeout() {
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }

    private void OnScoreTimerTimeout() {
        _score++;

        GetNode<HUD>("HUD").UpdateScore(_score);
    }

    private void OnMobTimerTimeout() {
        // Choose a random location on Path2D.
        var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
        mobSpawnLocation.SetOffset(_random.Next());

        // Create a Mob instance and add it to the scene.
        var mobInstance = (RigidBody2D) Mob.Instance();
        AddChild(mobInstance);

        // Set the mob's direction perpendicular to the path direction.
        float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

        // Set the mob's position to a random location.
        mobInstance.Position = mobSpawnLocation.Position;

        // Add some randomness to the direction.
        direction += RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mobInstance.Rotation = direction;

        // Choose the velocity.
        mobInstance.SetLinearVelocity(new Vector2(RandRange(150f, 250f), 0).Rotated(direction));
    }

    // We'll use this later because C# doesn't support GDScript's randi().
    private float RandRange(float min, float max) {
        return (float) _random.NextDouble() * (max - min) + min;
    }

    private void OnGameOver() {
        GD.Print("Game Over!");

        GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("ScoreTimer").Stop();

        GetNode<HUD>("HUD").ShowGameOver();
    }

    private void NewGame() {
        _score = 0;

        var player = GetNode<Player>("Player");
        var startPosition = GetNode<Position2D>("StartPosition");
        player.Start(startPosition.Position);

        GetNode<Timer>("StartTimer").Start();
    }
}