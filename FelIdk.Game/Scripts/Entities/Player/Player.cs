using System;
using Godot;

namespace FelIdk.Game.Scripts.Entities.Player;

public partial class Player : Entity
{
	[Export] public float JumpVelocity = 4.5f;
	[Export] public float MouseSens = 0.01f;
	[Export] public float LerpValue = 0.15f;
	
	private SpringArm3D _springArm;
	private Node3D _camPivot;
	private Node3D _armature;
	private AnimationTree _animationTree;
	private Camera3D _camera;
	
	private readonly float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _EnterTree()
	{
		SetMultiplayerAuthority(Name.ToString().ToInt());
	}

	public override void _Ready()
	{
		if (!IsMultiplayerAuthority()) return;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		
		_camera = GetNode<Camera3D>("CamPivot/SpringArm3D/Camera3D");
		_springArm = GetNode<SpringArm3D>("CamPivot/SpringArm3D");
		_camPivot = GetNode<Node3D>("CamPivot");
		_armature = GetNode<Node3D>("Armature");
		_animationTree = GetNode<AnimationTree>("AnimationTree");

		_camera.Current = true;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!IsMultiplayerAuthority()) return;
		
		if (Input.IsActionJustPressed("quit"))
			GetTree().Quit();

		if (@event is not InputEventMouseMotion ev) return;
		
		_camPivot.RotateY(-ev.Relative.X * MouseSens);
		_springArm.RotateX(-ev.Relative.Y * MouseSens);
		_springArm.Rotation = _springArm.Rotation with {X = (float)Mathf.Clamp(_springArm.Rotation.X, -Math.PI / 4, Math.PI / 4)};
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		
		if (!IsOnFloor())
			velocity.Y -= _gravity * (float)delta;
		
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
			velocity.Y = JumpVelocity;
		
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		direction = direction.Rotated(Vector3.Up, _camPivot.Rotation.Y);
		if (direction != Vector3.Zero)
		{
			velocity.X = Mathf.Lerp(velocity.X, direction.X * Speed, LerpValue);
			velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * Speed, LerpValue);
			_armature.Rotation = _armature.Rotation with
			{
				Y = Mathf.LerpAngle(_armature.Rotation.Y, Mathf.Atan2(-velocity.X, -velocity.Z), LerpValue)
			};
		}
		else
		{
			velocity.X = Mathf.Lerp(velocity.X, 0.0f, LerpValue);
			velocity.Z = Mathf.Lerp(velocity.Z, 0.0f, LerpValue);
		}
		
		_animationTree.Set("parameters/BlendSpace1D/blend_position", velocity.Length() / Speed);

		Velocity = velocity;
		MoveAndSlide();
	}
}