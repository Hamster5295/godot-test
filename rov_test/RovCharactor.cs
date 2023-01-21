using Godot;
using System;

public class RovCharactor : Node2D
{
    [Export] private float _Speed;
    [Export] private Vector2 _Destination;

    private NavigationAgent2D _Nav;

    public override void _Ready()
    {
        _Nav = GetNode<NavigationAgent2D>("Nav");
        _Nav.SetTargetLocation(_Destination);

        _Nav.Connect("velocity_computed", this, nameof(OnVelocityComputed));
    }

    public override void _PhysicsProcess(float delta)
    {
        var dest = _Nav.GetNextLocation();
        Rotation = Vector2.Up.AngleTo(dest - GlobalPosition);
        _Nav.SetVelocity(delta * _Speed * (dest - GlobalPosition).Normalized());
    }

    private void OnVelocityComputed(Vector2 v)
    {
        Translate(v);
    }
}
