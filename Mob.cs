using System;
using Godot;

public class Mob : RigidBody2D {
    [Export] public int MinSpeed = 150;
    [Export] public int MaxSpeed = 250;

    private string[] _mobTypes = {"walk", "swim", "fly"};

    static private Random _random = new Random();

    public override void _Ready() {
        var sprite = (AnimatedSprite) GetNode("AnimatedSprite");
        var animationIndex = _random.Next(0, _mobTypes.Length);

        sprite.Animation = _mobTypes[animationIndex];

        GetNode("Visibility").Connect("screen_exited", this, nameof(OnVisibilityScreenExited));
    }

    private void OnVisibilityScreenExited() {
        QueueFree();
    }
}