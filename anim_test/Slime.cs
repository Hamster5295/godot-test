using Godot;
using System;

public class Slime : KinematicBody2D
{
    [Export] public float Speed { get; private set; }
    [Export] public float JumpSpeed { get; private set; }
    [Export] public float Fraction { get; private set; }
    [Export] public float Gravity { get; private set; } = 9.8f;
    [Export] public int ExtraJumpCount { get; private set; } = 1;

    public Vector2 Velocity { get => _Velocity; }

    private Vector2 _Velocity = Vector2.Zero;
    private AnimationTree _AnimationTree;
    private Sprite _Sprite;

    private int _ExtraJumped = 0;

    public override void _Ready()
    {
        _Sprite = GetNode<Sprite>("Sprite");
        _AnimationTree = GetNode<AnimationTree>("AnimationTree");
        ((AnimationNodeStateMachinePlayback)_AnimationTree.Get("parameters/playback")).Start("normal");
    }

    public override void _PhysicsProcess(float delta)
    {
        _Velocity.y += Gravity * delta;
        _Velocity.x -= _Velocity.x * Fraction * delta;

        if (Input.IsActionPressed("move_left")) { _Velocity.x = -Speed; _Sprite.FlipH = true; }
        else if (Input.IsActionPressed("move_right")) { _Velocity.x = Speed; _Sprite.FlipH = false; }

        if (Input.IsActionJustPressed("move_jump"))
        {
            if (IsOnFloor())
            {
                _ExtraJumped = 0;
                Jump();
            }
            else if (_ExtraJumped < ExtraJumpCount)
            {
                _ExtraJumped++;
                Jump();
            }
        }

        if (IsOnFloor())
        {
            _Velocity.y = Mathf.Min(0, _Velocity.y);
            _ExtraJumped = 0;
        }

        _AnimationTree.Set("parameters/conditions/is_down", Velocity.y > 0);
        _AnimationTree.Set("parameters/conditions/is_up", Velocity.y < 0);
        _AnimationTree.Set("parameters/conditions/is_normal", Velocity.y == 0);

        MoveAndSlide(_Velocity, Vector2.Up);
    }

    private void Jump() => _Velocity.y = -JumpSpeed;
}
